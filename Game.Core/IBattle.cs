using RC.Net;

namespace RC.Game.Core
{
	public interface IBattle
	{
		int frame { get; }
		long deltaTime { get; }
		long time { get; }
		EntityManager entityManager { get; }
		INetServer transmitter { get; set; }
	}
}