using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using RC.Core.Misc;
using RC.Core.Structure;
using RC.Net.Protocol;

namespace RC.Net
{
	public sealed class TCPServer : INetServer
	{
		public event SocketEventHandler OnSocketEvent;

		private readonly int _maxClient;
		private Socket _socket;
		private SocketEvent? _closeEvent;
		private readonly SwitchQueue<ReceivedData> _receivedDatas = new SwitchQueue<ReceivedData>();
		private readonly NetworkUpdateContext _updateContext = new NetworkUpdateContext();
		private readonly UserTokenManager<TCPUserToken> _tokenManager = new UserTokenManager<TCPUserToken>();
		private readonly ServerRPCManager _rpcManager = new ServerRPCManager();

		internal TCPServer( int maxClient )
		{
			this._maxClient = maxClient;
		}

		public void Dispose()
		{
			this.Stop();
			this._tokenManager.Dispose();
		}

		public void Send( ushort tokenId, Packet packet, RPCHandler callback )
		{
			TCPUserToken token = this._tokenManager.Get( tokenId );
			this._rpcManager.Maped( token, packet, callback );
			packet.OnSend();
			token?.Send( packet );
		}

		public void Send( IEnumerable<ushort> tokenIds, Packet packet )
		{
			packet.OnSend();
			byte[] data = NetworkHelper.EncodePacket( packet );
			foreach ( ushort tokenId in tokenIds )
			{
				TCPUserToken token = this._tokenManager.Get( tokenId );
				token?.Send( data );
			}
		}

		public void SendAll( Packet packet )
		{
			packet.OnSend();
			foreach ( TCPUserToken token in this._tokenManager )
				token.Send( packet );
		}

		public void Stop()
		{
			Socket socket = this._socket;
			lock ( this._receivedDatas )
			{
				if ( this._socket == null )
					return;
				this._socket = null;
				this._receivedDatas.Clear();

				while ( this._tokenManager.count > 0 )
				{
					TCPUserToken token = this._tokenManager[0];
					this.OnSocketEvent?.Invoke( new SocketEvent( SocketEvent.Type.Disconnect, "Server stoped", SocketError.Shutdown, token ) );
					token.OnDespawn();
					this._rpcManager.OnUserTokenDespawn( token );
					this._tokenManager.Destroy( 0 );
				}
			}

			if ( socket.Connected )
				socket.Shutdown( SocketShutdown.Both );
			socket.Close();
		}

		private void MarkToStop( string msg, SocketError errorCode )
		{
			if ( this._closeEvent != null )
				return;
			this._closeEvent = new SocketEvent( SocketEvent.Type.Close, msg, errorCode, null );
		}

		public void Start( int port )
		{
			this.Stop();
			this._socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			this._socket.NoDelay = true;
			this._socket.Bind( new IPEndPoint( IPAddress.Any, port ) );
			this._socket.Listen( 10 );
			this.StartAccept( null );
		}

		private void StartAccept( SocketAsyncEventArgs acceptEventArgs )
		{
			if ( this._socket == null )
				return;

			if ( acceptEventArgs == null )
			{
				acceptEventArgs = new SocketAsyncEventArgs();
				acceptEventArgs.RemoteEndPoint = new IPEndPoint( IPAddress.Any, 0 );
				acceptEventArgs.Completed += this.OnAcceptComplete;
			}
			else
				acceptEventArgs.AcceptSocket = null;

			bool asyncResult;
			try
			{
				asyncResult = this._socket.AcceptAsync( acceptEventArgs );
			}
			catch ( ObjectDisposedException )
			{
				return;
			}
			catch ( SocketException e )
			{
				this.MarkToStop( e.ToString(), e.SocketErrorCode );
				return;
			}
			if ( !asyncResult )
				this.ProcessAccept( acceptEventArgs );
		}

		private void OnAcceptComplete( object sender, SocketAsyncEventArgs acceptEventArgs )
		{
			this.ProcessAccept( acceptEventArgs );
		}

		private void ProcessAccept( SocketAsyncEventArgs acceptEventArgs )
		{
			if ( acceptEventArgs.SocketError == SocketError.Success )
			{
				lock ( this._receivedDatas )
					this._receivedDatas.Push( new ReceivedData( acceptEventArgs.AcceptSocket ) );
			}
			this.StartAccept( acceptEventArgs );
		}

