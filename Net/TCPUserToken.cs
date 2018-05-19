using System;
using System.Net;
using System.Net.Sockets;
using RC.Core.Misc;
using RC.Net.Protocol;

namespace RC.Net
{
	//此实例对socket的所有操作需调用者自行进行线程同步
	public sealed class TCPUserToken : IUserToken
	{
		public delegate void ProcessDataOutputHandler( Packet packet );

		public ushort id { get; }
		public SocketEvent? disconnectEvent => this._disconnectEvent;
		public EndPoint remoteEndPoint => this._conn?.RemoteEndPoint;

		internal SocketAsyncEventArgs receiveEventArgs { get; private set; }

		private TCPServer _server;
		private Socket _conn;
		private long _activeDateTime;
		private SocketEvent? _disconnectEvent;
		private readonly StreamBuffer _cache = new StreamBuffer();

		internal TCPUserToken( TCPServer server, ushort id )
		{
			this.id = id;
			this._server = server;
			this.receiveEventArgs = new SocketAsyncEventArgs();
			this.receiveEventArgs.SetBuffer( new byte[NetworkConfig.BUFFER_SIZE], 0, NetworkConfig.BUFFER_SIZE );
			this.receiveEventArgs.UserToken = this;
			this.receiveEventArgs.Completed += this.OnIOComplete;
		}

		internal void Dispose()
		{
			this.receiveEventArgs.Completed -= this.OnIOComplete;
			this.receiveEventArgs.UserToken = null;
			this.receiveEventArgs = null;
			this._server = null;
		}

		internal void OnAccepted( Socket conn, long connectTime )
		{
			this._activeDateTime = connectTime;
			this._conn = conn;
		}

		internal void MarkToDisconnect( string msg, SocketError errorCode )
		{
			if ( this._disconnectEvent != null )
				return;
			this._disconnectEvent = new SocketEvent( SocketEvent.Type.Disconnect, msg, errorCode, this );
		}

		internal void Close()
		{
			if ( this._conn == null )
				return;

			if ( this._conn.Connected )
				this._conn.Shutdown( SocketShutdown.Both );
			this._conn.Close();
			this._conn = null;
			this._disconnectEvent = null;
			this._cache.Clear();
		}

		private void OnIOComplete( object sender, SocketAsyncEventArgs asyncEventArgs )
		{
			switch ( asyncEventArgs.LastOperation )
			{
				case SocketAsyncOperation.Receive:
					this._server.ProcessReceive( asyncEventArgs );
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
			if ( this._cache.length == 0 )
				return;

			this._activeDateTime = TimeUtils.utcTime;

			byte[] data;
			lock ( this._cache )
			{
				int len = LengthEncoder.Decode( this._cache.GetBuffer(), 0, this._cache.position, out data );
				if ( data == null )
					return;
				this._cache.Strip( len, ( int )this._cache.length - len );
			}

			Packet packet = NetworkHelper.DecodePacket( data, 0, data.Length );
			packet.OnReceive();
			outputHandler.Invoke( packet );

			if ( this._cache.length > 0 )
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
