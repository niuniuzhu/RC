using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	[Packet( 0, 32004 )]
	public class _PACKET_INTERNAL_ENTITY_DESTROY : Packet
	{
		public _DTO_ulong dto;

		public _PACKET_INTERNAL_ENTITY_DESTROY() : base( 0, 32004 )
		{
		}

		public _PACKET_INTERNAL_ENTITY_DESTROY( _DTO_ulong dto ) : base( 0, 32004 )
		{
			this.dto = dto;
		}

		public _PACKET_INTERNAL_ENTITY_DESTROY( ulong value ) : base( 0, 32004 )
		{
			this.dto = new _DTO_ulong( value );
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );
			this.dto.Serialize( buffer );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );
			this.dto = new _DTO_ulong();
			this.dto.Deserialize( buffer );
		}

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}, dto:{this.dto}";
		}
	}
}