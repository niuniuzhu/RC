using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol
{
	[Packet( 100, 0 )]
	public class _PACKET_LV_BATTLE_KEYFRAME : Packet
	{
		public _DTO_keyframe dto;

		public _PACKET_LV_BATTLE_KEYFRAME() : base( 100, 0, -1 )
		{
		}

		public _PACKET_LV_BATTLE_KEYFRAME( _DTO_keyframe dto ) : base( 100, 0, -1 )
		{
			this.dto = dto;
		}

		public _PACKET_LV_BATTLE_KEYFRAME( int frame ) : base( 100, 0, -1 )
		{
			this.dto = new _DTO_keyframe( frame );
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );
			this.dto.Serialize( buffer );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );
			this.dto = new _DTO_keyframe();
			this.dto.Deserialize( buffer );
		}

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}, dto:{this.dto}";
		}
	}
}