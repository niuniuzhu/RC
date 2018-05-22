using RC.Game;
using RC.Game.Core;
using System.Diagnostics;
using System.Threading;
using RC.Game.Logic;
using RC.Game.Logic.Components;

namespace Example
{
	public class GameCoreTest : Base
	{
		public GameCoreTest()
		{
			Battle battle = new Battle( 20, 4 );
			Entity entity = battle.entityManager.Create<Entity>();
			Transform transform = entity.AddComponent<Transform>();
			Stopwatch sw = new Stopwatch();
			sw.Start();
			while ( true )
			{
				battle.Update( sw.ElapsedMilliseconds );
				Thread.Sleep( 10 );
			}
		}
	}
}