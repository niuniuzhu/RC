using System.Collections.Generic;
using RC.Net.Protocol;

namespace RC.Net
{
	public class ClientRPCManager
	{
		private ushort _packetIncrement;
		private readonly Dictionary<ushort, RPCHandler> _rpcHandlers = new Dictionary<ushort, RPCHandler>();

		public void Clear()
		{
			this._packetIncrement = 0;
			this._rpcHandlers.Clear();
		}

		public void Accept( Packet packet, RPCHandler callback )
		{
			if ( !packet.isRPCCall )
				return;
			ushort pid = this.GetNextPacketId();
			packet.pid = pid;
			this._rpcHandlers[pid] = callback;
		}

		private ushort GetNextPacketId()
		{
			ushort value = this._packetIncrement;
			++this._packetIncrement;
			return value;
		}

		public void Invoke( Packet packet )
		{
			if ( !packet.isRPCReturn )
				return;
			RPCHandler handler = this._rpcHandlers[packet.srcPid];
			this._rpcHandlers.Remove( packet.srcPid );
			handler.Invoke( null, packet );
		}
	}
}