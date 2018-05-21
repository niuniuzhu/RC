using System;
using System.IO;
using System.Text;

namespace RC.ProtoGen
{
	static class Program
	{
		static int Main( string[] args )
		{
			if ( args.Length < 2 )
			{
				Console.WriteLine( "invalid arguments" );
				return -1;
			}
			string file = args[0];
			string text = File.ReadAllText( file, Encoding.UTF8 );
			Interpreter interpreter = new Interpreter();
			interpreter.Parse( text );
			interpreter.Gen( args[1], args[2] );
			return 0;
		}
	}
}
