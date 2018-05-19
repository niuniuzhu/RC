using System;

namespace RC.Net
{
	public sealed class DistendBuffer
	{
		private byte[] _buffer;

		public DistendBuffer( int initialSize )
		{
			this._buffer = new byte[initialSize];
		}

		public byte[] Get( int size )
		{
			if ( this._buffer.Length < size )
				Array.Resize( ref this._buffer, size );
			return this._buffer;
		}

		public void Clear()
		{
			Array.Clear( this._buffer, 0, this._buffer.Length );
		}
	}
}