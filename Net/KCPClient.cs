using System;
using System.Net;
using System.Net.Sockets;
using RC.Core.Misc;
using RC.Core.Structure;
using RC.Net.Kcp;
using RC.Net.Protocol;

namespace RC.Net
{
	public sealed class KCPClient : INetClient
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
		private KCPProxy _kcpProxy;
		private UdpClient _udpClient;
		private IPEndPoint _remoteEndPoint;
		private SocketEvent? _closeEvent;
		private long _activeTime;
		private readonly NetworkUpdateContext _updateContext = new NetworkUpdateContext();
		private readonly SwitchQueue<ReceivedData> _receivedDatas = new SwitchQueue<ReceivedData>();
		private readonly SimpleScheduler _pingScheduler = new SimpleScheduler();
		private readonly ClientRPCManager _rpcManager = new ClientRPCManager();

		internal KCPClient()
		{
		}

		public void Dispose()
		{
			this.Close();
		}

		public void Close()
		{
			UdpClient client = this._udpClient;
			lock ( this._receivedDatas )
			{
				if ( this._udpClient == null )
					return;
				this._udpClient = null;
				this._receivedDatas.Clear();
			}

			this._state = State.Disconnect;

			if ( this._kcpProxy != null )
			{
				this._kcpProxy.Dispose();
				this._kcpProxy = null;
			}
			this._pingScheduler.Stop();

			client.Close();
			client.Dispose();
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
			this._udpClient = new UdpClient();
			try
			{
				this._udpClient.Connect( ip, port );
			}
			catch ( SocketException e )
			{
				this.MarkToClose( e.ToString(), e.SocketErrorCode );
				return;
			}

			this.StartReceive();
			this.SendHandShake();
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
			this._kcpProxy?.Send( data, offset, size );
		}

		private int SendDirect( byte[] data, int size )
		{
			if ( this._udpClient == null )
				return 0;

			int len;
			try
			{
				len = this._udpClient.Send( data, size );
			}
			catch ( SocketException e )
			{
				this.MarkToClose( e.ToString(), e.SocketErrorCode );
				return 0;
			}
			return len;
		}

		private void SendHandShake()
		{
			byte[] data = new byte[NetworkConfig.SIZE_OF_CONN_KEY + NetworkConfig.SIZE_OF_SIGNATURE];
			int offset = ByteUtils.Encode32u( data, 0, NetworkConfig.CONN_KEY );
			offset += ByteUtils.Encode16u( data, offset, NetworkConfig.HANDSHAKE_SIGNATURE );
			this.SendDirect( data, offset );
		}

		private void StartReceive()
		{
			if ( this._udpClient == null )
				return;
			try
			{
				this._udpClient.BeginReceive( this.ProcessReceive, null );
			}
			catch ( ObjectDisposedException )
			{
			}
			catch ( SocketException e )
			{
				this.MarkToClose( e.ToString(), e.SocketErrorCode );
			}
		}

