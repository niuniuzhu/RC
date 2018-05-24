using RC.Net;
using System.Threading;

namespace Example.lockstep
{
	public class Lockstep : Base
	{
		private readonly RemoteBattle _remoteBattle;
		private readonly LocalBattle _localBattle;

		public Lockstep( NetworkManager.PType protocolType, string ip, int port )
		{
			this._remoteBattle = new RemoteBattle( protocolType, port );
			Thread.Sleep( 100 );
			this._localBattle = new LocalBattle( protocolType, ip, port );

			this.StartLoopCycle( 1 );
		}

		protected override void OnUpdate( long dt )
		{
			this._localBattle.Update( dt );
			NetworkManager.Update( dt );
		}

		protected override void OnInput( string cmd )
		{
			switch( cmd )
			{
				case "cstop":
					this._localBattle.Close();
					break;

				case "sstop":
					this._remoteBattle.Stop();
					break;
			}
		}
	}
}