namespace RC.Core.Misc
{
	public static class Logger
	{
		public delegate void LogHandler( object obj );

		private static LogHandler _logAction;
		private static LogHandler _debugAction;
		private static LogHandler _netAction;
		private static LogHandler _infoAction;
		private static LogHandler _warnAction;
		private static LogHandler _errorAction;
		private static LogHandler _factalAction;

		public static void SetActions( LogHandler logAction, LogHandler debugAction, LogHandler netAction,
									   LogHandler infoAction, LogHandler warnAction, LogHandler errorAction,
									   LogHandler factalAction )
		{
			_logAction = logAction;
			_debugAction = debugAction;
			_netAction = netAction;
			_infoAction = infoAction;
			_warnAction = warnAction;
			_errorAction = errorAction;
			_factalAction = factalAction;
		}

		public static void Log( object obj )
		{
			_logAction?.Invoke( obj );
		}

		public static void Debug( object obj )
		{
			_debugAction?.Invoke( obj );
		}

		public static void Net( object obj )
		{
			_netAction?.Invoke( obj );
		}

		public static void Info( object obj )
		{
			_infoAction?.Invoke( obj );
		}

		public static void Warn( object obj )
		{
			_warnAction?.Invoke( obj );
		}

		public static void Error( object obj )
		{
			_errorAction?.Invoke( obj );
		}

		public static void Fatal( object obj )
		{
			_factalAction?.Invoke( obj );
		}
	}
}