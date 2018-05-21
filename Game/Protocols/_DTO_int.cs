using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	public class _DTO_int : DTO
	{
		public int value;
		
		public _DTO_int(  )
		{
			
		}
public _DTO_int( int value )
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

			this.value = buffer.ReadInt();
		}

		public override string ToString()
		{
			return $"value:{this.value}";
		}
	}
}