		private void StartReceive( TCPUserToken token )
		{
			bool asyncResult;
			try
			{
				asyncResult = token.ReceiveAsync( token.receiveEventArgs );
			}
			catch ( ObjectDisposedException )
			{
				return;
			}
			catch ( SocketException e )
			{
				token.MarkToDisconnect( $"Receive error:{e}, remote endpoint: {token.remoteEndPoint}", e.SocketErrorCode );
				return;
			}
			if ( !asyncResult )
				this.ProcessReceive( token.receiveEventArgs );
		}

		internal void ProcessReceive( SocketAsyncEventArgs receiveEventArgs )
		{
			TCPUserToken token = ( TCPUserToken )receiveEventArgs.UserToken;
			if ( receiveEventArgs.SocketError != SocketError.Success )
			{
				token.MarkToDisconnect( $"Receive error, remote endpoint: {token.remoteEndPoint}",
									receiveEventArgs.SocketError );
				return;
			}
			int size = receiveEventArgs.BytesTransferred;
			if ( size == 0 )
			{
				token.MarkToDisconnect( $"Receive zero bytes, remote endpoint: {token.remoteEndPoint}", SocketError.NoData );
				return;
			}
			lock ( this._receivedDatas )
			{
				if ( this._socket != null )
				{
					token.CacheData( receiveEventArgs.Buffer, receiveEventArgs.Offset, receiveEventArgs.BytesTransferred );
					this._receivedDatas.Push( new ReceivedData( token ) );
				}
			}
			this.StartReceive( token );
		}

		private void CheckClientOverRange()
		{
			int over = this._tokenManager.count - this._maxClient;
			for ( int i = 0; i < over; i++ )
			{
				TCPUserToken token = this._tokenManager[this._tokenManager.count - 1];
				token.MarkToDisconnect( $"Client overrange, remote endpoint: {token.remoteEndPoint}",
									SocketError.SocketError );
			}
		}

		private void ProcessReceiveDatas()
		{
			this._receivedDatas.Switch();
			while ( !this._receivedDatas.isEmpty )
			{
				ReceivedData receivedData = this._receivedDatas.Pop();
				switch ( receivedData.type )
				{
					case ReceivedData.Type.Accept:
						{
							TCPUserToken newToken = this._tokenManager.Create();
							newToken.OnSpawn( this, receivedData.conn, TimeUtils.utcTime );
							this.OnSocketEvent?.Invoke( new SocketEvent( SocketEvent.Type.Accept,
																		 $"Client connection accepted, remote endpoint: {receivedData.conn.RemoteEndPoint}",
																		 SocketError.Success, newToken ) );
							this.StartReceive( newToken );
							newToken.Send( new PacketAccept( newToken.id ) );
						}
						break;

					case ReceivedData.Type.Receive:
						{
							TCPUserToken token = ( TCPUserToken )receivedData.token;
							token.ProcessData( packet =>
							{
								if ( packet.module == NetworkConfig.INTERNAL_MODULE && packet.command == 0 )
									token.Send( new PacketHeartBeat( ( ( PacketHeartBeat )packet ).localTime ) );
								else
								{
									this._rpcManager.Invoke( token, packet );
									this.OnSocketEvent?.Invoke( new SocketEvent( SocketEvent.Type.Receive, packet, token ) );
								}
							} );
						}
						break;
				}
			}
		}

		public void Update( long dt )
		{
			this._updateContext.deltaTime = dt;
			this._updateContext.time += dt;

			this.ProcessReceiveDatas();
			this.CheckClientOverRange();

			int count = this._tokenManager.count;
			for ( int i = 0; i < count; i++ )
			{
				TCPUserToken token = this._tokenManager[i];
				if ( token.disconnectEvent == null )
					continue;
				this.OnSocketEvent?.Invoke( token.disconnectEvent.Value );
				token.OnDespawn();
				this._rpcManager.OnUserTokenDespawn( token );
				this._tokenManager.Destroy( i );
				--i;
				--count;
			}
			if ( this._closeEvent != null )
			{
				this.OnSocketEvent?.Invoke( this._closeEvent.Value );
				this._closeEvent = null;
				this.Stop();
			}
		}
	}
}