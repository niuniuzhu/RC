using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using RC.Core.Misc;
using RC.Net;
using RC.Net.Protocol;

namespace UnitTest.lockstep
{
	public class RemoteBattle
	{
		private const string NETWORK_NAME = "server";

		private readonly Dictionary<ushort, IUserToken> _idToToken = new Dictionary<ushort, IUserToken>();
		private readonly List<StepLocker> _stepLockers = new List<StepLocker>();

		public RemoteBattle( NetworkManager.PType protocolType, int port )
		{
			NetworkManager.AddPacketTypes();
			NetworkManager.CreateServer( NETWORK_NAME, protocolType, 10 );
			NetworkManager.AddServerEventHandler( NETWORK_NAME, this.ProcessServerEvent );
			NetworkManager.StartServer( NETWORK_NAME, port );
			Logger.Log( $"Server started, listening port: {port}" );
		}

		public void Stop()
		{
			NetworkManager.StopServer( NETWORK_NAME );
		}

		private void ProcessServerEvent( SocketEvent e )
		{
			switch ( e.type )
			{
				case SocketEvent.Type.Accept:
					if ( e.errorCode == SocketError.Success )
					{
						Logger.Log( $"有客户端连接了, code:{e.errorCode}, msg:{e.msg}" );
						this.HandleClientConnected( e.userToken );
					}
					else
						Logger.Log( $"Socket error, type:{e.type}, code:{e.errorCode}, msg:{e.msg}" );

					break;

				case SocketEvent.Type.Disconnect:
					Logger.Log( $"有客户端断开连接了, code:{e.errorCode}, msg:{e.msg}" );
					break;

				case SocketEvent.Type.Receive:
					//Logger.Log( $"Received: {e.packet}" );
					break;

				default:
					if ( e.errorCode != SocketError.Success )
					{
						Logger.Log( $"Socket error, type:{e.type}, code:{e.errorCode}, msg:{e.msg}" );
					}
					break;
			}
		}

		private void HandleClientConnected( IUserToken token )
		{
			this._idToToken[token.id] = token;
			StepLocker stepLocker = new StepLocker( this );
			stepLocker.AddUser( token.id );
			this._stepLockers.Add( stepLocker );

			ThreadPool.QueueUserWorkItem( state =>
			{
				stepLocker.Start();
				while ( !stepLocker.finished )
				{
					stepLocker.Update();
					Thread.Sleep( 10 );
				}
			} );
			Logger.Log( $"创建战场:{stepLocker.id}" );
		}

		private void HandleClientDisconnected( IUserToken token )
		{
			this._idToToken.Remove( token.id );
			StepLocker stepLocker = null;
			foreach ( StepLocker s in this._stepLockers )
			{
				if ( !s.HasUser( token.id ) )
					continue;
				stepLocker = s;
				break;
			}

			if ( stepLocker != null )
			{
				stepLocker.RemoveUser( token.id );
				if ( stepLocker.userCount == 0 )
					this._stepLockers.Remove( stepLocker );
			}
		}

		public void Brocast( List<ushort> users, Packet packet, IUserToken except )
		{
			foreach ( ushort id in users )
			{
				IUserToken token = this._idToToken[id];
				if ( token != except )
				{
					token.Send( packet );
				}
			}
		}
	}
}