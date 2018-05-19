using RC.Core.Misc;
using RC.Core.Xml;
using RC.ProtoGen.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RC.ProtoGen
{
	public class Interpreter
	{
		public static readonly Regex REGEX_CTOR = new Regex( @"\[ctors\s([^\]]+)\](.*?)\[\/ctors\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_CONDITION_CTOR = new Regex( @"\[condition_ctors\s([^\]]+)\](.*?)\[\/condition_ctors\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_FIELD = new Regex( @"\[fields\s([^\]]+)\](.*?)\[\/fields\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_SERIALIZE = new Regex( @"\[serialize\s([^\]]+)\](.*?)\[\/serialize\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_CONDITION = new Regex( @"\[condition\s([^\]]+)\](.*?)\[\/condition\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_RW_BUFFER = new Regex( @"\[rw_buffer\s([^\]]+)\](.*?)\[\/rw_buffer\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_LIST = new Regex( @"\[list\s([^\]]+)\](.*?)\[\/list\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_OPTION = new Regex( @"\[option\](.*?)\[\/option\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_DTOS = new Regex( @"\[dtos\s([^\]]+)\](.*?)\[\/dtos\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_PACKETS = new Regex( @"\[packets\s([^\]]+)\](.*?)\[\/packets\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_MODULES = new Regex( @"\[modules\s([^\]]+)\](.*?)\[\/modules\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_GET_DTO = new Regex( @"\[get_dto\s([^\]]+)\](.*?)\[\/get_dto\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_GET_PACKET0 = new Regex( @"\[get_packet0\s([^\]]+)\](.*?)\[\/get_packet0\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_GET_PACKET1 = new Regex( @"\[get_packet1\s([^\]]+)\](.*?)\[\/get_packet1\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_GET_PACKET2 = new Regex( @"\[get_packet2\s([^\]]+)\](.*?)\[\/get_packet2\]", RegexOptions.Singleline );
		public static readonly Regex REGEX_GET_PACKET3 = new Regex( @"\[get_packet3\s([^\]]+)\](.*?)\[\/get_packet3\]", RegexOptions.Singleline );

		public static readonly Hashtable BUFFER_WRITE;
		public static readonly Hashtable BUFFER_READ;
		public static readonly Hashtable CONVERTIONS;
		public static readonly string DTO_TEMPLATE;
		public static readonly string PACKET_TEMPLATE;
		public static readonly string MGR_TEMPLATE;

		private readonly List<DTOEntry> _dtos = new List<DTOEntry>();
		private readonly List<ModuleEntry> _modules = new List<ModuleEntry>();

		static Interpreter()
		{
			Hashtable bufferReadWrite = ( Hashtable )MiniJSON.JsonDecode( Resources.convertions );
			BUFFER_WRITE = bufferReadWrite.GetMap( "write" );
			BUFFER_READ = bufferReadWrite.GetMap( "read" );
			CONVERTIONS = bufferReadWrite.GetMap( "convert" );
			DTO_TEMPLATE = Resources.dto_template;
			PACKET_TEMPLATE = Resources.packet_template;
			MGR_TEMPLATE = Resources.mgr_template;
		}

		public void Parse( string text )
		{
			XML xml = new XML( text );
			XMLList dtoNodes = xml.Elements( "structs" )[0].Elements();
			foreach ( XML dtoNode in dtoNodes )//parse structs
			{
				if ( !this.CreateStruct( ushort.Parse( dtoNode.GetAttribute( "id" ) ), dtoNode.GetAttribute( "name" ), out DTOEntry dto ) )
					this.ParseField( dto, dtoNode, dtoNodes );
			}
			XMLList moduleNodes = xml.Elements( "modules" )[0].Elements();
			foreach ( XML moduleNode in moduleNodes ) //parse modules
			{
				ModuleEntry module = new ModuleEntry();
				module.id = moduleNode.GetAttribute( "id" );
				module.key = moduleNode.GetAttribute( "key" );
				this._modules.Add( module );

				XMLList packetNodes = moduleNode.Elements();
				foreach ( XML packetNode in packetNodes ) //parse packets
				{
					PacketEntry packet = new PacketEntry( module );
					packet.id = packetNode.GetAttribute( "cmd" );
					packet.key = packetNode.GetAttribute( "key" );
					packet.dto = this.FindDTO( packetNode.GetAttribute( "struct" ) );
					module.packets.Add( packet );
				}
			}
		}

		private void ParseField( IFieldHolder parent, XML parentNode, XMLList nodeElements )
		{
			XMLList fieldNodes = parentNode.Elements();
			foreach ( XML fieldNode in fieldNodes ) //parse packets
			{
				switch ( fieldNode.name )
				{
					case "field":
						FieldEntry field = new FieldEntry();
						field.id = fieldNode.GetAttribute( "id" );
						field.type = fieldNode.GetAttribute( "type" );
						parent.AddField( field );
						if ( field.type == "alist" )
						{
							string dtoName = fieldNode.GetAttribute( "struct" );
							XML subDTONode = FindId( nodeElements, dtoName );
							if ( !this.CreateStruct( ushort.Parse( subDTONode.GetAttribute( "id" ) ), dtoName, out DTOEntry subDTO ) )
								this.ParseField( subDTO, subDTONode, nodeElements );
							field.subDTO = subDTO;
						}
						break;

					case "conditions":
						XMLList conditionNodes = fieldNode.Elements();
						foreach ( XML conditionNode in conditionNodes )
						{
							Condition condition = new Condition( parent );
							condition.key = conditionNode.GetAttribute( "key" );
							condition.values = conditionNode.GetAttribute( "value" ).Split( ',' );
							parent.conditions.Add( condition );
							this.ParseField( condition, conditionNode, nodeElements );
						}
						break;
				}
			}
		}

		private bool CreateStruct( ushort id, string name, out DTOEntry dto )
		{
			int count = this._dtos.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( this._dtos[i].id == id )
				{
					dto = this._dtos[i];
					return true;
				}
			}
			dto = new DTOEntry( id, name );
			this._dtos.Add( dto );
			return false;
		}

		private static XML FindId( XMLList root, string selector )
		{
			foreach ( XML xml in root )
			{
				if ( xml.GetAttribute( "name" ) == selector )
					return xml;
			}
			return null;
		}

		private DTOEntry FindDTO( string name )
		{
			int count = this._dtos.Count;
			for ( int i = 0; i < count; i++ )
				if ( this._dtos[i].name == name )
					return this._dtos[i];
			return null;
		}

		public void Gen( string outputPath )
		{
			foreach ( DTOEntry dto in this._dtos )
				dto.Gen( outputPath );

			foreach ( ModuleEntry module in this._modules )
				foreach ( PacketEntry packet in module.packets )
					packet.Gen( outputPath );

			this.GenManager( outputPath );
			this.GenConst( outputPath );
		}

		private void GenManager( string outputPath )
		{
			string template = this.ProcessDTOs( MGR_TEMPLATE );
			template = this.ProcessPackets( template );
			template = this.ProcessDTOFuncs( template );
			template = this.ProcessPacketFuncs( template );

			File.WriteAllText( Path.Combine( outputPath, "ProtocolManager.cs" ), template, Encoding.UTF8 );
		}

		private string ProcessDTOs( string input )
		{
			Match match = REGEX_DTOS.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;

			StringBuilder sb = new StringBuilder();
			int count = this._dtos.Count;
			for ( int i = 0; i < count; i++ )
			{
				DTOEntry dto = this._dtos[i];
				string content = match.Groups[2].Value.Replace( "[dto_cls_name]", dto.ClsName() );
				content = content.Replace( "[id]", string.Empty + dto.id );
				if ( i != count - 1 )
					content += splitChar;
				sb.Append( content );
			}
			return input.Replace( match.Value, sb.ToString() );
		}

		private string ProcessPackets( string input )
		{
			Match match = REGEX_PACKETS.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;

			StringBuilder sb = new StringBuilder();
			int i = 0;
			int total = 0;
			foreach ( ModuleEntry module in this._modules )
				total += module.packets.Count;
			foreach ( ModuleEntry module in this._modules )
			{
				foreach ( PacketEntry packet in module.packets )
				{
					string content = match.Groups[2].Value.Replace( "[packet_cls_name]", packet.ClsName() );
					content = content.Replace( "[module]", string.Empty + module.id );
					content = content.Replace( "[cmd]", string.Empty + packet.id );
					if ( i != total - 1 )
						content += splitChar;
					sb.Append( content );
					++i;
				}
			}
			return input.Replace( match.Value, sb.ToString() );
		}

		private string ProcessDTOFuncs( string input )
		{
			StringBuilder sb = new StringBuilder();
			Match match = REGEX_GET_DTO.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;

			int c1 = this._dtos.Count;
			for ( int j = 0; j < c1; j++ )
			{
				DTOEntry dto = this._dtos[j];
				string template = match.Groups[2].Value.Replace( "[dto_cls_name]", dto.ClsName() );
				template = template.Replace( "[dto_func_name]", dto.FuncName() );

				string content = template;
				content = this.ProcessFields( content, null );
				content += splitChar;
				sb.Append( content );

				content = template;
				content = this.ProcessFields( content, dto.allFields );
				int c2 = dto.conditions.Count;
				if ( c2 > 0 )
					content += splitChar;
				sb.Append( content );

				for ( int i = 0; i < c2; i++ )
				{
					content = this.ProcessFields( template, dto.conditions[i].allFields );
					if ( i != c2 - 1 )
						content += splitChar;
					sb.Append( content );
				}
				if ( j != c1 - 1 )
					sb.Append( splitChar );
			}
			input = input.Replace( match.Value, sb.ToString() );
			return input;
		}

		private string ProcessPacketFuncs( string input )
		{
			input = this.ProcessPacketFuncs0( input );
			input = this.ProcessPacketFuncs1( input );
			input = this.ProcessPacketFuncs2( input );
			input = this.ProcessPacketFuncs3( input );

			return input;
		}

		private string ProcessPacketFuncs0( string input )
		{
			StringBuilder sb = new StringBuilder();
			int i = 0;
			int total = 0;
			foreach ( ModuleEntry module in this._modules )
				total += module.packets.Count;

			Match match = REGEX_GET_PACKET0.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;
			foreach ( ModuleEntry module in this._modules )
			{
				foreach ( PacketEntry packet in module.packets )
				{
					if ( packet.dto != null )
					{
						string content = match.Groups[2].Value.Replace( "[dto_cls_name]", packet.dto.ClsName() );
						content = content.Replace( "[packet_cls_name]", packet.ClsName() );
						content = content.Replace( "[packet_func_name]", packet.FuncName() );
						content = content.Replace( match.Value, content );
						if ( i != total - 1 )
							content += splitChar;
						sb.Append( content );
					}
					i++;
				}
			}
			return input.Replace( match.Value, sb.ToString() );
		}

		private string ProcessPacketFuncs1( string input )
		{
			StringBuilder sb = new StringBuilder();
			int i = 0;
			int total = 0;
			foreach ( ModuleEntry module in this._modules )
				total += module.packets.Count;

			Match match = REGEX_GET_PACKET1.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;
			foreach ( ModuleEntry module in this._modules )
			{
				foreach ( PacketEntry packet in module.packets )
				{
					string template = match.Groups[2].Value.Replace( "[packet_cls_name]", packet.ClsName() );
					template = template.Replace( "[packet_func_name]", packet.FuncName() );

					string content = this.ProcessFields( template, null );
					if ( packet.dto != null )
						content += splitChar;
					sb.Append( content );

					if ( packet.dto != null )
					{
						content = this.ProcessFields( template, packet.dto.allFields );
						int c2 = packet.dto.conditions.Count;
						if ( c2 > 0 )
							content += splitChar;
						sb.Append( content );

						for ( int j = 0; j < c2; j++ )
						{
							content = this.ProcessFields( template, packet.dto.conditions[j].allFields );
							if ( j != c2 - 1 )
								content += splitChar;
							sb.Append( content );
						}
					}
					if ( i != total - 1 )
						sb.Append( splitChar );
					i++;
				}
			}
			input = input.Replace( match.Value, sb.ToString() );
			return input;
		}

		private string ProcessPacketFuncs2( string input )
		{
			StringBuilder sb = new StringBuilder();
			int i = 0;
			int total = 0;
			foreach ( ModuleEntry module in this._modules )
				total += module.packets.Count;

			Match match = REGEX_GET_PACKET2.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;
			foreach ( ModuleEntry module in this._modules )
			{
				foreach ( PacketEntry packet in module.packets )
				{
					if ( packet.dto != null )
					{
						string content = match.Groups[2].Value.Replace( "[dto_cls_name]", packet.dto.ClsName() );
						content = content.Replace( "[packet_cls_name]", packet.ClsName() );
						content = content.Replace( "[packet_call_name]", packet.CallName() );
						content = content.Replace( match.Value, content );
						if ( i != total - 1 )
							content += splitChar;
						sb.Append( content );
					}
					i++;
				}
			}
			return input.Replace( match.Value, sb.ToString() );
		}

		private string ProcessPacketFuncs3( string input )
		{
			StringBuilder sb = new StringBuilder();
			int i = 0;
			int total = 0;
			foreach ( ModuleEntry module in this._modules )
				total += module.packets.Count;

			Match match = REGEX_GET_PACKET3.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;
			foreach ( ModuleEntry module in this._modules )
			{
				foreach ( PacketEntry packet in module.packets )
				{
					string template = match.Groups[2].Value.Replace( "[packet_cls_name]", packet.ClsName() );
					template = template.Replace( "[packet_call_name]", packet.CallName() );

					string content = this.ProcessFields( template, null );
					MatchCollection collection = REGEX_OPTION.Matches( content );
					foreach ( Match match2 in collection )
						content = content.Replace( match2.Value, string.Empty );
					if ( packet.dto != null )
						content += splitChar;
					sb.Append( content );

					if ( packet.dto != null )
					{
						content = this.ProcessFields( template, packet.dto.allFields );
						collection = REGEX_OPTION.Matches( content );
						foreach ( Match match2 in collection )
							content = content.Replace( match2.Value, match2.Groups[1].Value );

						int c2 = packet.dto.conditions.Count;
						if ( c2 > 0 )
							content += splitChar;
						sb.Append( content );

						for ( int j = 0; j < c2; j++ )
						{
							content = this.ProcessFields( template, packet.dto.conditions[j].allFields );
							if ( j != c2 - 1 )
								content += splitChar;
							sb.Append( content );
						}
					}
					if ( i != total - 1 )
						sb.Append( splitChar );
					i++;
				}
			}
			input = input.Replace( match.Value, sb.ToString() );
			return input;
		}

		private string ProcessFields( string input, List<FieldEntry> mFields )
		{
			StringBuilder sb = new StringBuilder();
			MatchCollection collection = REGEX_FIELD.Matches( input );
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

		private void GenConst( string outputPath )
		{
			string input = Resources.const_template;
			StringBuilder sb = new StringBuilder();
			int i = 0;

			Match match = REGEX_MODULES.Match( Resources.const_template );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;
			foreach ( ModuleEntry module in this._modules )
			{
				string content = match.Groups[2].Value.Replace( "[key]", module.key );
				content = content.Replace( "[id]", module.id );
				if ( i != this._modules.Count - 1 )
					content += splitChar;
				sb.Append( content );
				++i;
			}
			input = input.Replace( match.Value, sb.ToString() );

			sb.Clear();
			i = 0;
			int total = 0;
			foreach ( ModuleEntry module in this._modules )
				total += module.packets.Count;

			match = REGEX_PACKETS.Match( Resources.const_template );
			splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;

			foreach ( ModuleEntry module in this._modules )
			{
				foreach ( PacketEntry packet in module.packets )
				{
					string content = match.Groups[2].Value.Replace( "[key]", packet.key );
					content = content.Replace( "[id]", packet.id );
					if ( i != total - 1 )
						content += splitChar;
					sb.Append( content );
					i++;
				}
			}
			input = input.Replace( match.Value, sb.ToString() );

			File.WriteAllText( Path.Combine( outputPath, "ProtocolConsts.cs" ), input, Encoding.UTF8 );
		}
	}
}