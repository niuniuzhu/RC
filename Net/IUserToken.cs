namespace RC.Net
{
	public interface IUserToken : INetTransmitter
	{
		ushort id { get; }
	}
}