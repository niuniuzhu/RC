namespace RC.ProtoGen
{
	public class FieldEntry
	{
		public string id;

		public string type;

		public DTOEntry subDTO;

		public string clsType
		{
			get
			{
				if ( this.type == "alist" )
					return this.subDTO.ClsName() + "[]";
				return this.type;
			}
		}
	}
}