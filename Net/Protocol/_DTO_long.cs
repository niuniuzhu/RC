namespace RC.Net.Protocol
{
	public class _DTO_long : DTO
	{
		public long value;
		
		public _DTO_long(  )
		{
			
		}
public _DTO_long( long value )
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

			this.value = buffer.ReadLong();
		}

		public override string ToString()
		{
			return $"value:{this.value}";
		}
	}
}