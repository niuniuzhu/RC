namespace RC.Game
{
	public abstract class GPoolObject
	{
		public ulong rid { get; internal set; }

		internal void Dispose()
		{
			this.InternalDispose();
		}

		protected abstract void InternalDispose();
	}
}