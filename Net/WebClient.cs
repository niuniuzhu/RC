#if !UNITY_IPHONE
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using RC.Core.Math;

namespace RC.Net
{
	public enum WebClientState
	{
		Idle = 0,
		Sending,
		Reciving,
		Finished,
		Error
	}

	/// <summary>
	/// web客户端
	/// </summary>
	public class WebClient : IDisposable
	{
		private const int DEFAULT_BUF_SIZE = 1024 * 4; //4k

		#region Fields

		private string _url;
		private WebRequest _req;
		private WebResponse _resp;

		protected long _size = -1;
		protected long _recived;
		protected string _error;
		protected WebClientState _state = WebClientState.Idle;
		protected byte[] _buf = new byte[DEFAULT_BUF_SIZE];

		private Stream _stream; //reponse stream
		private int _outType; // 0-bytes, 1-stream, 2-file
		private Stream _saveStream; //output stream
		private string _saveFile;
		private bool _isDisposed;

		#endregion

		#region Properties

		/// <summary>
		/// 下载的url
		/// </summary>
		public string url => this._url;

		/// <summary>
		/// 内部WebRequest对象
		/// </summary>
		public WebRequest request => this._req;

		/// <summary>
		/// 内容WebResponse对象
		/// </summary>
		public WebResponse response => this._resp;

		/// <summary>
		/// 缓冲区大小
		/// </summary>
		public int bufSize
		{
			get => this._buf.Length;
			set
			{
				int newSize = value;
				if ( newSize <= 0 )
					throw new ArgumentException( "Buffer size must be positive!" );
				this._buf = new byte[newSize];
			}
		}

		/// <summary>
		/// 发送或下载内容总字节大小
		/// </summary>
		public long size => this._size;

		/// <summary>
		/// 已发送或下载数据字节大小
		/// </summary>
		public long recived => this._recived;

		/// <summary>
		/// 下载进度（0.0~1.0）
		/// </summary>
		public float progress
		{
			get
			{
				if ( this._size <= 0 )
					return 0f;
				return ( float )this._recived / this._size;
			}
		}

		/// <summary>
		/// 输出类型，0-自由获取数据，1-输出到数据流，2-输出到文件
		/// </summary>
		public int outType => this._outType;

		/// <summary>
		/// 当前状态
		/// </summary>
		public WebClientState state => this._state;

		/// <summary>
		/// 指示是否已结束下载（一般用于异步模式）
		/// </summary>
		public bool isDone => ( this._state == WebClientState.Finished || this._state == WebClientState.Error );

		/// <summary>
		/// 不为空表示出现错误
		/// </summary>
		public string error => this._error;

		/// <summary>
		/// 下载的数据（一般用于异步模式）
		/// </summary>
		public byte[] bytes
		{
			get
			{
				if ( this._outType == 0 && !this._isDisposed )
					return ( ( MemoryStream )this._saveStream ).ToArray();
				return null;
			}
		}

		/// <summary>
		/// 下载的文本内容（一般用于异步模式）
		/// </summary>
		public string text => this.GetText();

		/// <summary>
		/// 获得下载的文本内容（一般用于异步模式）
		/// </summary>
		public virtual string GetText( string encoding = null )
		{
			if ( this._outType != 0 || this._isDisposed )
				return null;

			Encoding enc = string.IsNullOrEmpty( encoding ) ? Encoding.Default : Encoding.GetEncoding( encoding );
			return enc.GetString( this.bytes );
		}

		#endregion

		#region Constructors

		/// <summary>
		/// 构造器
		/// </summary>
		/// <param name="url">请求url</param>
		/// <param name="headers">请求header数据</param>
		public WebClient( string url, IDictionary headers = null )
		{
			this.SetUrl( url );
			if ( headers != null )
				this.SetHeaders( headers );
		}

		#endregion

		#region Async Interfaces

		private void StartGetResponse( object param )
		{
			this._req.BeginGetResponse( this.OnAsyncGetResponse, this ); //网络连不上时，为什么此方法会阻塞？
		}

		/// <summary>
		/// 异步获取数据
		/// </summary>
		public void AsyncGet()
		{
			this._outType = 0;
			this._state = WebClientState.Sending;
			this._saveStream = new MemoryStream();
			//_req.BeginGetResponse(OnAsyncGetResponse, this);
			ThreadPool.QueueUserWorkItem( this.StartGetResponse );
		}

