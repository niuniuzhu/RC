using System;

namespace RC.Net
{
	[AttributeUsage( AttributeTargets.Class )]
	public sealed class PacketAttribute : Attribute
	{
		public int id;

		public PacketAttribute( byte module, ushort command )
		{
			this.id = NetworkHelper.EncodePacketID( module, command );
		}
	}
}