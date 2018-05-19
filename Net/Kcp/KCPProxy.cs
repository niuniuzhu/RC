namespace RC.Net.Kcp
{
	public class KCPProxy
	{
		public delegate void KCPDataOutputHandler( byte[] data, int size );

		private readonly IKCP _kcp;
		private bool _kcpUpdateFlag;
		private uint _nextKcpUpdateTime;

		internal void Dispose()
		{
			this._kcp.Dispose();
		}

		internal void Clear()
		{
			this._kcp.Clear();
		}

		internal KCPProxy( ushort id, KCP.OutputHandler output, KCP.LoggerHandler writelog )
		{
			if ( NetworkConfig.USE_KCP_NATIVE )
			{
				KCPNative kcpNative;
				this._kcp = kcpNative = new KCPNative( NetworkConfig.CONN_KEY, id, output, writelog );
				kcpNative.SetLogMask( int.MaxValue );
			}
			else
				this._kcp = new KCP( NetworkConfig.CONN_KEY, id, output, writelog );
			this._kcp.NoDelay( NetworkConfig.KCP_NO_DELAY, NetworkConfig.KCP_INTERVAL, NetworkConfig.KCP_RESEND,
							   NetworkConfig.KCP_NC );
			this._kcp.WndSize( NetworkConfig.KCP_SND_WIN, NetworkConfig.KCP_REV_WIN );
			this._kcp.SetMtu( NetworkConfig.KCP_MTU );
		}

		internal void Send( byte[] data, int offset, int size )
		{
			this._kcpUpdateFlag = true;
			this._kcp.Send( data, offset, size );
		}

		internal void ProcessData( byte[] data, int offset, int size, KCPDataOutputHandler kcpDataOutputHandler )
		{
			int ret = this._kcp.Input( data, offset, size );

			if ( ret < 0 )
				return;

			this._kcpUpdateFlag = true;

			int peekSize;
			while ( ( peekSize = this._kcp.PeekSize() ) > 0 )
			{
				byte[] buffer = new byte[peekSize];
				int recv = this._kcp.Recv( buffer, peekSize );
				if ( recv > 0 )
					kcpDataOutputHandler( buffer, peekSize );
			}
		}

		internal void Update( NetworkUpdateContext context )
		{
			uint ct = ( uint )context.time;
			if ( this._kcpUpdateFlag || ct >= this._nextKcpUpdateTime )
			{
				this._kcp.Update( ct );
				this._nextKcpUpdateTime = this._kcp.Check( ct );
				this._kcpUpdateFlag = false;
			}
		}
	}
}