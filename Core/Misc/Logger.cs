namespace RC.Core.Misc
{
	public static class Logger
	{
		public delegate void LogHandler( object obj );

		public static LogHandler logAction;
		public static LogHandler debugAction;
		public static LogHandler netAction;
		public static LogHandler infoAction;
		public static LogHandler warnAction;
		public static LogHandler errorAction;
		public static LogHandler factalAction;

		public static void Log( object obj )
		{
			logAction?.Invoke( obj );
		}

		public static void Debug( object obj )
		{
			debugAction?.Invoke( obj );
		}

		public static void Net( object obj )
		{
			netAction?.Invoke( obj );
		}

		public static void Info( object obj )
		{
			infoAction?.Invoke( obj );
		}

		public static void Warn( object obj )
		{
			warnAction?.Invoke( obj );
		}

		public static void Error( object obj )
		{
			errorAction?.Invoke( obj );
		}

		public static void Fatal( object obj )
		{
			factalAction?.Invoke( obj );
		}
	}
}