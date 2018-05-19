using RC.Core.FMath;

namespace RC.Game
{
	public sealed class UpdateContext
	{
		public Fix64 deltaTime;

		public Fix64 time;

		public int frame;
	}
}