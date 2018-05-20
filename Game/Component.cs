using System;
using System.Collections.Generic;
using System.Reflection;

namespace RC.Game
{
	public abstract class Component
	{
		public static void Destroy( Component component )
		{
			component.markToDestroy = true;
		}

		public ulong typeID { get; private set; }
		public Entity owner { get; internal set; }
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

		internal void Awake()
		{
			this.typeID = BitConverter.ToUInt64( this.GetType().GUID.ToByteArray(), 0 );
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

		internal void NotifyComponentAdded( Component component )
		{
			this.OnNotifyComponentAdded( component );
		}

		internal void NotifyComponentDestroied( Component component )
		{
			this.OnNotifyComponentDestroied( component );
		}

		internal void Update( UpdateContext context )
		{
			this.OnUpdate( context );
		}

		internal void Synchronize()
		{
			this.OnSynchronize();
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

		protected virtual void OnSynchronize()
		{
		}

		protected virtual void OnNotifyComponentAdded( Component component )
		{
		}

		protected virtual void OnNotifyComponentDestroied( Component component )
		{
		}

		internal SynchronizeAttribute[] GetSyncProps()
		{
			List<SynchronizeAttribute> attributes = new List<SynchronizeAttribute>();
			Type type = this.GetType();
			PropertyInfo[] properties = type.GetProperties();
			int count = properties.Length;
			for ( int i = 0; i < count; i++ )
			{
				PropertyInfo propertyInfo = properties[i];
				SynchronizeAttribute attribute = propertyInfo.GetCustomAttribute<SynchronizeAttribute>( true );
				if ( attribute == null )
					continue;
				attribute.owner = propertyInfo;
				attributes.Add( attribute );
			}
			if ( attributes.Count == 0 )
				return null;
			return attributes.ToArray();
		}
	}
}