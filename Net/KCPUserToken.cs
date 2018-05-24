using RC.Core.Misc;
using RC.Net.Kcp;
using RC.Net.Protocol;
using System;
using System.Net;
using System.Net.Sockets;

namespace RC.Net
{
	public sealed class KCPUserToken : IUserToken
	{
		public delegate void ProcessDataOutputHandler( Packet packet );

		public ushort id { get; private set; }
		public SocketEvent? disconnectEvent { get; private set; }

		private EndPoint _remoteEndPoint;
		private KCPServer _server;
		private readonly KCPProxy _kcpProxy;
		private bool _active;
		private long _activeTime;

		internal KCPUserToken( ushort id )
		{
			this.id = id;
			this._kcpProxy = new KCPProxy( this.id, this.OnKCPOutout, ( log, user ) => /* Logger.Net( $"KCP log:{log}" )*/ { } );
		}

		public void Dispose()
		{
			this._kcpProxy.Dispose();
		}

		public void OnSpawn( KCPServer server, EndPoint remoteEndPoint, long connectTime )
		{
			this._server = server;
			this._active = true;
			this._remoteEndPoint = remoteEndPoint;
			this._activeTime = connectTime;
		}

		public void OnDespawn()
		{
			if ( !this._active )
				return;
			this._active = false;
			this.disconnectEvent = null;
			this._kcpProxy.Clear();
			this._server = null;
		}

		public void MarkToDisconnect( string msg, SocketError errorCode )
		{
			if ( this.disconnectEvent != null )
				return;
			this.disconnectEvent = new SocketEvent( SocketEvent.Type.Disconnect, msg, errorCode, this );
		}

		internal void ProcessData( byte[] data, int offset, int size, ProcessDataOutputHandler outputHandler )
		{
			if ( !this._active )
				return;

			this._kcpProxy.ProcessData( data, offset, size, ( outData, outSize ) =>
			{
				Packet packet = NetworkHelper.DecodePacket( outData, 0, outSize );
				packet.OnReceive();
				outputHandler( packet );

			} );

			this._activeTime = TimeUtils.utcTime;
		}

		private int OnKCPOutout( byte[] buf, int size, uint user )
		{
			byte[] sdata = new byte[size + NetworkConfig.SIZE_OF_CONN_KEY];
			int offset = ByteUtils.Encode32u( sdata, 0, NetworkConfig.CONN_KEY );
			Buffer.BlockCopy( buf, 0, sdata, offset, size );
			offset += size;
			this.SendDirect( sdata, 0, offset );
			return 0;
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
			this._kcpProxy.Send( data, offset, size );
		}

		internal void SendDirect( byte[] data, int offset, int size )
		{
			if ( !this._active )
				return;
			this._server.SendTo( this, data, offset, size, this._remoteEndPoint );
		}

		public void CheckAlive()
		{
			if ( TimeUtils.utcTime < this._activeTime + NetworkConfig.PING_TIME_OUT )
				return;
			this.MarkToDisconnect( $"Heartbeat timeout, remote Address: {this._remoteEndPoint}", SocketError.TimedOut );
		}

		public void Update( NetworkUpdateContext context )
		{
			this._kcpProxy.Update( context );
		}
	}
}