// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
using RC.Net;
using RC.Net.Protocol;
using System;
using System.Collections.Generic;

namespace RC.Game.Protocols
{
	public static class ProtocolManager
	{
		private static readonly Dictionary<ushort, Type> DTO_MAP = new Dictionary<ushort, Type>
		{
			{ 0, typeof( _DTO_byte ) },
{ 1, typeof( _DTO_bool ) },
{ 2, typeof( _DTO_short ) },
{ 3, typeof( _DTO_ushort ) },
{ 4, typeof( _DTO_int ) },
{ 5, typeof( _DTO_uint ) },
{ 6, typeof( _DTO_float ) },
{ 7, typeof( _DTO_long ) },
{ 8, typeof( _DTO_ulong ) },
{ 9, typeof( _DTO_double ) },
{ 10, typeof( _DTO_string ) },
{ 11, typeof( _DTO_transform ) },
		};

		private static readonly Dictionary<int, Type> PACKET_MAP = new Dictionary<int, Type>
		{
			{ EncodeID( 0, 32000 ), typeof( _PACKET_INTERNAL_BATTLE_CREATE ) },
{ EncodeID( 0, 32001 ), typeof( _PACKET_INTERNAL_BATTLE_DESTROY ) },
{ EncodeID( 0, 32002 ), typeof( _PACKET_INTERNAL_ENTITY_AWAKE ) },
{ EncodeID( 0, 32003 ), typeof( _PACKET_INTERNAL_ENTITY_START ) },
{ EncodeID( 0, 32004 ), typeof( _PACKET_INTERNAL_ENTITY_DESTROY ) },
{ EncodeID( 0, 32005 ), typeof( _PACKET_INTERNAL_TRANSFORM ) },
		};
		
		public static Type GetDTOType( ushort dtoId )
		{
			DTO_MAP.TryGetValue( dtoId, out Type type );
			return type;
		}
		
		public static Type GetPacketType( byte module, ushort command )
		{
			PACKET_MAP.TryGetValue( EncodeID( module, command ), out Type type );
			return type;
		}

		public static int EncodeID( byte moduleId, ushort cmd )
		{
			return ( moduleId << 16 ) | cmd;
		}

		public static _DTO_byte DTO_byte(  )
		{
			return new _DTO_byte(  );
		}
public static _DTO_byte DTO_byte( byte value )
		{
			return new _DTO_byte( value );
		}
public static _DTO_bool DTO_bool(  )
		{
			return new _DTO_bool(  );
		}
public static _DTO_bool DTO_bool( bool value )
		{
			return new _DTO_bool( value );
		}
public static _DTO_short DTO_short(  )
		{
			return new _DTO_short(  );
		}
public static _DTO_short DTO_short( short value )
		{
			return new _DTO_short( value );
		}
public static _DTO_ushort DTO_ushort(  )
		{
			return new _DTO_ushort(  );
		}
public static _DTO_ushort DTO_ushort( ushort value )
		{
			return new _DTO_ushort( value );
		}
public static _DTO_int DTO_int(  )
		{
			return new _DTO_int(  );
		}
public static _DTO_int DTO_int( int value )
		{
			return new _DTO_int( value );
		}
public static _DTO_uint DTO_uint(  )
		{
			return new _DTO_uint(  );
		}
public static _DTO_uint DTO_uint( uint value )
		{
			return new _DTO_uint( value );
		}
public static _DTO_float DTO_float(  )
		{
			return new _DTO_float(  );
		}
public static _DTO_float DTO_float( float value )
		{
			return new _DTO_float( value );
		}
public static _DTO_long DTO_long(  )
		{
			return new _DTO_long(  );
		}
public static _DTO_long DTO_long( long value )
		{
			return new _DTO_long( value );
		}
public static _DTO_ulong DTO_ulong(  )
		{
			return new _DTO_ulong(  );
		}
public static _DTO_ulong DTO_ulong( ulong value )
		{
			return new _DTO_ulong( value );
		}
public static _DTO_double DTO_double(  )
		{
			return new _DTO_double(  );
		}
public static _DTO_double DTO_double( double value )
		{
			return new _DTO_double( value );
		}
public static _DTO_string DTO_string(  )
		{
			return new _DTO_string(  );
		}
public static _DTO_string DTO_string( string value )
		{
			return new _DTO_string( value );
		}
public static _DTO_transform DTO_transform(  )
		{
			return new _DTO_transform(  );
		}
public static _DTO_transform DTO_transform( float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z )
		{
			return new _DTO_transform( position_x,position_y,position_z,rotation_x,rotation_y,rotation_z );
		}

