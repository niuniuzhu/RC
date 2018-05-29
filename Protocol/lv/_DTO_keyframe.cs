using System.Collections.Generic;
using RC.Net;
using RC.Net.Protocol;

namespace RC.Game.Protocol.LV
{
	public class _DTO_keyframe : DTO
	{
		public int frame;
		
		public _DTO_keyframe(  )
		{
			
		}
public _DTO_keyframe( int frame )
		{
			this.frame = frame;
		}

		protected override void InternalSerialize( StreamBuffer buffer )
		{
			base.InternalSerialize( buffer );

			buffer.Write( this.frame );
		}

		protected override void InternalDeserialize( StreamBuffer buffer )
		{
			base.InternalDeserialize( buffer );

			this.frame = buffer.ReadInt();
		}

		public override string ToString()
		{
			return $"frame:{this.frame}";
		}
	}
}