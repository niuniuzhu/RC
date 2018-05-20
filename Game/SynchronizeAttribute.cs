using System;
using System.Reflection;

namespace RC.Game
{
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class SynchronizeAttribute : Attribute
	{
		public byte id { get; }

		public bool alwaysSync;

		public PropertyInfo owner;

		public SynchronizeAttribute( byte id )
		{
			this.id = id;
		}
	}
}