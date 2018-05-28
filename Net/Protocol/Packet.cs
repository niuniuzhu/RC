namespace RC.Net.Protocol
{
	public abstract class Packet : Serializable
	{
		public byte module { get; private set; }
		public ushort command { get; private set; }
		public bool isRPCCall { get; private set; }

		public ushort pid;
		public ushort srcPid;
		public bool isRPCReturn;

		protected Packet( byte module, ushort command, bool isRPCCall )
		{
			this.module = module;
			this.command = command;
			this.isRPCCall = isRPCCall;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			buffer.Write( this.module );
			buffer.Write( this.command );
			buffer.Write( this.pid );
			buffer.Write( this.isRPCReturn );
			if ( this.isRPCReturn )
				buffer.Write( this.srcPid );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			this.module = buffer.ReadByte();
			this.command = buffer.ReadUShort();
			this.pid = buffer.ReadUShort();
			this.isRPCReturn = buffer.ReadBool();
			if ( this.isRPCReturn )
				this.srcPid = buffer.ReadUShort();
		}

		internal void OnSend()
		{
			this.InternalOnSend();
		}

		internal void OnReceive()
		{
			this.InternalOnReceive();
		}

		protected virtual void InternalOnSend()
		{
		}

		protected virtual void InternalOnReceive()
		{
		}

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}";
		}
	}
}