using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	public class _DTO_bool : DTO
	{
		public bool value;
		
		public _DTO_bool(  )
		{
			
		}
public _DTO_bool( bool value )
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

			this.value = buffer.ReadBool();
		}

		public override string ToString()
		{
			return $"value:{this.value}";
		}
	}
}