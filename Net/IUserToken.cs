using RC.Net.Protocol;

namespace RC.Net
{
	public interface IUserToken
	{
		ushort id { get; }
		void Dispose();
		void Send( Packet packet );
	}
}