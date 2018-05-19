using System.Net.Sockets;
using RC.Net.Protocol;

namespace RC.Net
{
	public struct SocketEvent
	{
		public enum Type
		{
			Accept,
			Disconnect,
			Connect,
			Receive,
			Close
		}

		public Type type { get; }
		public IUserToken userToken { get; }
		public string msg { get; }
		public SocketError errorCode { get; }
		public Packet packet { get; }
		public byte[] data { get; }

		public SocketEvent( Type type, string msg, SocketError errorCode, IUserToken userToken )
		{
			this.type = type;
			this.msg = msg;
			this.errorCode = errorCode;
			this.userToken = userToken;
			this.packet = null;
			this.data = null;
		}

		public SocketEvent( Type type, Packet packet, IUserToken userToken )
		{
			this.type = type;
			this.userToken = userToken;
			this.packet = packet;
			this.msg = string.Empty;
			this.errorCode = SocketError.Success;
			this.data = null;
		}
	}
}