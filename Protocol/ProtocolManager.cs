// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
using RC.Net;
using RC.Net.Protocol;
using System;
using System.Collections.Generic;

namespace RC.Game.Protocol
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
{ 20, typeof( _DTO_frame_info ) },
{ 21, typeof( _DTO_action_info ) },
{ 22, typeof( _DTO_keyframe ) },
{ 30, typeof( _DTO_transform ) },
		};

		private static readonly Dictionary<int, Type> PACKET_MAP = new Dictionary<int, Type>
		{
			{ EncodeID( 100, 0 ), typeof( _PACKET_LV_BATTLE_KEYFRAME ) },
{ EncodeID( 100, 1 ), typeof( _PACKET_LV_BATTLE_CREATE ) },
{ EncodeID( 100, 2 ), typeof( _PACKET_LV_BATTLE_DESTROY ) },
{ EncodeID( 100, 3 ), typeof( _PACKET_LV_BATTLE_ENTITY_AWAKE ) },
{ EncodeID( 100, 4 ), typeof( _PACKET_LV_BATTLE_ENTITY_START ) },
{ EncodeID( 100, 5 ), typeof( _PACKET_LV_BATTLE_ENTITY_DESTROY ) },
{ EncodeID( 100, 6 ), typeof( _PACKET_LV_BATTLE_TRANSFORM ) },
{ EncodeID( 101, 1000 ), typeof( _PACKET_BATTLE_SC_FRAME ) },
{ EncodeID( 255, 0 ), typeof( _PACKET_TEST_CS_RPC ) },
{ EncodeID( 255, 1000 ), typeof( _PACKET_TEST_SC_RPC ) },
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
public static _DTO_keyframe DTO_keyframe(  )
		{
			return new _DTO_keyframe(  );
		}
public static _DTO_keyframe DTO_keyframe( int frame )
		{
			return new _DTO_keyframe( frame );
		}
public static _DTO_transform DTO_transform(  )
		{
			return new _DTO_transform(  );
		}
public static _DTO_transform DTO_transform( float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z )
		{
			return new _DTO_transform( position_x,position_y,position_z,rotation_x,rotation_y,rotation_z );
		}

		public static _PACKET_LV_BATTLE_KEYFRAME PACKET_LV_BATTLE_KEYFRAME( _DTO_keyframe dto )
		{
			return new _PACKET_LV_BATTLE_KEYFRAME( dto );
		}
public static _PACKET_LV_BATTLE_ENTITY_AWAKE PACKET_LV_BATTLE_ENTITY_AWAKE( _DTO_ulong dto )
		{
			return new _PACKET_LV_BATTLE_ENTITY_AWAKE( dto );
		}
public static _PACKET_LV_BATTLE_ENTITY_START PACKET_LV_BATTLE_ENTITY_START( _DTO_ulong dto )
		{
			return new _PACKET_LV_BATTLE_ENTITY_START( dto );
		}
public static _PACKET_LV_BATTLE_ENTITY_DESTROY PACKET_LV_BATTLE_ENTITY_DESTROY( _DTO_ulong dto )
		{
			return new _PACKET_LV_BATTLE_ENTITY_DESTROY( dto );
		}
public static _PACKET_LV_BATTLE_TRANSFORM PACKET_LV_BATTLE_TRANSFORM( _DTO_transform dto )
		{
			return new _PACKET_LV_BATTLE_TRANSFORM( dto );
		}
public static _PACKET_BATTLE_SC_FRAME PACKET_BATTLE_SC_FRAME( _DTO_frame_info dto )
		{
			return new _PACKET_BATTLE_SC_FRAME( dto );
		}
public static _PACKET_TEST_CS_RPC PACKET_TEST_CS_RPC( _DTO_string dto )
		{
			return new _PACKET_TEST_CS_RPC( dto );
		}
public static _PACKET_TEST_SC_RPC PACKET_TEST_SC_RPC( _DTO_string dto )
		{
			return new _PACKET_TEST_SC_RPC( dto );
		}
		public static _PACKET_LV_BATTLE_KEYFRAME PACKET_LV_BATTLE_KEYFRAME(  )
		{
			return new _PACKET_LV_BATTLE_KEYFRAME(  );
		}
public static _PACKET_LV_BATTLE_KEYFRAME PACKET_LV_BATTLE_KEYFRAME( int frame )
		{
			return new _PACKET_LV_BATTLE_KEYFRAME( frame );
		}
public static _PACKET_LV_BATTLE_CREATE PACKET_LV_BATTLE_CREATE(  )
		{
			return new _PACKET_LV_BATTLE_CREATE(  );
		}
public static _PACKET_LV_BATTLE_DESTROY PACKET_LV_BATTLE_DESTROY(  )
		{
			return new _PACKET_LV_BATTLE_DESTROY(  );
		}
public static _PACKET_LV_BATTLE_ENTITY_AWAKE PACKET_LV_BATTLE_ENTITY_AWAKE(  )
		{
			return new _PACKET_LV_BATTLE_ENTITY_AWAKE(  );
		}
public static _PACKET_LV_BATTLE_ENTITY_AWAKE PACKET_LV_BATTLE_ENTITY_AWAKE( ulong value )
		{
			return new _PACKET_LV_BATTLE_ENTITY_AWAKE( value );
		}
public static _PACKET_LV_BATTLE_ENTITY_START PACKET_LV_BATTLE_ENTITY_START(  )
		{
			return new _PACKET_LV_BATTLE_ENTITY_START(  );
		}
public static _PACKET_LV_BATTLE_ENTITY_START PACKET_LV_BATTLE_ENTITY_START( ulong value )
		{
			return new _PACKET_LV_BATTLE_ENTITY_START( value );
		}
public static _PACKET_LV_BATTLE_ENTITY_DESTROY PACKET_LV_BATTLE_ENTITY_DESTROY(  )
		{
			return new _PACKET_LV_BATTLE_ENTITY_DESTROY(  );
		}
public static _PACKET_LV_BATTLE_ENTITY_DESTROY PACKET_LV_BATTLE_ENTITY_DESTROY( ulong value )
		{
			return new _PACKET_LV_BATTLE_ENTITY_DESTROY( value );
		}
public static _PACKET_LV_BATTLE_TRANSFORM PACKET_LV_BATTLE_TRANSFORM(  )
		{
			return new _PACKET_LV_BATTLE_TRANSFORM(  );
		}
public static _PACKET_LV_BATTLE_TRANSFORM PACKET_LV_BATTLE_TRANSFORM( float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z )
		{
			return new _PACKET_LV_BATTLE_TRANSFORM( position_x,position_y,position_z,rotation_x,rotation_y,rotation_z );
		}
public static _PACKET_BATTLE_SC_FRAME PACKET_BATTLE_SC_FRAME(  )
		{
			return new _PACKET_BATTLE_SC_FRAME(  );
		}
public static _PACKET_BATTLE_SC_FRAME PACKET_BATTLE_SC_FRAME( _DTO_action_info[] actions,int frameId )
		{
			return new _PACKET_BATTLE_SC_FRAME( actions,frameId );
		}
public static _PACKET_TEST_CS_RPC PACKET_TEST_CS_RPC(  )
		{
			return new _PACKET_TEST_CS_RPC(  );
		}
public static _PACKET_TEST_CS_RPC PACKET_TEST_CS_RPC( string value )
		{
			return new _PACKET_TEST_CS_RPC( value );
		}
public static _PACKET_TEST_SC_RPC PACKET_TEST_SC_RPC(  )
		{
			return new _PACKET_TEST_SC_RPC(  );
		}
public static _PACKET_TEST_SC_RPC PACKET_TEST_SC_RPC( string value )
		{
			return new _PACKET_TEST_SC_RPC( value );
		}
		public static void CALL_LV_BATTLE_KEYFRAME( this INetClient transmitter, _DTO_keyframe dto, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_KEYFRAME( dto ),callback );
		}
