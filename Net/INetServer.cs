using System.Collections.Generic;
using RC.Net.Protocol;

namespace RC.Net
{
	public interface INetServer: ISocketEventHolder
	{
		void Send( ushort tokenId, Packet packet );
		void Send( IEnumerable<ushort> tokenIds, Packet packet );
		void SendAll( Packet packet );
		void Dispose();
		void Stop();
		void Start( int port );
		void Update( long dt );
	}
}