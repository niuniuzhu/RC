using System.Collections.Generic;
using System.Threading;

namespace RC.Net
{
	public sealed class TCPUserTokenPool
	{
		private static int _currId;

		private readonly Queue<TCPUserToken> _pool = new Queue<TCPUserToken>();

		public TCPUserToken Pop( TCPServer server )
		{
			return this._pool.Count == 0 ? new TCPUserToken( server, ( ushort )Interlocked.Increment( ref _currId ) ) : this._pool.Dequeue();
		}

		public void Push( TCPUserToken token )
		{
			this._pool.Enqueue( token );
		}

		public void Dispose()
		{
			while ( this._pool.Count > 0 )
			{
				this._pool.Dequeue().Dispose();
			}
		}
	}

	public sealed class KCPUserTokenPool
	{
		private static int _currId;

		private readonly Queue<KCPUserToken> _pool = new Queue<KCPUserToken>();

		public KCPUserToken Pop( KCPServer server )
		{
			return this._pool.Count == 0 ? new KCPUserToken( server, ( ushort )Interlocked.Increment( ref _currId ) ) : this._pool.Dequeue();
		}

		public void Push( KCPUserToken token )
		{
			this._pool.Enqueue( token );
		}

		public void Dispose()
		{
			while ( this._pool.Count > 0 )
			{
				this._pool.Dequeue().Dispose();
			}
		}
	}
}