public static void CALL_LV_BATTLE_ENTITY_AWAKE( this INetClient transmitter, _DTO_ulong dto, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_AWAKE( dto ),callback );
		}
public static void CALL_LV_BATTLE_ENTITY_START( this INetClient transmitter, _DTO_ulong dto, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_START( dto ),callback );
		}
public static void CALL_LV_BATTLE_ENTITY_DESTROY( this INetClient transmitter, _DTO_ulong dto, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_DESTROY( dto ),callback );
		}
public static void CALL_LV_BATTLE_TRANSFORM( this INetClient transmitter, _DTO_transform dto, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_TRANSFORM( dto ),callback );
		}
public static void CALL_BATTLE_SC_FRAME( this INetClient transmitter, _DTO_frame_info dto, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_BATTLE_SC_FRAME( dto ),callback );
		}
public static void CALL_TEST_CS_RPC( this INetClient transmitter, _DTO_string dto, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_TEST_CS_RPC( dto ),callback );
		}
public static void CALL_TEST_SC_RPC( this INetClient transmitter, _DTO_string dto, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_TEST_SC_RPC( dto ),callback );
		}
		public static void CALL_LV_BATTLE_KEYFRAME( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_KEYFRAME(  ), callback );
		}
public static void CALL_LV_BATTLE_KEYFRAME( this INetClient transmitter, int frame, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_KEYFRAME( frame ), callback );
		}
