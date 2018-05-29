// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
using RC.Net;
using RC.Net.Protocol;
using System;
using System.Collections.Generic;

namespace RC.Game.Protocol.CG
{
	public static class CGProtoMgr
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
{ 100, typeof( _DTO_frame_info ) },
{ 101, typeof( _DTO_action_info ) },
		};

		private static readonly Dictionary<int, Type> PACKET_MAP = new Dictionary<int, Type>
		{
			{ EncodeID( 100, 1000 ), typeof( _PACKET_BATTLE_GC_FRAME ) },
{ EncodeID( 255, 0 ), typeof( _PACKET_TEST_CG_RPC ) },
{ EncodeID( 255, 1000 ), typeof( _PACKET_TEST_GC_RPC ) },
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
public static _DTO_frame_info DTO_frame_info(  )
		{
			return new _DTO_frame_info(  );
		}
public static _DTO_frame_info DTO_frame_info( _DTO_action_info[] actions,int frameId )
		{
			return new _DTO_frame_info( actions,frameId );
		}
public static _DTO_action_info DTO_action_info(  )
		{
			return new _DTO_action_info(  );
		}
public static _DTO_action_info DTO_action_info( string sender,byte type,float x,float y,float z,string target )
		{
			return new _DTO_action_info( sender,type,x,y,z,target );
		}
public static _DTO_action_info DTO_action_info( string sender,byte type,float x,float y,float z )
		{
			return new _DTO_action_info( sender,type,x,y,z );
		}
public static _DTO_action_info DTO_action_info( string sender,byte type,string target )
		{
			return new _DTO_action_info( sender,type,target );
		}

		public static _PACKET_BATTLE_GC_FRAME _PACKET_BATTLE_GC_FRAME( _DTO_frame_info dto )
		{
			return new _PACKET_BATTLE_GC_FRAME( dto );
		}
public static _PACKET_TEST_CG_RPC _PACKET_TEST_CG_RPC( _DTO_string dto )
		{
			return new _PACKET_TEST_CG_RPC( dto );
		}
public static _PACKET_TEST_GC_RPC _PACKET_TEST_GC_RPC( _DTO_string dto )
		{
			return new _PACKET_TEST_GC_RPC( dto );
		}
		public static _PACKET_BATTLE_GC_FRAME _PACKET_BATTLE_GC_FRAME(  )
		{
			return new _PACKET_BATTLE_GC_FRAME(  );
		}
public static _PACKET_BATTLE_GC_FRAME _PACKET_BATTLE_GC_FRAME( _DTO_action_info[] actions,int frameId )
		{
			return new _PACKET_BATTLE_GC_FRAME( actions,frameId );
		}
public static _PACKET_TEST_CG_RPC _PACKET_TEST_CG_RPC(  )
		{
			return new _PACKET_TEST_CG_RPC(  );
		}
public static _PACKET_TEST_CG_RPC _PACKET_TEST_CG_RPC( string value )
		{
			return new _PACKET_TEST_CG_RPC( value );
		}
public static _PACKET_TEST_GC_RPC _PACKET_TEST_GC_RPC(  )
		{
			return new _PACKET_TEST_GC_RPC(  );
		}
public static _PACKET_TEST_GC_RPC _PACKET_TEST_GC_RPC( string value )
		{
			return new _PACKET_TEST_GC_RPC( value );
		}
	}
}
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global