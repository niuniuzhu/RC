using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	public class _DTO_uint : DTO
	{
		public uint value;
		
		public _DTO_uint(  )
		{
			
		}
public _DTO_uint( uint value )
		{
			this.value = value;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );

			buffer.Write( this.value );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );

			this.value = buffer.ReadUInt();
		}

		public override string ToString()
		{
			return $"value:{this.value}";
		}
	}
}