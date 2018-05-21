using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	[Packet( 0, 32001 )]
	public class _PACKET_INTERNAL_BATTLE_DESTROY : Packet
	{
		

		public _PACKET_INTERNAL_BATTLE_DESTROY() : base( 0, 32001 )
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