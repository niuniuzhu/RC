using System;
using System.Reflection;

namespace RC.Game
{
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class SynchronizeAttribute : Attribute
	{
		public byte id { get; }

		public PropertyInfo owner;

		public SynchronizeAttribute( byte idD )
		{
			this.id = id;
		}
	}
}