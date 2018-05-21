using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	public class _DTO_byte : DTO
	{
		public byte value;
		
		public _DTO_byte(  )
		{
			
		}
public _DTO_byte( byte value )
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

			this.value = buffer.ReadByte();
		}

		public override string ToString()
		{
			return $"value:{this.value}";
		}
	}
}