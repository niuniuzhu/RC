namespace RC.Net.Protocol
{
	public class _DTO_ushort : DTO
	{
		public ushort value;
		
		public _DTO_ushort(  )
		{
			
		}
public _DTO_ushort( ushort value )
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

			this.value = buffer.ReadUShort();
		}

		public override string ToString()
		{
			return $"value:{this.value}";
		}
	}
}