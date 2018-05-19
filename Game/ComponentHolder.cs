using System;
using System.Collections;
using System.Collections.Generic;

namespace RC.Game
{
	public class ComponentHolder : IEnumerable<Component>
	{
		private readonly Dictionary<Type, Component> _typeToComps = new Dictionary<Type, Component>();
		private readonly List<Component> _comps = new List<Component>();

		public Component GetComponent<T>() where T : Component
		{
			return !this._typeToComps.TryGetValue( typeof( T ), out Component comp ) ? default( T ) : comp;
		}

		public T AddComponent<T>() where T : Component, new()
		{
			if ( this._typeToComps.ContainsKey( typeof( T ) ) )
				return default( T );
			T component = new T();
			this._typeToComps[typeof( T )] = component;
			component.Awake();
			return component;
		}

		internal virtual void Update( UpdateContext context )
		{
			int count = this._comps.Count;
			for ( int i = 0; i < count; i++ )
				this._comps[i].Update( context );
			this.DestroyComponents();
		}

		private void DestroyComponents()
		{
			int count = this._comps.Count;
			for ( int i = 0; i < count; i++ )
			{
				Component com = this._comps[i];
				if ( !com.markToDestroy )
					continue;

				com.Destroy();
				com.owner = null;

				this._comps.RemoveAt( i );
				this._typeToComps.Remove( com.GetType() );
				--i;
				--count;
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
	}
}