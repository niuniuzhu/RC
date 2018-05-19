namespace RC.Net
{
	public interface INetClient : INetTransmitter
	{
		event SocketEventHandler OnSocketEvent;
		ushort id { get; }
		void Dispose();
		void Close();
		void Connect( string ip, int port );
		void Update( long dt );
	}
}