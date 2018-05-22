using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol
{
	[Packet( 0, 6 )]
	public class _PACKET_LV_BATTLE_TRANSFORM : Packet
	{
		public _DTO_transform dto;

		public _PACKET_LV_BATTLE_TRANSFORM() : base( 0, 6 )
		{
		}

		public _PACKET_LV_BATTLE_TRANSFORM( _DTO_transform dto ) : base( 0, 6 )
		{
			this.dto = dto;
		}

		public _PACKET_LV_BATTLE_TRANSFORM( float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z ) : base( 0, 6 )
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