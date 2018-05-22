using RC.Net;
using System.Diagnostics;
using System.Threading;

namespace Example
{
	static class Program
	{
		static void Main( string[] args )
		{
			Test test = new Test( NetworkManager.PType.Tcp, "127.0.0.1", 2551 );

			Stopwatch sw = new Stopwatch();
			sw.Start();
			long realCost = 0;
			long lastElapsed = 0;
			while ( true )
			{
				test.Update( realCost );
				Thread.Sleep( 1 );
				long elapsed = sw.ElapsedMilliseconds;
				realCost = elapsed - lastElapsed;
				lastElapsed = elapsed;
			}
		}
	}
}
