using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	[Packet( 0, 32000 )]
	public class _PACKET_INTERNAL_BATTLE_CREATE : Packet
	{
		

		public _PACKET_INTERNAL_BATTLE_CREATE() : base( 0, 32000 )
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