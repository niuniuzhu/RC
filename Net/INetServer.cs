using System.Collections.Generic;
using RC.Net.Protocol;

namespace RC.Net
{
	public interface INetServer
	{
		event SocketEventHandler OnSocketEvent;
		void Send( ushort tokenId, Packet packet );
		void Send( IEnumerable<ushort> tokenIds, Packet packet );
		void Dispose();
		void Stop();
		void Start( int port );
		void Update( long dt );
	}
}