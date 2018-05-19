using System;
using RC.Core.Misc;

namespace RC.Net.Protocol
{
	public abstract class Packet : Serializable
	{
		public byte module { get; private set; }
		public ushort command { get; private set; }
		public uint createTime { get; private set; }
		public ulong pid { get; private set; }

		protected Packet( byte module, ushort command )
		{
			this.module = module;
			this.command = command;
			this.createTime = ( uint )TimeUtils.utcTime;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			byte[] data = new byte[8];
			data[0] = this.module;
			ByteUtils.Encode16u( data, 1, this.command );
			ByteUtils.Encode32u( data, 3, this.createTime );
			buffer.Write( data );
			this.pid = BitConverter.ToUInt64( data, 0 );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			byte[] data = buffer.ReadBytes( 8 );
			this.module = data[0];
			this.command = ByteUtils.Decode16u( data, 1 );
			this.createTime = ByteUtils.Decode32u( data, 3 );
			this.pid = BitConverter.ToUInt64( data, 0 );
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