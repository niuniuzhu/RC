using System.Collections.Generic;

namespace RC.ProtoGen
{
	public class ModuleEntry
	{
		public string id;
		public string key;
		public readonly List<PacketEntry> packets = new List<PacketEntry>();
	}
}