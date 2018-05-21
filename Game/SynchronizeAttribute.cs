using System;
using System.Reflection;

namespace RC.Game
{
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class SynchronizeAttribute : Attribute
	{
		public byte id { get; }

		public PropertyInfo property;

		public SynchronizeAttribute( byte idD )
		{
			this.id = id;
		}
	}
}