namespace RC.Game
{
	public abstract class Component
	{
		public ComponentHolder owner { get; internal set; }
		public bool destroied { get; private set; }
		public bool enabled
		{
			get => this._enabled;
			set
			{
				if ( this._enabled == value )
					return;
				this._enabled = value;
				if ( this._enabled )
					this.Enable();
				else
					this.Disable();
			}
		}
		internal bool markToDestroy { get; private set; }

		private bool _enabled;

		public static void Destroy( Component component )
		{
			component.markToDestroy = true;
		}

		internal void Awake()
		{
			this.OnAwake();
			this.enabled = true;
		}

		internal void Destroy()
		{
			this.enabled = false;
			this.OnDestroy();
			this.markToDestroy = false;
			this.destroied = true;
		}

		private void Enable()
		{
			this.OnEnable();
		}

		private void Disable()
		{
			this.OnDisable();
		}

		internal void Update( UpdateContext context )
		{
			this.OnUpdate( context );
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void OnUpdate( UpdateContext context )
		{
		}
	}
}