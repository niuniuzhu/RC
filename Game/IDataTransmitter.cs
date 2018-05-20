namespace RC.Game
{
	public class BuiltinDataType
	{
		public const ushort BATTLE_CREATE = 0;
		public const ushort BATTLE_DESTROY = 1;
		public const ushort ENTITY_AWAKE = 10;
		public const ushort ENTITY_START = 11;
		public const ushort ENTITY_DESTROY = 12;
		public const ushort SYNCHRONIZE = 13;
	}

	public interface IDataTransmitter
	{
		void Push( ushort type, params object[] param );
	}
}