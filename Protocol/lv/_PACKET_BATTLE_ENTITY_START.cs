﻿using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol.LV
{
	[Packet( 100, 4 )]
	public class _PACKET_BATTLE_ENTITY_START : Packet
	{
		public _DTO_ulong dto;

		public _PACKET_BATTLE_ENTITY_START() : base( 100, 4, false )
		{
		}

		public _PACKET_BATTLE_ENTITY_START( _DTO_ulong dto ) : base( 100, 4, false )
		{
			this.dto = dto;
		}

		public _PACKET_BATTLE_ENTITY_START( ulong value ) : base( 100, 4, false )
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