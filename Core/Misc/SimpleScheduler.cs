namespace RC.Core.Misc
{
	public class SimpleScheduler
	{
		public delegate void Handler();

		private long _interval;
		private Handler _handler;
		private long _currTime;

		public void Start( long interval, Handler handler )
		{
			this._interval = interval;
			this._handler = handler;
			this._currTime = 0;
		}

		public void Stop()
		{
			this._handler = null;
		}

		public void Update( long dt )
		{
			this._currTime += dt;
			if ( this._currTime >= this._interval )
			{
				this._handler?.Invoke();
				this._currTime = 0;
			}
		}
	}
}