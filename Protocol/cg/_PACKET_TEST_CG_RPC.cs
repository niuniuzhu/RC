using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol.CG
{
	[Packet( 255, 0 )]
	public class _PACKET_TEST_CG_RPC : Packet
	{
		public _DTO_string dto;

		public _PACKET_TEST_CG_RPC() : base( 255, 0, true )
		{
		}

		public _PACKET_TEST_CG_RPC( _DTO_string dto ) : base( 255, 0, true )
		{
			this.dto = dto;
		}

		public _PACKET_TEST_CG_RPC( string value ) : base( 255, 0, true )
		{
			this.dto = new _DTO_string( value );
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );
			this.dto.Serialize( buffer );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );
			this.dto = new _DTO_string();
			this.dto.Deserialize( buffer );
		}
		
		public _PACKET_TEST_GC_RPC Reply(  )
		{
			_PACKET_TEST_GC_RPC packet = new _PACKET_TEST_GC_RPC(  );
			packet.srcPid = this.pid;
			packet.isRPCReturn = true;
			return packet;
		}
public _PACKET_TEST_GC_RPC Reply( string value )
		{
			_PACKET_TEST_GC_RPC packet = new _PACKET_TEST_GC_RPC( value );
			packet.srcPid = this.pid;
			packet.isRPCReturn = true;
			return packet;
		}

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}, dto:{this.dto}";
		}
	}
}