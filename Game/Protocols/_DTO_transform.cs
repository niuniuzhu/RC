using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace Protocol.Gen
{
	public class _DTO_transform : DTO
	{
		public float position_x;
public float position_y;
public float position_z;
public float rotation_x;
public float rotation_y;
public float rotation_z;
		
		public _DTO_transform(  )
		{
			
		}
public _DTO_transform( float position_x,float position_y,float position_z,float rotation_x,float rotation_y,float rotation_z )
		{
			this.position_x = position_x;
this.position_y = position_y;
this.position_z = position_z;
this.rotation_x = rotation_x;
this.rotation_y = rotation_y;
this.rotation_z = rotation_z;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );

			buffer.Write( this.position_x );
buffer.Write( this.position_y );
buffer.Write( this.position_z );
buffer.Write( this.rotation_x );
buffer.Write( this.rotation_y );
buffer.Write( this.rotation_z );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );

			this.position_x = buffer.ReadFloat();
this.position_y = buffer.ReadFloat();
this.position_z = buffer.ReadFloat();
this.rotation_x = buffer.ReadFloat();
this.rotation_y = buffer.ReadFloat();
this.rotation_z = buffer.ReadFloat();
		}

		public override string ToString()
		{
			return $"position_x:{this.position_x},position_y:{this.position_y},position_z:{this.position_z},rotation_x:{this.rotation_x},rotation_y:{this.rotation_y},rotation_z:{this.rotation_z}";
		}
	}
}