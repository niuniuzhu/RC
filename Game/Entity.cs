namespace RC.Game
{
	public abstract class Entity : ComponentHolder
	{
		public static void Destroy( Entity entity )
		{
			entity.markToDestroy = true;
			foreach ( Component component in entity )
				Component.Destroy( component );
		}

		public ulong rid { get; internal set; }
		public Battle battle { get; internal set; }
		public bool destroied { get; private set; }
		internal bool markToDestroy { get; private set; }

		internal void Awake()
		{
			this.OnAwake();
		}

		internal void Destroy()
		{

			this.OnDestroy();
			this.markToDestroy = false;
			this.destroied = true;
		}

		internal override void Update( UpdateContext context )
		{
			this.OnUpdate( context );
			base.Update( context );
		}

		internal void UpdateState( UpdateContext context )
		{
			this.OnUpdateState( context );
		}

		internal void Synchronize()
		{
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnUpdate( UpdateContext context )
		{
		}

		protected virtual void OnUpdateState( UpdateContext context )
		{
		}
	}
}