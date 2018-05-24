﻿using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol
{
	[Packet( 100, 5 )]
	public class _PACKET_LV_BATTLE_ENTITY_DESTROY : Packet
	{
		public _DTO_ulong dto;

		public _PACKET_LV_BATTLE_ENTITY_DESTROY() : base( 100, 5, -1 )
		{
		}

		public _PACKET_LV_BATTLE_ENTITY_DESTROY( _DTO_ulong dto ) : base( 100, 5, -1 )
		{
			this.dto = dto;
		}

		public _PACKET_LV_BATTLE_ENTITY_DESTROY( ulong value ) : base( 100, 5, -1 )
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