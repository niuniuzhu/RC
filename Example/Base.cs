using System.Reflection;
using Example.Properties;

namespace Example
{
	public class Base
	{
		public Base()
		{
			AssemblyName[] assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			foreach ( AssemblyName assembly in assemblies )
				Assembly.Load( assembly );

			LoggerProxy.Init( Resources.log4net_config );
		}
	}
}