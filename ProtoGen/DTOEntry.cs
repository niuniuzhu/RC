using RC.Core.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RC.ProtoGen
{
	public interface IFieldHolder
	{
		List<FieldEntry> fields { get; }
		List<FieldEntry> allFields { get; }
		List<Condition> conditions { get; }
		void AddField( FieldEntry field );
	}

	public class DTOEntry : IFieldHolder
	{
		public ushort id { get; }
		public string name { get; }
		public List<Condition> conditions { get; } = new List<Condition>();
		public List<FieldEntry> fields => this._fields ?? ( this._fields = this._fieldsMap.Values.ToList() );
		public List<FieldEntry> allFields
		{
			get
			{
				if ( this._allFields != null )
					return this._allFields;

				HashSet<string> fieldIds = new HashSet<string>();
				this._allFields = new List<FieldEntry>( this.fields );
				foreach ( Condition condition in this.conditions )
				{
					int count = condition.fields.Count;
					for ( int i = 0; i < count; i++ )
					{
						FieldEntry field = condition.fields[i];
						if ( fieldIds.Contains( field.id ) )
							continue;
						this._allFields.Add( field );
						fieldIds.Add( field.id );
					}
				}
				return this._allFields;
			}
		}

		private List<FieldEntry> _fields;
		private List<FieldEntry> _allFields;
		private readonly SortedDictionary<string, FieldEntry> _fieldsMap = new SortedDictionary<string, FieldEntry>();
		private bool _isInternal;

		public DTOEntry( ushort id, string name, bool isInternal )
		{
			this.id = id;
			this.name = name;
			this._isInternal = isInternal;
		}

		public void AddField( FieldEntry field )
		{
			this._fieldsMap[field.id] = field;
		}

		public string FuncName()
		{
			return $"DTO_{this.name}";
		}

		public string ClsName()
		{
			return $"_DTO_{this.name}";
		}

		public void Gen( string outputPath, string ns )
		{
			if ( this._isInternal )
				return;

			string output = this.ProcessCtors( Interpreter.DTO_TEMPLATE );
			output = this.ProcessSerialize( output );
			output = this.ProcessFields( output, this.allFields );
			output = output.Replace( "[cls_name]", this.ClsName() );
			output = output.Replace( "[ns]", ns );

			if ( !Directory.Exists( outputPath ) )
				Directory.CreateDirectory( outputPath );
			File.WriteAllText( Path.Combine( outputPath, this.ClsName() + ".cs" ), output, Encoding.UTF8 );
		}

		private string ProcessCtors( string input )
		{
			StringBuilder sb = new StringBuilder();
			Match match = Interpreter.REGEX_CTOR.Match( input );
			string splitChar = match.Groups[1].Value;
			splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;

			string content = this.ProcessFields( match.Groups[2].Value, null );
			content += splitChar;
			sb.Append( content );

			content = this.ProcessFields( match.Groups[2].Value, this.allFields );
			int count = this.conditions.Count;
			if ( count > 0 )
				content += splitChar;
			sb.Append( content );

			for ( int i = 0; i < count; i++ )
			{
				content = this.ProcessFields( match.Groups[2].Value, this.conditions[i].allFields );
				if ( i != count - 1 )
					content += splitChar;
				sb.Append( content );
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

		private string ProcessSerialize( string input )
		{
			StringBuilder sb = new StringBuilder();
			StringBuilder sb1 = new StringBuilder();
			MatchCollection collection = Interpreter.REGEX_SERIALIZE.Matches( input );
			foreach ( Match match in collection )
			{
				sb.Clear();

				Match match2 = Interpreter.REGEX_RW_BUFFER.Match( match.Groups[2].Value );
				string content = this.ProcessSerialize( match2.Value, this.fields );
				sb.Append( content );

				int count = this.conditions.Count;
				if ( count > 0 )
				{
					string splitChar = match.Groups[1].Value;
					splitChar = splitChar == "\\n" ? Environment.NewLine : splitChar;
					for ( int i = 0; i < count; i++ )
					{
						Condition condition = this.conditions[i];
						content = this.ProcessSerialize( match.Groups[2].Value, condition.fields );
						Match match3 = Interpreter.REGEX_CONDITION.Match( content );
						sb1.Clear();
						int c2 = condition.values.Length;
						for ( int j = 0; j < c2; j++ )
						{
							string content1 = match3.Groups[2].Value.Replace( "[key]", condition.key );
							string value = string.Format( Interpreter.CONVERTIONS.GetString( this._fieldsMap[condition.key].type ), $"\"{condition.values[j]}\"" );
							content1 = content1.Replace( "[value]", value );
							if ( j != c2 - 1 )
								content1 += match3.Groups[1].Value;
							sb1.Append( content1 );
						}
						content = content.Replace( match3.Value, sb1.ToString() );
						if ( i != count - 1 )
							content += splitChar;
						sb.Append( content );
					}
				}
				input = input.Replace( match.Value, sb.ToString() );
			}
			return input;
		}

		private string ProcessSerialize( string input, List<FieldEntry> mFields )
		{
			StringBuilder sb = new StringBuilder();
			MatchCollection collection = Interpreter.REGEX_RW_BUFFER.Matches( input );
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
						string content = this.ProcessSerializeField( match.Groups[2].Value, field );
						content = content.Replace( "[field_name]", field.id );
						content = content.Replace( "[write_method]",
												   Interpreter.BUFFER_WRITE.GetString( field.clsType ) );
						content = content.Replace( "[read_method]",
												   Interpreter.BUFFER_READ.GetString( field.clsType ) );
						if ( i != count - 1 )
							content += splitChar;
						sb.Append( content );
					}
				}
				input = input.Replace( match.Value, sb.ToString() );
			}
			return input;
		}

		private string ProcessSerializeField( string input, FieldEntry field )
		{
			MatchCollection collection = Interpreter.REGEX_LIST.Matches( input );
			if ( field.type == "alist" )
			{
				foreach ( Match match in collection )
				{
					input = match.Groups[2].Value;
					input = input.Replace( "[sub_dto_cls_name]", field.subDTO.ClsName() );
				}
			}
			else
			{
				foreach ( Match match in collection )
				{
					input = input.Replace( match.Value, string.Empty );
				}
			}
			input = input.Replace( "[field_name]", field.id );
			return input;
		}
	}

	public class Condition : IFieldHolder
	{
		public string key;
		public string[] values;
		public List<Condition> conditions => null;
		public List<FieldEntry> fields => this._fields ?? ( this._fields = this._fieldsMap.Values.ToList() );
		public List<FieldEntry> allFields
		{
			get
			{
				if ( this._allFields != null )
					return this._allFields;
				HashSet<string> fieldIds = new HashSet<string>();
				this._allFields = new List<FieldEntry>( this._dto.fields );
				int count = this.fields.Count;
				for ( int i = 0; i < count; i++ )
				{
					FieldEntry field = this.fields[i];
					if ( fieldIds.Contains( field.id ) )
						continue;
					this._allFields.Add( field );
					fieldIds.Add( field.id );
				}
				return this._allFields;
			}
		}

		private readonly IFieldHolder _dto;
		private List<FieldEntry> _fields;
		private List<FieldEntry> _allFields;
		private readonly SortedDictionary<string, FieldEntry> _fieldsMap = new SortedDictionary<string, FieldEntry>();

		public Condition( IFieldHolder dto )
		{
			this._dto = dto;
		}

		public void AddField( FieldEntry field )
		{
			this._fieldsMap[field.id] = field;
		}
	}
}