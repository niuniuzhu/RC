using System.Collections.Generic;
using RC.Core.Misc;

namespace RC.Game
{
	public class EntityManager
	{
		public int entityCount => this._entities.Count;

		private readonly GPool _gPool = new GPool();
		private readonly List<Entity> _entities = new List<Entity>();
		private readonly Dictionary<ulong, Entity> _idToEntity = new Dictionary<ulong, Entity>();

		private Battle _battle;

		public EntityManager( Battle battle )
		{
			this._battle = battle;
		}

		public void Dispose()
		{
			int count = this._entities.Count;
			for ( int i = 0; i < count; i++ )
				this._entities[i].MarkToDestroy();
			this.DestroyEnties();
			this._gPool.Dispose();
			this._battle = null;
		}

		public T Create<T>() where T : Entity, new()
		{
			T entity = this._gPool.Pop<T>();
			entity.rid = GuidHash.GetUInt64();
			entity.battle = this._battle;
			this._idToEntity[entity.rid] = entity;
			this._entities.Add( entity );
			entity.OnAddedToBattle();
			return entity;
		}

		public Entity GetEntity( ulong id )
		{
			this._idToEntity.TryGetValue( id, out Entity entity );
			return entity;
		}

		private void DestroyEnties()
		{
			int count = this._entities.Count;
			for ( int i = 0; i < count; i++ )
			{
				Entity entity = this._entities[i];
				if ( !entity.markToDestroy )
					continue;

				entity.OnRemoveFromBattle();

				this._entities.RemoveAt( i );
				this._idToEntity.Remove( entity.rid );
				this._gPool.Push( entity );
				--i;
				--count;
			}
		}

		internal void Update( UpdateContext context )
		{
			this.GenericUpdate( context );
			//更新状态
			this.UpdateState( context );
			//发送实体状态
			this.Synchronize();
			//清理实体
			this.DestroyEnties();
		}

		private void GenericUpdate( UpdateContext context )
		{
			int count = this._entities.Count;
			for ( int i = 0; i < count; i++ )
			{
				Entity entity = this._entities[i];
				entity.OnGenericUpdate( context );
			}
		}

		private void UpdateState( UpdateContext context )
		{
			int count = this._entities.Count;
			for ( int i = 0; i < count; i++ )
			{
				Entity entity = this._entities[i];
				entity.OnUpdateState( context );
			}
		}

		private void Synchronize()
		{
			int count = this._entities.Count;
			for ( int i = 0; i < count; i++ )
			{
				Entity entity = this._entities[i];
				entity.OnSyncState();
			}
		}
	}
}