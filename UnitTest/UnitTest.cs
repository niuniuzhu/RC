using System.Reflection;
using System.Threading;
using RC.Core.Misc;
using RC.Net;
using UnitTest.lockstep;
using UnitTest.Misc;
using UnitTest.Properties;
using Xunit;
using Xunit.Abstractions;

namespace UnitTest
{
	public class UnitTest
	{
		public static ITestOutputHelper output;

		public UnitTest( ITestOutputHelper o )
		{
			output = o;

			AssemblyName[] assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			foreach ( AssemblyName assembly in assemblies )
				Assembly.Load( assembly );

			LoggerProxy.Init( Resources.log4net_config );
		}

		[Fact]
		public void Test1()
		{
			Lockstep lockstep = new Lockstep( NetworkManager.PType.Tcp, "127.0.0.1", 2551 );
		}

		[Fact]
		public void Test2()
		{
			new NetworkTest.NetworkTest();
		}
	}
}
