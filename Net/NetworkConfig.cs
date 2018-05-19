namespace RC.Net
{
	public static class NetworkConfig
	{
		//外部协议第一个字节从0x64开始,应避免第一个字节与外部协议冲突
		public const ushort HANDSHAKE_SIGNATURE = 0x6200;
		public const ushort PING_SIGNATURE = 0x6201;
		public const ushort PONG_SIGNATURE = 0x6202;

		public const int SIZE_OF_SIGNATURE = sizeof( ushort );
		public const int SIZE_OF_CONN_KEY = sizeof( uint );
		public const int SIZE_OF_PEER_ID = sizeof( ushort );

		public const uint CONN_KEY = 0x11223344;
		public const byte INTERNAL_MODULE = 0x63;//外部协议模块id从100开始

		public static int KCP_NO_DELAY = 1;
		public static int KCP_INTERVAL = 10;
		public static int KCP_RESEND = 2;
		public static int KCP_NC = 1;
		public static int KCP_SND_WIN = 128;
		public static int KCP_REV_WIN = 128;
		public static int KCP_MTU = 512;

		public static bool USE_KCP_NATIVE;
		public static int BUFFER_SIZE = 512;
		public static int CONNECTION_TIMEOUT = 5000;
		public static int HANDSHAKE_INTERVAL = 1000;
		public static int PING_INTERVAL = 5000;
		public static int PING_TIME_OUT = 15000;
	}
}