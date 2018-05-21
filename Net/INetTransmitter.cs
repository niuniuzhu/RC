using RC.Net.Protocol;

namespace RC.Net
{
	public interface INetTransmitter
	{
		void Send( Packet packet );
	}
}