using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol
{
	public class _DTO_frame_info : DTO
	{
		public _DTO_action_info[] actions;
public int frameId;
		
		public _DTO_frame_info(  )
		{
			
		}
public _DTO_frame_info( _DTO_action_info[] actions,int frameId )
		{
			this.actions = actions;
this.frameId = frameId;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );

			int count = this.actions.Length;
				buffer.Write( ( ushort )count );
				for ( int i = 0; i < count; ++i )
					this.actions[i].Serialize( buffer );
				
buffer.Write( this.frameId );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );

			int count = buffer.ReadUShort();
				this.actions = new _DTO_action_info[count];
				for ( int i = 0; i < count; ++i )
				{
					var actions = this.actions[i] = new _DTO_action_info();
					actions.Deserialize( buffer );
				}
this.frameId = buffer.ReadInt();
		}

		public override string ToString()
		{
			return $"actions:{this.actions},frameId:{this.frameId}";
		}
	}
}