public static void CALL_LV_BATTLE_CREATE( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_CREATE(  ), callback );
		}
public static void CALL_LV_BATTLE_DESTROY( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_DESTROY(  ), callback );
		}
public static void CALL_LV_BATTLE_ENTITY_AWAKE( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_AWAKE(  ), callback );
		}
public static void CALL_LV_BATTLE_ENTITY_AWAKE( this INetClient transmitter, ulong value, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_AWAKE( value ), callback );
		}
public static void CALL_LV_BATTLE_ENTITY_START( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_START(  ), callback );
		}
public static void CALL_LV_BATTLE_ENTITY_START( this INetClient transmitter, ulong value, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_START( value ), callback );
		}
public static void CALL_LV_BATTLE_ENTITY_DESTROY( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_DESTROY(  ), callback );
		}
public static void CALL_LV_BATTLE_ENTITY_DESTROY( this INetClient transmitter, ulong value, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_ENTITY_DESTROY( value ), callback );
		}
public static void CALL_LV_BATTLE_TRANSFORM( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_TRANSFORM(  ), callback );
		}
public static void CALL_LV_BATTLE_TRANSFORM( this INetClient transmitter, float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_LV_BATTLE_TRANSFORM( position_x,position_y,position_z,rotation_x,rotation_y,rotation_z ), callback );
		}
public static void CALL_BATTLE_SC_FRAME( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_BATTLE_SC_FRAME(  ), callback );
		}
public static void CALL_BATTLE_SC_FRAME( this INetClient transmitter, _DTO_action_info[] actions,int frameId, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_BATTLE_SC_FRAME( actions,frameId ), callback );
		}
public static void CALL_TEST_CS_RPC( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_TEST_CS_RPC(  ), callback );
		}
public static void CALL_TEST_CS_RPC( this INetClient transmitter, string value, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_TEST_CS_RPC( value ), callback );
		}
public static void CALL_TEST_SC_RPC( this INetClient transmitter, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_TEST_SC_RPC(  ), callback );
		}
public static void CALL_TEST_SC_RPC( this INetClient transmitter, string value, RPCHandler callback = null )
		{
			transmitter.Send( new _PACKET_TEST_SC_RPC( value ), callback );
		}
	}
}
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global