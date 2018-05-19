namespace RC.Net.Protocol
{
	[Packet( NetworkConfig.INTERNAL_MODULE, 1 )]
	public class PacketAccept : Packet
	{
		public ushort tokenId { get; private set; }

		public PacketAccept() : base( NetworkConfig.INTERNAL_MODULE, 1 )//外部协议模块id从100开始
		{
		}

		public PacketAccept( ushort tokenId ) : base( NetworkConfig.INTERNAL_MODULE, 1 )
		{
			this.tokenId = tokenId;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );

			buffer.Write( this.tokenId );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );

			this.tokenId = buffer.ReadUShort();
		}

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}, tokenId:{this.tokenId}";
		}
	}
}