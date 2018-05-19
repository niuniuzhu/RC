using System.Collections.Concurrent;

namespace RC.Net.Protocol
{
	public abstract class Serializable : ISerializable
	{
		public void Serialize( StreamBuffer buffer )
		{
			this.InternalSerialize( buffer );
		}

		public void Deserialize( StreamBuffer buffer )
		{
			this.InternalDeserialize( buffer );
		}

		protected virtual void InternalSerialize( StreamBuffer buffer ) { }

		protected virtual void InternalDeserialize( StreamBuffer buffer ) { }

		public static byte[] Serialize( Packet packet )
		{
			StreamBuffer buffer = BufferPool.Pop();
			packet.Serialize( buffer );
			byte[] result = buffer.ToArray();
			BufferPool.Push( buffer );
			return result;
		}

		public static void Deserialize( Packet packet, byte[] data, int offset, int size )
		{
			StreamBuffer buffer = BufferPool.Pop();
			buffer.Write( 0, data, offset, size );
			packet.Deserialize( buffer );
			BufferPool.Push( buffer );
		}
	}

	static class BufferPool
	{
		private static readonly ConcurrentStack<StreamBuffer> POOL = new ConcurrentStack<StreamBuffer>();

		internal static StreamBuffer Pop()
		{
			if ( POOL.IsEmpty )
				return new StreamBuffer();
			POOL.TryPop( out StreamBuffer buffer );
			return buffer;
		}

		internal static void Push( StreamBuffer buffer )
		{
			buffer.Clear();
			POOL.Push( buffer );
		}
	}
}