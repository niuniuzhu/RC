namespace RC.Net.Protocol
{
	public interface ISerializable
	{
		void Serialize( StreamBuffer buffer );
		void Deserialize( StreamBuffer buffer );
	}
}