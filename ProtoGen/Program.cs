using System.IO;
using System.Text;

namespace RC.ProtoGen
{
	static class Program
	{
		static void Main( string[] args )
		{
			string file = args[0];
			string text = File.ReadAllText( file, Encoding.UTF8 );
			Interpreter interpreter = new Interpreter();
			interpreter.Parse( text );
			interpreter.Gen( args[1] );

			//Console.ReadLine();
		}
	}
}
