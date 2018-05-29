using RC.Game.Core;
using RC.Game.Logic;
using RC.Game.Logic.Components;
using RC.Game.Protocol.CG;
using RC.Net;

namespace UnitTest.lockstep
{
	public class LocalBattle
	{
		private const string NETWORK_NAME = "client";

		private Battle _lBattle;
		private INetServer _transmitter;

		public LocalBattle( NetworkManager.PType protocolType, string ip, int port )
		{
			NetworkManager.CreateClient( NETWORK_NAME, protocolType );
			NetworkManager.AddClientEventHandler( NETWORK_NAME, this.ProcessClientEvent );
			NetworkManager.Connect( NETWORK_NAME, ip, port );
		}

		public void Close()
		{
			NetworkManager.CloseClient( NETWORK_NAME );
		}

		private void ProcessClientEvent( SocketEvent e )
		{
			switch ( e.type )
			{
				case SocketEvent.Type.Connect:
					this.InitBattle();
					break;

				case SocketEvent.Type.Receive:
					if ( e.packet.module == CGModule.BATTLE && e.packet.command == CGCommand.GC_FRAME )
					{
						this._lBattle?.ProcessServerKeyFrame( ( ( _PACKET_BATTLE_GC_FRAME )e.packet ).dto );
					}
					break;

				case SocketEvent.Type.Close:
					break;
			}
		}

		private void InitBattle()
		{
			this._transmitter = new NativeTransmitter();
			this._transmitter.OnSocketEvent += this.OnTransmitterEvent;

			this._lBattle = new Battle( 50, 5, this._transmitter );
			Entity entity = this._lBattle.entityManager.Create<Entity>();
			Transform transform = entity.AddComponent<Transform>();
		}

		private void OnTransmitterEvent( SocketEvent e )
		{
			switch ( e.type )
			{
				case SocketEvent.Type.Receive:
					//Logger.Log( e.packet );
					break;
			}
		}

		public void Update( long dt )
		{
			this._lBattle?.Update( dt );
		}
	}
}