using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocols
{
	public class _DTO_action_info : DTO
	{
		public string sender;
public byte type;
public float x;
public float y;
public float z;
public string target;
		
		public _DTO_action_info(  )
		{
			
		}
public _DTO_action_info( string sender,byte type,float x,float y,float z,string target )
		{
			this.sender = sender;
this.type = type;
this.x = x;
this.y = y;
this.z = z;
this.target = target;
		}
public _DTO_action_info( string sender,byte type,float x,float y,float z )
		{
			this.sender = sender;
this.type = type;
this.x = x;
this.y = y;
this.z = z;
		}
public _DTO_action_info( string sender,byte type,string target )
		{
			this.sender = sender;
this.type = type;
this.target = target;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );

			buffer.WriteUTF8( this.sender );
buffer.Write( this.type );
			if ( this.type == byte.Parse("0"))
			{
				buffer.Write( this.x );
buffer.Write( this.y );
buffer.Write( this.z );
			}

			if ( this.type == byte.Parse("1"))
			{
				buffer.WriteUTF8( this.target );
			}
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );

			this.sender = buffer.ReadUTF8();
this.type = buffer.ReadByte();
			if ( this.type == byte.Parse("0"))
			{
				this.x = buffer.ReadFloat();
this.y = buffer.ReadFloat();
this.z = buffer.ReadFloat();
			}

			if ( this.type == byte.Parse("1"))
			{
				this.target = buffer.ReadUTF8();
			}
		}

		public override string ToString()
		{
			return $"sender:{this.sender},type:{this.type},x:{this.x},y:{this.y},z:{this.z},target:{this.target}";
		}
	}
}