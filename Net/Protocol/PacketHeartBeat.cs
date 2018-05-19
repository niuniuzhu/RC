using RC.Core.Misc;

namespace RC.Net.Protocol
{
	[Packet( NetworkConfig.INTERNAL_MODULE, 0 )]
	public class PacketHeartBeat : Packet
	{
		public long localTime { get; private set; }
		public long remoteTime { get; private set; }
		public long lag { get; private set; }
		public long appxRemoteTime { get; private set; }
		public long timeDiff { get; private set; }

		public PacketHeartBeat() : base( NetworkConfig.INTERNAL_MODULE, 0 )//外部协议模块id从100开始
		{
		}

		public PacketHeartBeat( long remoteTime ) : base( NetworkConfig.INTERNAL_MODULE, 0 )
		{
			this.remoteTime = remoteTime;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );

			buffer.Write( this.localTime );
			buffer.Write( this.remoteTime );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );

			this.localTime = buffer.ReadLong();
			this.remoteTime = buffer.ReadLong();
		}

		protected override void InternalOnSend()
		{
			this.localTime = TimeUtils.utcTime;
		}

		protected override void InternalOnReceive()
		{
			this.lag = ( TimeUtils.utcTime - this.remoteTime ) / 2;
			this.appxRemoteTime = this.localTime + this.lag;
			this.timeDiff = TimeUtils.utcTime - this.appxRemoteTime;
		}

		public override string ToString()
		{
			return $"module:{this.module}, cmd:{this.command}, lag:{this.lag}, appx_remote_time:{TimeUtils.GetLocalTime( this.appxRemoteTime )}, diff:{this.timeDiff}";
		}
	}
}