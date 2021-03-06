﻿namespace RC.Net.Protocol
{
	public class _DTO_short : DTO
	{
		public short value;
		
		public _DTO_short(  )
		{
			
		}
public _DTO_short( short value )
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

			this.value = buffer.ReadShort();
		}

		public override string ToString()
		{
			return $"value:{this.value}";
		}
	}
}