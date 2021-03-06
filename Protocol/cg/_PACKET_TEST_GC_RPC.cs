﻿using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol.CG
{
	[Packet( 255, 1000 )]
	public class _PACKET_TEST_GC_RPC : Packet
	{
		public _DTO_string dto;

		public _PACKET_TEST_GC_RPC() : base( 255, 1000, false )
		{
		}

		public _PACKET_TEST_GC_RPC( _DTO_string dto ) : base( 255, 1000, false )
		{
			this.dto = dto;
		}

		public _PACKET_TEST_GC_RPC( string value ) : base( 255, 1000, false )
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
		
		

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}, dto:{this.dto}";
		}
	}
}