using System;
using System.Collections;
using System.Collections.Generic;

namespace RC.Game
{
	public class ComponentManager : IEnumerable<Component>
	{
		private readonly Dictionary<Type, Component> _typeToComps = new Dictionary<Type, Component>();
		private readonly List<Component> _comps = new List<Component>();
		private readonly Entity _owner;

		public ComponentManager( Entity owner )
		{
			this._owner = owner;
		}

		public T AddComponent<T>() where T : Component, new()
		{
			if ( this._typeToComps.ContainsKey( typeof( T ) ) )
				return default( T );
			T component = new T { owner = this._owner };
			this.NotifyComponentAdded( component );
			this._typeToComps[typeof( T )] = component;
			this._comps.Add( component );
			component.Awake();
			return component;
		}

		public Component GetComponent<T>() where T : Component
		{
			return !this._typeToComps.TryGetValue( typeof( T ), out Component comp ) ? default( T ) : comp;
		}

		internal void Update( UpdateContext context )
		{
			int count = this._comps.Count;
			for ( int i = 0; i < count; i++ )
				this._comps[i].Update( context );
			this.DestroyComponents();
		}

		internal void Synchronize()
		{
			int count = this._comps.Count;
			for ( int i = 0; i < count; i++ )
				this._comps[i].Synchronize();
		}

		internal void DestroyComponents()
		{
			int count = this._comps.Count;
			for ( int i = 0; i < count; i++ )
			{
				Component component = this._comps[i];
				if ( !component.markToDestroy )
					continue;

				component.Destroy();
				component.owner = null;

				this._comps.RemoveAt( i );
				this._typeToComps.Remove( component.GetType() );
				--i;
				--count;
				this.NotifyComponentDestroied( component );
			}
		}

		public IEnumerator<Component> GetEnumerator()
		{
			return this._comps.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._comps.GetEnumerator();
		}

		private void NotifyComponentAdded<T>( T added ) where T : Component, new()
		{
			int count = this._comps.Count;
			for ( int i = 0; i < count; i++ )
				this._comps[i].NotifyComponentAdded( added );
		}

		private void NotifyComponentDestroied( Component destroied )
		{
			int count = this._comps.Count;
			for ( int i = 0; i < count; i++ )
				this._comps[i].NotifyComponentDestroied( destroied );
		}
	}
}