using System;

namespace RC.Net.Kcp
{
	public delegate byte[] BufferAllocHandler( int size );
	public delegate void BufferFreeHandler( byte[] buffer );

	public interface IKCP : IDisposable
	{
		void Clear();
		int PeekSize();
		int Recv( byte[] buffer, int size );
		int Send( byte[] buffer, int offset, int size );
		int Input( byte[] data, int offset, int size );
		void Update( uint current );
		uint Check( uint current );
		int SetMtu( int mtu );
		int NoDelay( int nodelay, int interval, int resend, int nc );
		int WndSize( int sndwnd, int rcvwnd );
		int WaitSnd();
	}
}