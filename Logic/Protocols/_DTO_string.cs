using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocols
{
	public class _DTO_string : DTO
	{
		public string value;
		
		public _DTO_string(  )
		{
			
		}
public _DTO_string( string value )
		{
			this.value = value;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );

			buffer.WriteUTF8( this.value );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );

			this.value = buffer.ReadUTF8();
		}

		public override string ToString()
		{
			return $"value:{this.value}";
		}
	}
}