		private void ProcessReceive( IAsyncResult asyncReceive )
		{
			if ( this._udpClient == null )
				return;

			byte[] data;
			try
			{
				data = this._udpClient.EndReceive( asyncReceive, ref this._remoteEndPoint );
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
			lock ( this._receivedDatas )
			{
				if ( this._udpClient != null )
				{
					ReceivedData receivedData = new ReceivedData( data, this._remoteEndPoint );
					this._receivedDatas.Push( receivedData );
				}
			}
			this.StartReceive();
		}

		private bool VerifyConnKey( byte[] buffer, ref int offset, ref int size )
		{
			if ( size < NetworkConfig.SIZE_OF_CONN_KEY )
				return false;

			uint key = ByteUtils.Decode32u( buffer, offset );
			if ( key != NetworkConfig.CONN_KEY )
				return false;

			offset += NetworkConfig.SIZE_OF_CONN_KEY;
			size -= NetworkConfig.SIZE_OF_CONN_KEY;
			return true;
		}

		private bool VerifyHandshakeAck( byte[] data, ref int offset, ref int size )
		{
			if ( size < NetworkConfig.SIZE_OF_SIGNATURE )
				return false;

			ushort signature = ByteUtils.Decode16u( data, offset );
			if ( signature != NetworkConfig.HANDSHAKE_SIGNATURE )
				return false;

			offset += NetworkConfig.SIZE_OF_SIGNATURE;
			size -= NetworkConfig.SIZE_OF_SIGNATURE;
			return true;
		}

		private bool VerifyPeerId( byte[] data, ref int offset, ref int size, ref ushort id )
		{
			if ( size < NetworkConfig.SIZE_OF_PEER_ID )
				return false;

			ByteUtils.Decode16u( data, offset, ref id );
			offset += NetworkConfig.SIZE_OF_PEER_ID;
			size -= NetworkConfig.SIZE_OF_PEER_ID;
			return true;
		}

		private bool IsPong( byte[] data, int size )
		{
			if ( size < NetworkConfig.SIZE_OF_SIGNATURE )
				return false;

			ushort signature = 0;
			ByteUtils.Decode16u( data, 0, ref signature );
			if ( signature != NetworkConfig.PONG_SIGNATURE )
				return false;

			return true;
		}

		private void ProcessReceiveDatas()
		{
			this._receivedDatas.Switch();
			while ( !this._receivedDatas.isEmpty )
			{
				ReceivedData receivedData = this._receivedDatas.Pop();
				byte[] data = receivedData.data;
				int offset = 0;
				int size = data.Length;

				if ( !this.VerifyConnKey( data, ref offset, ref size ) )
					continue;

				switch ( this._state )
				{
					case State.Connecting:
						if ( !this.VerifyHandshakeAck( data, ref offset, ref size ) )
							continue;

						ushort peerId = 0;
						if ( !this.VerifyPeerId( data, ref offset, ref size, ref peerId ) )
							continue;
						this.id = peerId;
						this._kcpProxy = new KCPProxy( this.id, this.OnKCPOutput, ( log, user ) => /*Logger.Net( $"KCP log:{log}" )*/ { } );
						this._state = State.Connected;
						this._pingScheduler.Start( NetworkConfig.PING_INTERVAL, this.SendPing );
						this._activeTime = TimeUtils.utcTime;
						this.OnSocketEvent?.Invoke( new SocketEvent( SocketEvent.Type.Connect, string.Empty, SocketError.Success, null ) );
						break;

					case State.Connected:
						this._activeTime = TimeUtils.utcTime;
						this._kcpProxy.ProcessData( data, offset, size, this.OnKCPRecv );
						break;
				}
			}
		}

		private int OnKCPOutput( byte[] buf, int size, uint user )
		{
			byte[] sdata = new byte[size + NetworkConfig.SIZE_OF_CONN_KEY + NetworkConfig.SIZE_OF_PEER_ID];
			int offset = ByteUtils.Encode32u( sdata, 0, NetworkConfig.CONN_KEY );
			offset += ByteUtils.Encode16u( sdata, offset, this.id );
			Buffer.BlockCopy( buf, 0, sdata, offset, size );
			offset += size;
			return this.SendDirect( sdata, offset );
		}

		private void OnKCPRecv( byte[] data, int size )
		{
			if ( this.IsPong( data, size ) )
				return;

			Packet packet = NetworkHelper.DecodePacket( data, 0, size );
			packet.OnReceive();
			this._rpcManager.Invoke( packet );
			this.OnSocketEvent?.Invoke( new SocketEvent( SocketEvent.Type.Receive, packet, null ) );
		}

		public void Update( long dt )
		{
			this._updateContext.deltaTime = dt;
			this._updateContext.time += dt;

			this.ProcessReceiveDatas();

			switch ( this._state )
			{
				case State.Connected:
					this._kcpProxy.Update( this._updateContext );
					this._pingScheduler.Update( dt );
					if ( TimeUtils.utcTime >= this._activeTime + NetworkConfig.PING_TIME_OUT )
						this.MarkToClose( "Heartbeat timeout", SocketError.TimedOut );
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