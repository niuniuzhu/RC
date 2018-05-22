using RC.Core.Structure;
using RC.Net.Protocol;
using System.Collections.Generic;
using RC.Core.Misc;

namespace RC.Net
{
	public class NativeTransmitter : INetServer
	{
		public event SocketEventHandler OnSocketEvent;

		private readonly SwitchQueue<Packet> _pendingList = new SwitchQueue<Packet>();

		private void Invoke( Packet packet )
		{
			SocketEvent e = new SocketEvent( SocketEvent.Type.Receive, packet, null );
			this.OnSocketEvent?.Invoke( e );
		}

		public void Dispose()
		{
		}

		public void Stop()
		{
		}

		public void Start( int port )
		{
		}

		public void Send( Packet packet )
		{
		}

		public void Send( ushort tokenId, Packet packet )
		{
		}

		public void Send( IEnumerable<ushort> tokenIds, Packet packet )
		{
		}

		public void SendAll( Packet packet )
		{
			this._pendingList.Push( packet );
		}

		public void Close()
		{
		}

		public void Connect( string ip, int port )
		{
		}

		public void Update( long dt )
		{
			this._pendingList.Switch();
			while ( !this._pendingList.isEmpty )
			{
				Packet e = this._pendingList.Pop();
				this.Invoke( e );
			}
		}
	}
}