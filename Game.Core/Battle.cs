using RC.Net;

namespace RC.Game.Core
{
	public sealed class Battle
	{
		public int frame { get; private set; }
		public long deltaTime { get; private set; }
		public long time { get; private set; }
		public EntityManager entityManager { get; }
		public INetClient transmitter { get; set; }

		private readonly UpdateContext _context;

		public Battle()
		{
			this._context = new UpdateContext();
			this.entityManager = new EntityManager( this );
		}

		public void Dispose()
		{
			this.entityManager.Dispose();
		}

		public void Update( long dt )
		{
			++this.frame;

			this.deltaTime = dt;
			this.time += this.deltaTime;

			this._context.deltaTime = this.deltaTime;
			this._context.time = this.time;
			this._context.frame = this.frame;

			this.entityManager.Update( this._context );
			this.transmitter?.Update( this.deltaTime );
		}
	}
}