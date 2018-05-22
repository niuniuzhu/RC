using RC.Net;
using System.Threading;

namespace Example
{
	public class Test : Base
	{
		private RemoteBattle _remoteBattle;
		private LocalBattle _localBattle;

		public Test( NetworkManager.PType protocolType, string ip, int port )
		{
			this._remoteBattle = new RemoteBattle( protocolType, port );

			Thread.Sleep( 100 );

			this._localBattle = new LocalBattle( protocolType, ip, port );
		}

		public void Update( long deltaTime )
		{
			this._localBattle.Update( deltaTime );
			NetworkManager.Update( deltaTime );
		}
	}
}