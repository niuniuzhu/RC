namespace RC.Game
{
	public abstract class Entity : GPoolObject
	{
		public Battle battle { get; internal set; }

		internal bool markToDestroy { get; private set; }

		protected override void InternalDispose()
		{
		}

		public void MarkToDestroy()
		{
			this.markToDestroy = true;
		}

		internal void OnAddedToBattle()
		{
			this.InternalOnAddedToBattle();
			this.OnSyncState();
		}

		internal void OnRemoveFromBattle()
		{
			this.InternalOnRemoveFromBattle();
			this.markToDestroy = false;
			this.battle = null;
		}

		protected virtual void InternalOnAddedToBattle()
		{
		}

		protected virtual void InternalOnRemoveFromBattle()
		{
		}

		protected virtual void OnPositionChanged()
		{
		}

		protected virtual void OnDirectionChanged()
		{
		}

		public virtual void OnGenericUpdate( UpdateContext context )
		{
		}

		internal virtual void OnUpdateState( UpdateContext context )
		{
		}

		internal void OnSyncState()
		{
		}
	}
}