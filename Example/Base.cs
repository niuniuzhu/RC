using System.Reflection;
using Example.Misc;
using Example.Properties;

namespace Example
{
	public abstract class Base
	{
		protected Base()
		{
			AssemblyName[] assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			foreach ( AssemblyName assembly in assemblies )
				Assembly.Load( assembly );

			LoggerProxy.Init( Resources.log4net_config );
		}
	}
}