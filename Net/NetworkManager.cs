using System;
using System.Collections.Generic;
using System.Reflection;
using RC.Core.Misc;
using RC.Net.Protocol;

namespace RC.Net
{
	public delegate void SocketEventHandler( SocketEvent socketEvent );

	public static class NetworkManager
	{
		#region packet support
		private static readonly Dictionary<int, Type> PACKET_MAP = new Dictionary<int, Type>();

		public static void AddPacketTypes()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach ( Assembly assembly in assemblies )
			{
				Type[] types = assembly.GetTypes();
				foreach ( Type type in types )
				{
					PacketAttribute attribute = type.GetCustomAttribute<PacketAttribute>();
					if ( attribute != null )
						AddPacketType( attribute.id, type );
				}
			}
		}

		private static void AddPacketType( int id, Type type )
		{
			PACKET_MAP.Add( id, type );
		}

		internal static Type GetPacketType( byte module, ushort command )
		{
			return !PACKET_MAP.TryGetValue( NetworkHelper.EncodePacketID( module, command ), out Type type ) ? null : type;
		}
		#endregion

		public enum PType
		{
			Tcp,
			Kcp
		}

		private static readonly Dictionary<string, INetServer> SERVERS = new Dictionary<string, INetServer>();
		private static readonly Dictionary<string, INetClient> CLIENTS = new Dictionary<string, INetClient>();
		private static bool _involveKCP;
		public static void SetupKCP()
		{
			_involveKCP = true;
			NetworkConfig.BUFFER_SIZE = 512;
			NetworkConfig.KCP_NO_DELAY = 1;
			NetworkConfig.KCP_INTERVAL = 20;
			NetworkConfig.KCP_RESEND = 2;
			NetworkConfig.KCP_NC = 1;
			NetworkConfig.KCP_SND_WIN = 128;
			NetworkConfig.KCP_REV_WIN = 128;
			// IPv4最大传输单元是65535,不能超过这个数值
			// 虽然以太网的链路层最大传输单元为1500字节,但最好能减少分片数量,宁愿增加一些带宽浪费,换来更好的低延迟
			NetworkConfig.KCP_MTU = 512;
			NetworkConfig.PING_INTERVAL = 5000;
			NetworkConfig.PING_TIME_OUT = 15000;
#if KCP_NATIVE
			NetworkConfig.USE_KCP_NATIVE = true;
			Kcp.KCPInterface.InitMemoryPool();
#else
			Core.Net.Kcp.KCP.InitMemoryPool( 8 * NetworkConfig.BUFFER_SIZE, 50 );
#endif
		}

		public static void Dispose()
		{
			foreach ( KeyValuePair<string, INetServer> kv in SERVERS )
				kv.Value.Dispose();

			foreach ( KeyValuePair<string, INetClient> kv in CLIENTS )
				kv.Value.Dispose();

#if KCP_NATIVE
			if ( _involveKCP )
				Kcp.KCPInterface.ikcp_release_allocator();
#endif
		}

		public static void Update( long deltaTime )
		{
			foreach ( KeyValuePair<string, INetServer> kv in SERVERS )
				kv.Value.Update( deltaTime );

			foreach ( KeyValuePair<string, INetClient> kv in CLIENTS )
				kv.Value.Update( deltaTime );
		}

		#region server

		public static void CreateServer( string name, PType type, int maxClient )
		{
			if ( SERVERS.ContainsKey( name ) )
			{
				Logger.Warn( $"Server [{name}] already exist" );
				return;
			}
			INetServer server = null;
			switch ( type )
			{
				case PType.Tcp:
					server = new TCPServer( maxClient );
					break;

				case PType.Kcp:
					server = new KCPServer( maxClient );
					break;
			}
			SERVERS[name] = server;
		}

		public static void DestroyServer( string name )
		{
			INetServer server = GetServer( name );
			if ( server == null )
				return;
			server.Dispose();
			SERVERS.Remove( name );
		}

		private static INetServer GetServer( string name )
		{
			if ( !SERVERS.TryGetValue( name, out INetServer server ) )
				return null;
			return server;
		}

		public static void AddServerEventHandler( string serverName, SocketEventHandler socketEventHandler )
		{
			INetServer server = GetServer( serverName );
			if ( server != null )
				server.OnSocketEvent += socketEventHandler;
		}

		public static void StopServer( string serverName )
		{
			GetServer( serverName )?.Stop();
		}

		public static void StartServer( string serverName, int port )
		{
			GetServer( serverName )?.Start( port );
		}

		public static void Send( string serverName, ushort tokenId, Packet packet )
		{
			GetServer( serverName )?.Send( tokenId, packet );
		}

		public static void Send( string serverName, IEnumerable<ushort> tokenIds, Packet packet )
		{
			GetServer( serverName )?.Send( tokenIds, packet );
		}
		#endregion

		#region client
		public static void CreateClient( string name, PType type )
		{
			if ( CLIENTS.ContainsKey( name ) )
			{
				Logger.Warn( $"Client [{name}] already exist" );
				return;
			}
			INetClient client = null;
			switch ( type )
			{
				case PType.Tcp:
					client = new TCPClient();
					break;

				case PType.Kcp:
					client = new KCPClient();
					break;
			}
			CLIENTS[name] = client;
		}

		public static void DestroyClient( string name )
		{
			INetClient client = GetClient( name );
			if ( client == null )
				return;
			client.Dispose();
			CLIENTS.Remove( name );
		}

		private static INetClient GetClient( string name )
		{
			if ( !CLIENTS.TryGetValue( name, out INetClient client ) )
				return null;
			return client;
		}

		public static void AddClientEventHandler( string clientName, SocketEventHandler socketEventHandler )
		{
			INetClient client = GetClient( clientName );
			if ( client != null )
				client.OnSocketEvent += socketEventHandler;
		}

		public static void CloseClient( string clientName )
		{
			GetClient( clientName )?.Close();
		}

		public static void Connect( string clientName, string ip, int port )
		{
			GetClient( clientName )?.Connect( ip, port );
		}

		public static void Send( string clientName, Packet packet )
		{
			GetClient( clientName )?.Send( packet );
		}
		#endregion
	}
}