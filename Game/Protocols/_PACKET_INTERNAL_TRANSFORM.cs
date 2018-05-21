﻿using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	[Packet( 0, 32005 )]
	public class _PACKET_INTERNAL_TRANSFORM : Packet
	{
		public _DTO_transform dto;

		public _PACKET_INTERNAL_TRANSFORM() : base( 0, 32005 )
		{
		}

		public _PACKET_INTERNAL_TRANSFORM( _DTO_transform dto ) : base( 0, 32005 )
		{
			this.dto = dto;
		}

		public _PACKET_INTERNAL_TRANSFORM( float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z ) : base( 0, 32005 )
		{
			this.dto = new _DTO_transform( position_x,position_y,position_z,rotation_x,rotation_y,rotation_z );
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );
			this.dto.Serialize( buffer );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );
			this.dto = new _DTO_transform();
			this.dto.Deserialize( buffer );
		}

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}, dto:{this.dto}";
		}
	}
}