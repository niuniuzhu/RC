using RC.Net;

namespace Example
{
	static class Program
	{
		static void Main( string[] args )
		{
			Lockstep lockstep = new Lockstep( NetworkManager.PType.Tcp, "127.0.0.1", 2551 );
		}
	}
}
