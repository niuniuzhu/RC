using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RC.ProtoGen
{
	public class PacketEntry
	{
		public string id;
		public string key;
		public int reply; 
		public DTOEntry dto;

		public ModuleEntry module { get; }

		public PacketEntry( ModuleEntry module )
		{
			this.module = module;
		}

		public string CallName()
		{
			return $"CALL_{this.module.key}_{this.key}";
		}

		public string FuncName()
		{
			return $"PACKET_{this.module.key}_{this.key}";
		}

		public string ClsName()
		{
			return $"_PACKET_{this.module.key}_{this.key}";
		}

		public void Gen( string outputPath, string ns )
		{
			string output = Interpreter.PACKET_TEMPLATE;
			MatchCollection collection = Interpreter.REGEX_OPTION.Matches( output );
			foreach ( Match match in collection )
			{
				if ( this.dto != null )
					output = output.Replace( match.Value, match.Groups[1].Value );
				else
					output = output.Replace( match.Value, string.Empty );
			}
			if ( this.dto != null )
			{
				output = this.ProcessCtors( output );
				output = output.Replace( "[dto_cls_name]", this.dto.ClsName() );
			}
			output = output.Replace( "[cls_name]", this.ClsName() );
			output = output.Replace( "[module]", this.module.id );
			output = output.Replace( "[cmd]", this.id );
			output = output.Replace( "[reply]", string.Empty + this.reply );
			output = output.Replace( "[ns]", ns );

			File.WriteAllText( Path.Combine( outputPath, this.ClsName() + ".cs" ), output, Encoding.UTF8 );
		}

		private string ProcessCtors( string input )
		{
			StringBuilder sb = new StringBuilder();
			Match match = Interpreter.REGEX_CTOR.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;

			string content = match.Groups[2].Value;
			content = this.ProcessFields( content, this.dto.allFields );
			int count = this.dto.conditions.Count;
			if ( count > 0 )
				content += splitChar;
			sb.Append( content );

			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					content = this.ProcessFields( match.Groups[2].Value, this.dto.conditions[i].allFields );
					if ( i != count - 1 )
						content += splitChar;
					sb.Append( content );
				}
			}
			return input.Replace( match.Value, sb.ToString() );
		}

		private string ProcessFields( string input, List<FieldEntry> mFields )
		{
			StringBuilder sb = new StringBuilder();
			MatchCollection collection = Interpreter.REGEX_FIELD.Matches( input );
			foreach ( Match match in collection )
			{
				sb.Clear();
				int count = mFields?.Count ?? 0;
				if ( count > 0 )
				{
					string splitChar = match.Groups[1].Value;
					splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;
					for ( int i = 0; i < count; i++ )
					{
						FieldEntry field = mFields[i];
						string content = match.Groups[2].Value.Replace( "[field_type]", field.clsType );
						content = content.Replace( "[field_name]", field.id );
						if ( i != count - 1 )
							content += splitChar;
						sb.Append( content );
					}
				}
				input = input.Replace( match.Value, sb.ToString() );
			}
			return input;
		}

		public static int EncodeID( byte module, ushort cmd )
		{
			return ( module << 16 ) | cmd;
		}
	}
}