		/// <summary>
		/// 异步请求资源并保存到流
		/// </summary>
		/// <param name="saveStream">输出流</param>
		public void AsyncToStream( Stream saveStream )
		{
			this._outType = 1;
			this._state = WebClientState.Sending;
			this._saveStream = saveStream;
			//_req.BeginGetResponse(OnAsyncGetResponse, this);
			ThreadPool.QueueUserWorkItem( this.StartGetResponse );
		}

		/// <summary>
		/// 异步请求资源并保存到文件
		/// </summary>
		/// <param name="saveFile">输出文件名</param>
		public void AsyncToFile( string saveFile )
		{
			this._saveFile = saveFile;
			this._outType = 2;
			this._state = WebClientState.Sending;
			this._saveStream = File.Open( saveFile, FileMode.Create, FileAccess.Write, FileShare.None );
			//_req.BeginGetResponse(OnAsyncGetResponse, this);
			ThreadPool.QueueUserWorkItem( this.StartGetResponse );
		}

		private void StartGetRequestStream( object state )
		{
			this._req.BeginGetRequestStream( this.OnAsyncGetRequestStream, state ); //网络连不上时，为什么此方法会阻塞？
		}

		/// <summary>
		/// 异步请求提交数据并获取数据
		/// </summary>
		/// <param name="postData">提交数据</param>
		public void AsyncPost( byte[] postData )
		{
			this._outType = 0;
			this._state = WebClientState.Sending;
			this._saveStream = new MemoryStream();
			this._req.Method = "POST";
			//_req.BeginGetRequestStream(OnAsyncGetRequestStream, postData);
			ThreadPool.QueueUserWorkItem( this.StartGetRequestStream, postData );
		}

		/// <summary>
		/// 异步请求提交数据并获取数据
		/// </summary>
		/// <param name="dataStream">输入数据流</param>
		public void AsyncPost( Stream dataStream )
		{
			this._outType = 0;
			this._state = WebClientState.Sending;
			this._saveStream = new MemoryStream();
			this._req.Method = "POST";
			//_req.BeginGetRequestStream(OnAsyncGetRequestStream, dataStream);
			ThreadPool.QueueUserWorkItem( this.StartGetRequestStream, dataStream );
		}

		/// <summary>
		/// 异步请求提交数据并保存到流
		/// </summary>
		/// <param name="postData">提交数据</param>
		/// <param name="saveStream">输出流</param>
		public void AsyncPostAndToStream( byte[] postData, Stream saveStream )
		{
			this._outType = 1;
			this._state = WebClientState.Sending;
			this._saveStream = saveStream;
			this._req.Method = "POST";
			//_req.BeginGetRequestStream(OnAsyncGetRequestStream, postData);
			ThreadPool.QueueUserWorkItem( this.StartGetRequestStream, postData );
		}

		/// <summary>
		/// 异步请求提交数据并保存到流
		/// </summary>
		/// <param name="dataStream">输入数据流</param>
		/// <param name="saveStream">输出流</param>
		public void AsyncPostAndToStream( Stream dataStream, Stream saveStream )
		{
			this._outType = 1;
			this._state = WebClientState.Sending;
			this._saveStream = saveStream;
			this._req.Method = "POST";
			//_req.BeginGetRequestStream(OnAsyncGetRequestStream, dataStream);
			ThreadPool.QueueUserWorkItem( this.StartGetRequestStream, dataStream );
		}

		/// <summary>
		/// 异步请求提交数据并保存到文件
		/// </summary>
		/// <param name="postData">提交数据</param>
		/// <param name="saveFile">输出文件名</param>
		public void AsyncPostAndToFile( byte[] postData, string saveFile )
		{
			this._saveFile = saveFile;
			this._outType = 2;
			this._state = WebClientState.Sending;
			this._saveStream = File.Open( saveFile, FileMode.Create, FileAccess.Write, FileShare.None );
			this._req.Method = "POST";
			//_req.BeginGetRequestStream(OnAsyncGetRequestStream, postData);
			ThreadPool.QueueUserWorkItem( this.StartGetRequestStream, postData );
		}

