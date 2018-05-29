using System;
using System.IO;
using System.Text;

namespace RC.ProtoGen
{
	static class Program
	{
		static int Main( string[] args )
		{
			string configPath = args.Length == 0 ? Path.Combine( Directory.GetCurrentDirectory() + "/proto_config.json" ) : args[0];

			string content;
			try
			{
				content = File.ReadAllText( configPath );
			}
			catch ( Exception e )
			{
				Console.WriteLine( e );
				return -1;
			}

			Config config = new Config( content );
			foreach ( Config.Task task in config.tasks )
			{
				string file = task.protocol;
				string text;
				try
				{
					text = File.ReadAllText( file, Encoding.UTF8 );
				}
				catch ( Exception e )
				{
					Console.WriteLine( e );
					return -1;
				}
				Interpreter interpreter = new Interpreter();
				interpreter.Parse( text );
				interpreter.Gen( task.outputPath, task.flag, task.@namespace );
			}
			return 0;
		}
	}
}
