using RC.Net.Protocol;
using System.Collections.Generic;

namespace RC.Net
{
	public delegate void PacketHandler( Packet packet );

	public class PacketDispatcher
	{
		private readonly PacketListener _listener = new PacketListener();
		private readonly ISocketEventHolder _eventHolder;

		public PacketDispatcher( ISocketEventHolder eventHolder )
		{
			this._eventHolder = eventHolder;
			this._eventHolder.OnSocketEvent += this.OnSocketEvent;
		}

		public void Dispose()
		{
			this._eventHolder.OnSocketEvent -= this.OnSocketEvent;
		}

		private void OnSocketEvent( SocketEvent e )
		{
			if ( e.type != SocketEvent.Type.Receive )
				return;
			this._listener.Invoke( e.packet );
		}

		public void AddListener( byte module, ushort cmd, PacketHandler handler )
		{
			this._listener.Add( NetworkHelper.EncodePacketID( module, cmd ), handler );
		}

		public void RemoveListener( byte module, ushort cmd, PacketHandler handler )
		{
			this._listener.Remove( NetworkHelper.EncodePacketID( module, cmd ), handler );
		}
	}

	sealed class PacketListener
	{
		private readonly Dictionary<int, PacketHandler> _cmdToHandler = new Dictionary<int, PacketHandler>();

		internal void Add( int key, PacketHandler handler )
		{
			if ( this._cmdToHandler.TryGetValue( key, out PacketHandler handler2 ) )
			{
				handler2 -= handler;
				handler2 += handler;
			}
			else
				handler2 = handler;
			this._cmdToHandler[key] = handler2;
		}

		internal bool Remove( int key )
		{
			if ( !this._cmdToHandler.ContainsKey( key ) )
				return false;
			this._cmdToHandler.Remove( key );
			return true;
		}

		internal bool Remove( int key, PacketHandler handler )
		{
			if ( this._cmdToHandler.TryGetValue( key, out PacketHandler handler2 ) )
			{
				handler2 -= handler;
				if ( handler2 == null )
					this._cmdToHandler.Remove( key );
				else
					this._cmdToHandler[key] = handler2;
				return true;
			}
			return false;
		}

		internal void Clear()
		{
			this._cmdToHandler.Clear();
		}

		internal void Invoke( Packet packet )
		{
			int key = NetworkHelper.EncodePacketID( packet.module, packet.command );
			if ( this._cmdToHandler.TryGetValue( key, out PacketHandler handler2 ) )
				handler2.Invoke( packet );
		}
	}
}