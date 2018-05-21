namespace RC.Game.Protocols
{
	public static class Module
	{
		public const byte BATTLE = 0;
	}

	public static class Command
	{
		public const ushort SC_KEYFRAME = 0;
public const ushort SC_CREATE = 1;
public const ushort SC_DESTROY = 2;
public const ushort SC_ENTITY_AWAKE = 3;
public const ushort SC_ENTITY_START = 4;
public const ushort SC_ENTITY_DESTROY = 5;
public const ushort SC_TRANSFORM = 6;
public const ushort SC_FRAME = 1000;
	}
}