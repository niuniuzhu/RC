namespace RC.Game.Protocol
{
	public static class Module
	{
		public const byte LV_BATTLE = 100;
public const byte BATTLE = 101;
public const byte TEST = 255;
	}

	public static class Command
	{
		public const ushort KEYFRAME = 0;
public const ushort CREATE = 1;
public const ushort DESTROY = 2;
public const ushort ENTITY_AWAKE = 3;
public const ushort ENTITY_START = 4;
public const ushort ENTITY_DESTROY = 5;
public const ushort TRANSFORM = 6;
public const ushort SC_FRAME = 1000;
public const ushort CS_RPC = 0;
public const ushort SC_RPC = 1000;
	}

}