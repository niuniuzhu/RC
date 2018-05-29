using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol.LV
{
	[Packet( 100, 1 )]
	public class _PACKET_BATTLE_CREATE : Packet
	{
		

		public _PACKET_BATTLE_CREATE() : base( 100, 1, false )
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