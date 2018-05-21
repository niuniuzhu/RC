using RC.Net.Protocol;

namespace RC.Net
{
	public interface INetClient: ISocketEventHolder
	{
		ushort id { get; }
		void Dispose();
		void Close();
		void Connect( string ip, int port );
		void Send( Packet packet );
		void Update( long dt );
	}
}