		/// <summary>
		/// 异步请求提交数据并保存到文件
		/// </summary>
		/// <param name="dataStream">输入数据流</param>
		/// <param name="saveFile">输出文件名</param>
		public void AsyncPostAndToFile( Stream dataStream, string saveFile )
		{
			this._saveFile = saveFile;
			this._outType = 2;
			this._state = WebClientState.Sending;
			this._saveStream = File.Open( saveFile, FileMode.Create, FileAccess.Write, FileShare.None );
			this._req.Method = "POST";
			//_req.BeginGetRequestStream(OnAsyncGetRequestStream, dataStream);
			ThreadPool.QueueUserWorkItem( this.StartGetRequestStream, dataStream );
		}

		#endregion

		#region Async Impls

		private void OnAsyncGetRequestStream( IAsyncResult result )
		{
			if ( this.isDone )
				return;

			try
			{
				Stream os = this._req.EndGetRequestStream( result );

				object mState = result.AsyncState;
				if ( mState is Stream )
				{
					Stream stream = ( Stream )mState;
					this._size = -1;
					try
					{
						this._size = stream.Length;
					}
					catch
					{
						this._size = -1;
					}
					this._recived = 0;
					do
					{
						int tmp = stream.Read( this._buf, 0, this._buf.Length );
						if ( tmp > 0 )
						{
							os.Write( this._buf, 0, tmp );
							this._recived += tmp;
						}
						else
							break;
					}
					while ( true );
				}
				else
				{
					byte[] data = ( byte[] )mState;
					this._size = data.Length;
					this._recived = 0;
					while ( this._recived < this._size )
					{
						int bs = ( int )MathUtils.Min( this._buf.Length, this._size - this._recived );
						os.Write( data, ( int )this._recived, bs );
						this._recived += bs;
					}
				}
				os.Close();
			}
			catch ( Exception e )
			{
				this._error = "Send request data error! [" + e.Message + "]";
				this._state = WebClientState.Error;
				return;
			}

			try
			{
				this._req.BeginGetResponse( this.OnAsyncGetResponse, this );
			}
			catch ( Exception e )
			{
				this._error = "Get response error! [" + e.Message + "]";
				this._state = WebClientState.Error;
			}
		}

		private void OnAsyncGetResponse( IAsyncResult result )
		{
			if ( this.isDone )
				return;

			try
			{
				this._resp = this._req.EndGetResponse( result );
				this._size = this._resp.ContentLength;
				this._recived = 0;
				this._state = WebClientState.Reciving;
			}
			catch ( Exception e )
			{
				this._error = "Get response error! [" + e.Message + "]";
				this._state = WebClientState.Error;
				return;
			}

			this.OnAsyncGetResponse();
		}

		protected virtual void OnAsyncGetResponse()
		{
			try
			{
				this._stream = this._resp.GetResponseStream();
				this._stream.BeginRead( this._buf, 0, this._buf.Length, this.OnAsyncReciveData, this );
			}
			catch ( Exception e )
			{
				this._error = "Get response input stream error! [" + e.Message + "]";
				this._state = WebClientState.Error;
			}
		}

		private void OnAsyncReciveData( IAsyncResult result )
		{
			if ( this.isDone )
				return;

			try
			{
				int len = this._stream.EndRead( result );
				if ( len > 0 )
				{
					this._recived += len;

					try
					{
						this._saveStream.Write( this._buf, 0, len );
					}
					catch ( Exception e )
					{
						this._error = "Write data error! [" + e.Message + "]";
						this._state = WebClientState.Error;
						return;
					}

					this._stream.BeginRead( this._buf, 0, this._buf.Length, this.OnAsyncReciveData, this );
				}
				else
				{
					this._stream.Close();
					this._saveStream.Flush();
					if ( this._outType != 1 )
						this._saveStream.Close();
					this._state = WebClientState.Finished;
				}
			}
			catch ( Exception e )
			{
				this._error = "Recive data error! [" + e.Message + "]";
				this._state = WebClientState.Error;
			}
		}

		#endregion

		#region Sync Interfaces

		/// <summary>
		/// 请求获得WebResponse对象
		/// </summary>
		/// <returns></returns>
		public WebResponse GetResponse()
		{
			this._size = 0;
			this._recived = 0;
			this._state = WebClientState.Sending;
			this._resp = this._req.GetResponse();

			this._size = this._resp.ContentLength;
			this._state = WebClientState.Reciving;
			return this._resp;
		}

