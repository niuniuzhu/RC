using System;
using RC.Net.Protocol;

namespace RC.Net
{
	public static class NetworkHelper
	{
		internal static byte[] EncodePacket( Packet packet )
		{
			return Serializable.Serialize( packet );
		}

		internal static Packet DecodePacket( byte[] data, int offset, int size )
		{
			byte moduleId = data[0];
			ushort cmd = BitConverter.ToUInt16( data, 1 );
			Type type = NetworkManager.GetPacketType( moduleId, cmd );
			if ( type == null )
				return null;
			Packet packet = ( Packet )Activator.CreateInstance( type, null );
			Serializable.Deserialize( packet, data, offset, size );
			return packet;
		}

		public static int EncodePacketID( byte moduleId, ushort command )
		{
			return ( moduleId << 16 ) | command;
		}
	}
}