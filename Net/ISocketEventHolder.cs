namespace RC.Net
{
	public interface ISocketEventHolder
	{
		event SocketEventHandler OnSocketEvent;
	}
}