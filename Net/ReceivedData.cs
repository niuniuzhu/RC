using System;
using System.Net;
using System.Net.Sockets;

namespace RC.Net
{
	public struct ReceivedData
	{
		public enum Type
		{
			Accept,
			Receive
		}

		public Type type;
		public byte[] data;
		public IUserToken token;
		public Socket conn;
		public EndPoint remoteEndPoint;

		public ReceivedData( SocketAsyncEventArgs socketAsyncEventArgs )
		{
			this.type = Type.Receive;
			this.data = new byte[socketAsyncEventArgs.BytesTransferred];
			Buffer.BlockCopy( socketAsyncEventArgs.Buffer, socketAsyncEventArgs.Offset, this.data, 0, socketAsyncEventArgs.BytesTransferred );
			this.remoteEndPoint = socketAsyncEventArgs.RemoteEndPoint;
			this.token = null;
			this.conn = null;
		}

		public ReceivedData( IUserToken token )
		{
			this.type = Type.Receive;
			this.data = null;
			this.remoteEndPoint = null;
			this.token = token;
			this.conn = null;
		}

		public ReceivedData( byte[] data, EndPoint remoteEndPoint )
		{
			this.type = Type.Receive;
			this.data = data;
			this.remoteEndPoint = remoteEndPoint;
			this.token = null;
			this.conn = null;
		}

		public ReceivedData( Socket conn )
		{
			this.type = Type.Accept;
			this.data = null;
			this.remoteEndPoint = null;
			this.token = null;
			this.conn = conn;
		}
	}
}