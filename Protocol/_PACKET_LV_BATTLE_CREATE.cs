using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol
{
	[Packet( 0, 1 )]
	public class _PACKET_LV_BATTLE_CREATE : Packet
	{
		

		public _PACKET_LV_BATTLE_CREATE() : base( 0, 1 )
		{
		}

		

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );
			
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );
			
		}

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}";
		}
	}
}