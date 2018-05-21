using RC.Core.FMath;
using RC.Net;

namespace RC.Game
{
	public sealed class Battle
	{
		public int frame { get; private set; }
		public Fix64 deltaTime { get; private set; }
		public Fix64 time { get; private set; }

		public INetTransmitter transmitter { get; set; }

		private readonly UpdateContext _context;
		private readonly EntityManager _entityManager;

		public EntityManager entityManager => this._entityManager;

		public Battle()
		{
			this._context = new UpdateContext();
			this._entityManager = new EntityManager( this );
		}

		public void Dispose()
		{
			this._entityManager.Dispose();
		}

		public void Update( Fix64 dt )
		{
			++this.frame;

			this.deltaTime = dt;
			this.time += this.deltaTime;

			this._context.deltaTime = this.deltaTime;
			this._context.time = this.time;
			this._context.frame = this.frame;

			this._entityManager.Update( this._context );
		}
	}
}