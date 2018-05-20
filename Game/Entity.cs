using System.Collections;
using System.Collections.Generic;

namespace RC.Game
{
	public abstract class Entity : IEnumerable<Component>
	{
		public static void Destroy( Entity entity )
		{
			entity.markToDestroy = true;
		}

		public ulong rid { get; internal set; }
		public Battle battle { get; internal set; }
		public bool destroied { get; private set; }
		internal bool markToDestroy { get; private set; }

		private readonly ComponentManager _componentManager;

		protected Entity()
		{
			this._componentManager = new ComponentManager( this );
		}

		public T AddComponent<T>() where T : Component, new()
		{
			return this._componentManager.AddComponent<T>();
		}

		public Component GetComponent<T>() where T : Component
		{
			return this._componentManager.GetComponent<T>();
		}

		internal void Awake()
		{
			this.OnAwake();
		}

		internal void Destroy()
		{
			this.OnDestroy();
			foreach ( Component component in this )
				Component.Destroy( component );
			this._componentManager.DestroyComponents();
			this.markToDestroy = false;
			this.destroied = true;
		}

		internal void Update( UpdateContext context )
		{
			this.OnUpdate( context );
			this._componentManager.Update( context );
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

		public IEnumerator<Component> GetEnumerator()
		{
			return this._componentManager.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._componentManager.GetEnumerator();
		}
	}
}