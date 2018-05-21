using RC.Core.Structure;
using RC.Net.Protocol;
using System.Collections.Generic;

namespace RC.Net
{
	public class NativeClient : INetClient
	{
		public delegate void EventHandler( Packet e );

		public event SocketEventHandler OnSocketEvent;
		public ushort id { get; }

		private readonly Dictionary<int, List<EventHandler>> _handlers = new Dictionary<int, List<EventHandler>>();
		private readonly SwitchQueue<Packet> _pendingList = new SwitchQueue<Packet>();

		public void AddListener( byte module, ushort cmd, EventHandler handler )
		{
			int key = NetworkHelper.EncodePacketID( module, cmd );
			if ( !this._handlers.TryGetValue( key, out List<EventHandler> list ) )
			{
				list = new List<EventHandler>();
				this._handlers.Add( key, list );
			}
			list.Add( handler );
		}

		public void RemoveListener( byte module, ushort cmd, EventHandler handler )
		{
			int key = NetworkHelper.EncodePacketID( module, cmd );
			if ( !this._handlers.TryGetValue( key, out List<EventHandler> list ) )
				return;
			bool result = list.Remove( handler );
			if ( !result )
				return;
			if ( list.Count == 0 )
				this._handlers.Remove( key );
		}

		public void Send( Packet packet )
		{
			this._pendingList.Push( packet );
		}

		private void Invoke( Packet e )
		{
			int key = NetworkHelper.EncodePacketID( e.module, e.command );
			if ( !this._handlers.TryGetValue( key, out List<EventHandler> notifyHandlers ) )
				return;
			int count = notifyHandlers.Count;
			for ( int i = 0; i < count; i++ )
				notifyHandlers[i].Invoke( e );
		}

		public void Dispose()
		{
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