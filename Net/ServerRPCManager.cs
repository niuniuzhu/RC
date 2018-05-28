using System.Collections.Generic;
using RC.Net.Protocol;

namespace RC.Net
{
	public delegate void RPCHandler( IUserToken token, Packet packet );

	public class ServerRPCManager
	{
		private readonly Dictionary<IUserToken, ushort> _packetIncrements = new Dictionary<IUserToken, ushort>();
		private readonly Dictionary<IUserToken, Dictionary<ushort, RPCHandler>> _rpcsHandlers = new Dictionary<IUserToken, Dictionary<ushort, RPCHandler>>();

		public void OnUserTokenSpawn( IUserToken token )
		{
			this._packetIncrements[token] = 0;
			this._rpcsHandlers[token] = new Dictionary<ushort, RPCHandler>();
		}

		public void OnUserTokenDespawn( IUserToken token )
		{
			this._packetIncrements.Remove( token );
			this._rpcsHandlers.Remove( token );
		}

		public void Accept( IUserToken token, Packet packet, RPCHandler callback )
		{
			if ( !packet.isRPCCall )
				return;
			ushort pid = this.GetNextPacketId( token );
			packet.pid = pid;
			Dictionary<ushort, RPCHandler> handlers = this._rpcsHandlers[token];
			handlers[pid] = callback;
		}

		public void Invoke( IUserToken token, Packet packet )
		{
			if ( !packet.isRPCReturn )
				return;
			Dictionary<ushort, RPCHandler> map = this._rpcsHandlers[token];
			RPCHandler handler = map[packet.srcPid];
			map.Remove( packet.srcPid );
			handler.Invoke( token, packet );
		}

		private ushort GetNextPacketId( IUserToken token )
		{
			ushort value = this._packetIncrements[token];
			++this._packetIncrements[token];
			return value;
		}
	}
}