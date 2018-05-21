using RC.Net.Protocol;

namespace RC.Net
{
	public interface IUserToken
	{
		ushort id { get; }
		void Send( Packet packet );
	}
}