using RC.Net;
using System.Diagnostics;
using System.Threading;

namespace Example
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

			Stopwatch sw = new Stopwatch();
			sw.Start();
			long realCost = 0;
			long lastElapsed = 0;
			while ( true )
			{
				this.Update( realCost );
				Thread.Sleep( 1 );
				long elapsed = sw.ElapsedMilliseconds;
				realCost = elapsed - lastElapsed;
				lastElapsed = elapsed;
			}
		}

		public void Update( long deltaTime )
		{
			this._localBattle.Update( deltaTime );
			NetworkManager.Update( deltaTime );
		}
	}
}