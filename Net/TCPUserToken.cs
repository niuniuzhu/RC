using RC.Core.Misc;
using RC.Net.Protocol;
using System;
using System.Net;
using System.Net.Sockets;

namespace RC.Net
{
	//此实例对socket的所有操作需调用者自行进行线程同步
	public sealed class TCPUserToken : IUserToken
	{
		public delegate void ProcessDataOutputHandler( Packet packet );

		public ushort id { get; private set; }
		public SocketEvent? disconnectEvent => this._disconnectEvent;
		public EndPoint remoteEndPoint => this._conn?.RemoteEndPoint;

		internal SocketAsyncEventArgs receiveEventArgs { get; private set; }

		private TCPServer _server;
		private Socket _conn;
		private long _activeDateTime;
		private SocketEvent? _disconnectEvent;
		private readonly StreamBuffer _cache = new StreamBuffer();

		internal TCPUserToken( ushort id )
		{
			this.id = id;
			this.receiveEventArgs = new SocketAsyncEventArgs();
			this.receiveEventArgs.SetBuffer( new byte[NetworkConfig.BUFFER_SIZE], 0, NetworkConfig.BUFFER_SIZE );
			this.receiveEventArgs.UserToken = this;
			this.receiveEventArgs.Completed += this.OnIOComplete;
		}

		public void Dispose()
		{
			this.receiveEventArgs.Completed -= this.OnIOComplete;
			this.receiveEventArgs.UserToken = null;
			this.receiveEventArgs = null;
		}

		public void OnSpawn( TCPServer server, Socket conn, long connectTime )
		{
			this._server = server;
			this._activeDateTime = connectTime;
			this._conn = conn;
		}

		public void OnDespawn()
		{
			if ( this._conn == null )
				return;
			if ( this._conn.Connected )
				this._conn.Shutdown( SocketShutdown.Both );
			this._conn.Close();
			this._conn = null;
			this._disconnectEvent = null;
			lock ( this._cache )
				this._cache.Clear();
			this._server = null;
		}

		public void MarkToDisconnect( string msg, SocketError errorCode )
		{
			if ( this._disconnectEvent != null )
				return;
			this._disconnectEvent = new SocketEvent( SocketEvent.Type.Disconnect, msg, errorCode, this );
		}

		private void OnIOComplete( object sender, SocketAsyncEventArgs asyncEventArgs )
		{
			switch ( asyncEventArgs.LastOperation )
			{
				case SocketAsyncOperation.Receive:
					this._server?.ProcessReceive( asyncEventArgs );
					break;
			}
		}

		internal bool ReceiveAsync( SocketAsyncEventArgs receiveEventArgs )
		{
			return this._conn.ReceiveAsync( receiveEventArgs );
		}

		internal void CacheData( byte[] buffer, int offset, int size )
		{
			lock ( this._cache )
				this._cache.Write( buffer, offset, size );
		}

		internal void ProcessData( ProcessDataOutputHandler outputHandler )
		{
			lock ( this._cache )
			{
				if ( this._cache.length == 0 )
					return;
			}

			this._activeDateTime = TimeUtils.utcTime;

			byte[] data;
			long cacheLength;
			lock ( this._cache )
			{
				int len = LengthEncoder.Decode( this._cache.GetBuffer(), 0, this._cache.position, out data );
				if ( data == null )
					return;
				this._cache.Strip( len, ( int )this._cache.length - len );
				cacheLength = this._cache.length;
			}

			Packet packet = NetworkHelper.DecodePacket( data, 0, data.Length );
			packet.OnReceive();
			outputHandler.Invoke( packet );

			if ( cacheLength > 0 )
				this.ProcessData( outputHandler );
		}

		public void Send( Packet packet )
		{
			packet.OnSend();
			this.Send( NetworkHelper.EncodePacket( packet ) );
		}

		public void Send( byte[] data )
		{
			this.Send( data, 0, data.Length );
		}

		public void Send( byte[] data, int offset, int size )
		{
			if ( this._conn == null )
				return;
			data = LengthEncoder.Encode( data, offset, size );
			try
			{
				this._conn.Send( data, 0, data.Length, SocketFlags.None );
			}
			catch ( ObjectDisposedException )
			{

			}
			catch ( SocketException e )
			{
				this.MarkToDisconnect( $"Client send error, remote Address: {this.remoteEndPoint}", e.SocketErrorCode );
			}
		}
	}
}