		/// <summary>
		/// 同步请求Post提交数据
		/// </summary>
		/// <param name="postData">提交数据</param>
		public virtual void PostData( byte[] postData )
		{
			this._size = postData.Length;
			this._recived = 0;
			this._state = WebClientState.Sending;
			Stream os = this._req.GetRequestStream();
			while ( this._recived < this._size )
			{
				int bs = ( int )MathUtils.Min( this._buf.Length, this._size - this._recived );
				os.Write( postData, ( int )this._recived, bs );
				this._recived += bs;
			}
		}

		/// <summary>
		/// 同步请求Post提交数据
		/// </summary>
		/// <param name="dataStream">输入数据流</param>
		public virtual void PostStream( Stream dataStream )
		{
			Stream os = this._req.GetRequestStream();
			this._size = -1;
			try
			{
				this._size = dataStream.Length;
			}
			catch
			{
				this._size = -1;
			}
			this._recived = 0;
			do
			{
				int tmp = dataStream.Read( this._buf, 0, this._buf.Length );
				if ( tmp > 0 )
				{
					os.Write( this._buf, 0, tmp );
					this._recived += tmp;
				}
				else
					break;
			}
			while ( true );
		}

		protected void CopyStream( Stream from, Stream to )
		{
			do
			{
				int tmp = from.Read( this._buf, 0, this._buf.Length );
				if ( tmp > 0 )
				{
					to.Write( this._buf, 0, tmp );
					this._recived += tmp;
				}
				else
					break;
			}
			while ( true );
			to.Flush();
		}

		/// <summary>
		/// 同步获取数据
		/// </summary>
		public virtual byte[] GetBytes()
		{
			this._outType = 0;
			this._saveStream = new MemoryStream();

			if ( this._resp == null )
				this.GetResponse();

			this._stream = this._resp.GetResponseStream();
			this.CopyStream( this._stream, this._saveStream );
			this._saveStream.Close();
			this._state = WebClientState.Finished;
			return this.bytes;
		}

		/// <summary>
		/// 同步获取字符串数据
		/// </summary>
		public string GetString( string encoding = null )
		{
			this.GetBytes();
			return this.GetText( encoding );
		}

		/// <summary>
		/// 同步保存数据到流
		/// </summary>
		public virtual void ToStream( Stream saveStream )
		{
			this._outType = 1;
			this._saveStream = saveStream;

			if ( this._resp == null )
				this.GetResponse();

			this._stream = this._resp.GetResponseStream();
			this.CopyStream( this._stream, this._saveStream );
			this._state = WebClientState.Finished;
		}

		/// <summary>
		/// 同步保存数据到文件
		/// </summary>
		public virtual void ToFile( string saveFile )
		{
			this._saveFile = saveFile;
			this._outType = 2;
			this._saveStream = File.Open( saveFile, FileMode.Create, FileAccess.Write, FileShare.None );

			if ( this._resp == null )
				this.GetResponse();

			this._stream = this._resp.GetResponseStream();
			this.CopyStream( this._stream, this._saveStream );
			this._saveStream.Close();
			this._state = WebClientState.Finished;
		}

		#endregion

		protected void SetUrl( string url )
		{
			this._url = url;
			this._req = WebRequest.Create( url );
		}

		/// <summary>
		/// 设置请求header数据
		/// </summary>
		/// <param name="headers">请求header数据</param>
		public void SetHeaders( IDictionary headers )
		{
			SetHeaders( this._req, headers );
		}

		/// <summary>
		/// 关闭对象，释放资源
		/// </summary>
		public virtual void Dispose()
		{
			if ( this._isDisposed )
				return;

			if ( this._req != null )
			{
				try
				{
					this._req.Abort();
				}
				catch
				{ }
			}

		    this._stream?.Close();

		    if ( this._resp != null )
			{
				try
				{
					this._resp.Close();
				}
				catch
				{ }
			}

			if ( this._outType == 2 )
			{
				this._saveStream.Close();
				if ( !string.IsNullOrEmpty( this._error ) ) // error
				{
					try
					{
						File.Delete( this._saveFile );
					}
					catch
					{ }
				}
			}

			this._state = WebClientState.Finished;
			this._error = "Closed";

			this._req = null;
			this._resp = null;
			this._stream = null;
			this._saveStream = null;
			this._buf = null;
			this._isDisposed = true;
		}

		// inner methods

		// static method

		protected static void SetHeaders( WebRequest req, IDictionary headers )
		{
			foreach ( DictionaryEntry de in headers )
				req.Headers.Add( "" + de.Key, "" + de.Value );
		}

	}
}
#endif