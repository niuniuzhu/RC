using RC.Game.Core;
using System.Threading;
using RC.Game.Components;

namespace Example
{
	public class GameCoreTest : Base
	{
		public GameCoreTest()
		{
			Battle battle = new Battle();
			Entity entity = battle.entityManager.Create<Entity>();
			Transform transform = entity.AddComponent<Transform>();
			while ( true )
			{
				battle.Update( 50 );
				Thread.Sleep( 50 );
			}
		}
	}
}