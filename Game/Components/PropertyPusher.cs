using System.Collections.Generic;

namespace RC.Game.Components
{
	public class PropertyPusher : Component
	{
		private readonly Dictionary<ulong, SynchronizeAttribute[]> _comIDToSyncProps = new Dictionary<ulong, SynchronizeAttribute[]>();

		protected override void OnAwake()
		{
			this.UpdateSyncProps();
		}

		private void AddProps( Component component )
		{
			SynchronizeAttribute[] synchronizeAttributes = component.GetSyncProps();
			if ( synchronizeAttributes == null )
				return;
			this._comIDToSyncProps[component.typeID] = synchronizeAttributes;
		}

		private void RemoveProps( Component component )
		{
			this._comIDToSyncProps.Remove( component.typeID );
		}

		private void UpdateSyncProps()
		{
			foreach ( Component component in this.owner )
				this.AddProps( component );
		}

		protected override void OnNotifyComponentAdded( Component component )
		{
			this.AddProps( component );
		}

		protected override void OnNotifyComponentDestroied( Component component )
		{
			this.RemoveProps( component );
		}

		protected override void OnSynchronize()
		{
			foreach ( KeyValuePair<ulong, SynchronizeAttribute[]> kv in this._comIDToSyncProps )
			{
				//SynchronizeAttribute[] synchronizeAttributes = kv.Value;
				//this.owner.battle.dataTransmitter.Push( BuiltinDataType.SYNCHRONIZE, this.owner.rid, kv.Key,   );
			}
		}
	}
}