		public static _PACKET_INTERNAL_ENTITY_AWAKE PACKET_INTERNAL_ENTITY_AWAKE( _DTO_ulong dto )
		{
			return new _PACKET_INTERNAL_ENTITY_AWAKE( dto );
		}
public static _PACKET_INTERNAL_ENTITY_START PACKET_INTERNAL_ENTITY_START( _DTO_ulong dto )
		{
			return new _PACKET_INTERNAL_ENTITY_START( dto );
		}
public static _PACKET_INTERNAL_ENTITY_DESTROY PACKET_INTERNAL_ENTITY_DESTROY( _DTO_ulong dto )
		{
			return new _PACKET_INTERNAL_ENTITY_DESTROY( dto );
		}
public static _PACKET_INTERNAL_TRANSFORM PACKET_INTERNAL_TRANSFORM( _DTO_transform dto )
		{
			return new _PACKET_INTERNAL_TRANSFORM( dto );
		}
		public static _PACKET_INTERNAL_BATTLE_CREATE PACKET_INTERNAL_BATTLE_CREATE(  )
		{
			return new _PACKET_INTERNAL_BATTLE_CREATE(  );
		}
public static _PACKET_INTERNAL_BATTLE_DESTROY PACKET_INTERNAL_BATTLE_DESTROY(  )
		{
			return new _PACKET_INTERNAL_BATTLE_DESTROY(  );
		}
public static _PACKET_INTERNAL_ENTITY_AWAKE PACKET_INTERNAL_ENTITY_AWAKE(  )
		{
			return new _PACKET_INTERNAL_ENTITY_AWAKE(  );
		}
public static _PACKET_INTERNAL_ENTITY_AWAKE PACKET_INTERNAL_ENTITY_AWAKE( ulong value )
		{
			return new _PACKET_INTERNAL_ENTITY_AWAKE( value );
		}
public static _PACKET_INTERNAL_ENTITY_START PACKET_INTERNAL_ENTITY_START(  )
		{
			return new _PACKET_INTERNAL_ENTITY_START(  );
		}
public static _PACKET_INTERNAL_ENTITY_START PACKET_INTERNAL_ENTITY_START( ulong value )
		{
			return new _PACKET_INTERNAL_ENTITY_START( value );
		}
public static _PACKET_INTERNAL_ENTITY_DESTROY PACKET_INTERNAL_ENTITY_DESTROY(  )
		{
			return new _PACKET_INTERNAL_ENTITY_DESTROY(  );
		}
public static _PACKET_INTERNAL_ENTITY_DESTROY PACKET_INTERNAL_ENTITY_DESTROY( ulong value )
		{
			return new _PACKET_INTERNAL_ENTITY_DESTROY( value );
		}
public static _PACKET_INTERNAL_TRANSFORM PACKET_INTERNAL_TRANSFORM(  )
		{
			return new _PACKET_INTERNAL_TRANSFORM(  );
		}
public static _PACKET_INTERNAL_TRANSFORM PACKET_INTERNAL_TRANSFORM( float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z )
		{
			return new _PACKET_INTERNAL_TRANSFORM( position_x,position_y,position_z,rotation_x,rotation_y,rotation_z );
		}
		public static void CALL_INTERNAL_ENTITY_AWAKE( this INetTransmitter transmitter, _DTO_ulong dto )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_AWAKE( dto ) );
		}
public static void CALL_INTERNAL_ENTITY_START( this INetTransmitter transmitter, _DTO_ulong dto )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_START( dto ) );
		}
public static void CALL_INTERNAL_ENTITY_DESTROY( this INetTransmitter transmitter, _DTO_ulong dto )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_DESTROY( dto ) );
		}
public static void CALL_INTERNAL_TRANSFORM( this INetTransmitter transmitter, _DTO_transform dto )
		{
			transmitter.Send( new _PACKET_INTERNAL_TRANSFORM( dto ) );
		}
		public static void CALL_INTERNAL_BATTLE_CREATE( this INetTransmitter transmitter )
		{
			transmitter.Send( new _PACKET_INTERNAL_BATTLE_CREATE(  ) );
		}
public static void CALL_INTERNAL_BATTLE_DESTROY( this INetTransmitter transmitter )
		{
			transmitter.Send( new _PACKET_INTERNAL_BATTLE_DESTROY(  ) );
		}
public static void CALL_INTERNAL_ENTITY_AWAKE( this INetTransmitter transmitter )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_AWAKE(  ) );
		}
public static void CALL_INTERNAL_ENTITY_AWAKE( this INetTransmitter transmitter, ulong value )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_AWAKE( value ) );
		}
public static void CALL_INTERNAL_ENTITY_START( this INetTransmitter transmitter )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_START(  ) );
		}
public static void CALL_INTERNAL_ENTITY_START( this INetTransmitter transmitter, ulong value )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_START( value ) );
		}
public static void CALL_INTERNAL_ENTITY_DESTROY( this INetTransmitter transmitter )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_DESTROY(  ) );
		}
public static void CALL_INTERNAL_ENTITY_DESTROY( this INetTransmitter transmitter, ulong value )
		{
			transmitter.Send( new _PACKET_INTERNAL_ENTITY_DESTROY( value ) );
		}
public static void CALL_INTERNAL_TRANSFORM( this INetTransmitter transmitter )
		{
			transmitter.Send( new _PACKET_INTERNAL_TRANSFORM(  ) );
		}
public static void CALL_INTERNAL_TRANSFORM( this INetTransmitter transmitter, float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z )
		{
			transmitter.Send( new _PACKET_INTERNAL_TRANSFORM( position_x,position_y,position_z,rotation_x,rotation_y,rotation_z ) );
		}
	}
}
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global