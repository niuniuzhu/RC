using RC.Core.Misc;
using RC.Net;
using System.Net.Sockets;
using System.Threading;
using RC.Game.Protocol.CG;

namespace UnitTest.NetworkTest
{
	public class NetworkTest
	{
		private const string SERVER_NAME = "server";
		private const string CLIENT_NAME = "client";

		public NetworkTest()
		{
			NetworkManager.AddPacketTypes();
			NetworkManager.CreateServer( SERVER_NAME, NetworkManager.PType.Tcp, 10 );
			NetworkManager.AddServerEventHandler( SERVER_NAME, this.ProcessServerEvent );
			NetworkManager.StartServer( SERVER_NAME, 2551 );

			NetworkManager.CreateClient( CLIENT_NAME, NetworkManager.PType.Tcp );
			NetworkManager.AddClientEventHandler( CLIENT_NAME, this.ProcessClientEvent );
			NetworkManager.Connect( CLIENT_NAME, "127.0.0.1", 2551 );

			int i = 300;
			while ( i > 0 )
			{
				NetworkManager.Update( 10 );
				Thread.Sleep( 10 );
				--i;
			}
			NetworkManager.Dispose();
		}

		private void ProcessClientEvent( SocketEvent e )
		{
			switch ( e.type )
			{
				case SocketEvent.Type.Connect:
					// make RPC calls
					for ( int i = 0; i < 10; i++ )
						NetworkManager.Send( CLIENT_NAME, CGProtoMgr._PACKET_TEST_CG_RPC( "test" ),
											 ( token, packet ) =>
											 {
												 _PACKET_TEST_GC_RPC p = ( _PACKET_TEST_GC_RPC )packet;
												 Logger.Log( p.dto.value );
											 } );
					break;
			}
		}

		private void ProcessServerEvent( SocketEvent e )
		{
			switch ( e.type )
			{
				case SocketEvent.Type.Accept:
					if ( e.errorCode == SocketError.Success )
					{
						Logger.Log( $"有客户端连接了, code:{e.errorCode}, msg:{e.msg}" );
					}
					else
						Logger.Log( $"Socket error, type:{e.type}, code:{e.errorCode}, msg:{e.msg}" );

					break;

				case SocketEvent.Type.Disconnect:
					Logger.Log( $"有客户端断开连接了, code:{e.errorCode}, msg:{e.msg}" );
					break;

				case SocketEvent.Type.Receive:
					//Logger.Log( $"Received: {e.packet}" );
					if ( e.packet.module == CGModule.TEST &&
						 e.packet.command == CGCommand.CG_RPC )
					{
						_PACKET_TEST_CG_RPC packet = ( _PACKET_TEST_CG_RPC )e.packet;
						NetworkManager.Send( SERVER_NAME, e.userToken.id, packet.Reply( "return" ) );
					}
					break;

				default:
					if ( e.errorCode != SocketError.Success )
						Logger.Log( $"Socket error, type:{e.type}, code:{e.errorCode}, msg:{e.msg}" );
					break;
			}
		}
	}
}