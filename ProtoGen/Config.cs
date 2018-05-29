using RC.Core.Misc;
using System.Collections;
using System.IO;

namespace RC.ProtoGen
{
	public class Config
	{
		public string version { get; private set; }
		public Task[] tasks { get; private set; }

		public Config( string content )
		{
			Hashtable json = ( Hashtable )MiniJSON.JsonDecode( content );
			this.version = json.GetString( "version" );
			ArrayList list = json.GetList( "tasks" );
			this.tasks = new Task[list.Count];
			for ( int i = 0; i < list.Count; i++ )
				this.tasks[i] = new Task( ( Hashtable )list[i] );
		}

		public class Task
		{
			public string flag;
			public string protocol;
			public string outputPath;
			public string @namespace;

			public Task( Hashtable ht )
			{
				this.flag = ht.GetString( "flag" );
				this.protocol = ht.GetString( "protocol" );
				this.outputPath = Path.Combine( Directory.GetCurrentDirectory(), ht.GetString( "output_path" ) );
				this.@namespace = ht.GetString( "namespace" );
			}
		}
	}
}