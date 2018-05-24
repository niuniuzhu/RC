using RC.Core.Misc;
using RC.Net.Protocol;
using System;
using System.Net.Sockets;

namespace RC.Net
{
	public sealed class TCPClient : INetClient
	{
		private enum State
		{
			Disconnect,
			Connecting,
			Connected
		}

		public event SocketEventHandler OnSocketEvent;

		public ushort id { get; private set; }

		private State _state = State.Disconnect;
		private Socket _socket;
		private SocketEvent? _closeEvent;
		private readonly byte[] _socketBuffer = new byte[8 * NetworkConfig.BUFFER_SIZE];
		private readonly StreamBuffer _cache = new StreamBuffer();
		private readonly NetworkUpdateContext _updateContext = new NetworkUpdateContext();
		private readonly SimpleScheduler _pingScheduler = new SimpleScheduler();
		private readonly ClientRPCManager _rpcManager = new ClientRPCManager();

		internal TCPClient()
		{
		}

		public void Dispose()
		{
			this.Close();
			this._cache.Close();
		}

		public void Close()
		{
			Socket socket = this._socket;
			lock ( this._cache )
			{
				if ( this._socket == null )
					return;
				this._socket = null;
				this._cache.Clear();
			}

			this._state = State.Disconnect;

			if ( socket.Connected )
				socket.Shutdown( SocketShutdown.Both );
			socket.Close();

			this._pingScheduler.Stop();
			this._rpcManager.Clear();
		}

		private void MarkToClose( string msg, SocketError errorCode )
		{
			if ( this._closeEvent != null )
				return;
			this._closeEvent = new SocketEvent( SocketEvent.Type.Close, msg, errorCode, null );
		}

		public void Connect( string ip, int port )
		{
			this.Close();

			this._state = State.Connecting;
			this._socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			this._socket.NoDelay = true;
			try
			{
				this._socket.BeginConnect( ip, port, this.ProcessConnect, null );
			}
			catch ( SocketException e )
			{
				this.MarkToClose( e.ToString(), e.SocketErrorCode );
			}
		}

		private void ProcessConnect( IAsyncResult asyncConnect )
		{
			if ( this._socket == null )
				return;
			try
			{
				this._socket.EndConnect( asyncConnect );
			}
			catch ( ObjectDisposedException )
			{
				return;
			}
			catch ( SocketException e )
			{
				this.MarkToClose( e.ToString(), e.SocketErrorCode );
				return;
			}
			this.StartReceive();
		}

		private void StartReceive()
		{
			if ( this._socket == null )
				return;
			try
			{
				this._socket.BeginReceive( this._socketBuffer, 0, 8 * NetworkConfig.BUFFER_SIZE, SocketFlags.None,
										   this.ProcessReceive, null );
			}
			catch ( ObjectDisposedException )
			{
			}
			catch ( SocketException e )
			{
				this.MarkToClose( e.ToString(), e.SocketErrorCode );
			}
		}

		private void ProcessReceive( IAsyncResult ar )
		{
			if ( this._socket == null )
				return;

			int revCount;
			try
			{
				revCount = this._socket.EndReceive( ar );
			}
			catch ( ObjectDisposedException )
			{
				return;
			}
			catch ( SocketException e )
			{
				this.MarkToClose( e.ToString(), e.SocketErrorCode );
				return;
			}
			if ( revCount == 0 )
			{
				this.MarkToClose( "Receive zero bytes", SocketError.NoData );
				return;
			}
			lock ( this._cache )
			{
				if ( this._socket != null )
					this._cache.Write( this._socketBuffer, 0, revCount );
			}
			this.StartReceive();
		}

		private void SendPing()
		{
			this.Send( new PacketHeartBeat( 0 ) );
		}

		public void Send( Packet packet, RPCHandler callback = null )
		{
			this._rpcManager.Maped( packet, callback );
			packet.OnSend();
			this.Send( NetworkHelper.EncodePacket( packet ) );
		}

		private void Send( byte[] data )
		{
			this.Send( data, 0, data.Length );
		}

		private void Send( byte[] data, int offset, int size )
		{
			if ( this._socket == null )
				return;

			data = LengthEncoder.Encode( data, offset, size );
			try
			{
				this._socket.Send( data, 0, data.Length, SocketFlags.None );
			}
			catch ( SocketException e )
			{
				this.MarkToClose( e.ToString(), e.SocketErrorCode );
			}
		}

		private void ProcessReceiveDatas()
		{
			if ( this._cache.length == 0 )
				return;

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

			if ( packet.module == NetworkConfig.INTERNAL_MODULE && packet.command == 1 )
			{
				this.id = ( ( PacketAccept )packet ).tokenId;
				this._state = State.Connected;
				this._pingScheduler.Start( NetworkConfig.PING_INTERVAL, this.SendPing );
				this.OnSocketEvent?.Invoke( new SocketEvent( SocketEvent.Type.Connect, string.Empty, SocketError.Success, null ) );
			}
			else
			{
				this._rpcManager.Invoke( packet );
				this.OnSocketEvent?.Invoke( new SocketEvent( SocketEvent.Type.Receive, packet, null ) );
			}

			if ( this._cache.length > 0 )
				this.ProcessReceiveDatas();
		}

		public void Update( long dt )
		{
			this._updateContext.deltaTime = dt;
			this._updateContext.time += dt;

			this.ProcessReceiveDatas();

			switch ( this._state )
			{
				case State.Connected:
					this._pingScheduler.Update( dt );
					break;
			}
			if ( this._closeEvent != null )
			{
				this.OnSocketEvent?.Invoke( this._closeEvent.Value );
				this._closeEvent = null;
				this.Close();
			}
		}
	}
}