using System;
using System.Collections.Generic;

namespace RC.Game
{
	public class GPool
	{
		private readonly Dictionary<Type, Queue<GPoolObject>> _typeToObjects = new Dictionary<Type, Queue<GPoolObject>>();

		public T Pop<T>() where T : GPoolObject, new()
		{
			Type type = typeof( T );
			if ( !this._typeToObjects.TryGetValue( type, out Queue<GPoolObject> objs ) )
			{
				objs = new Queue<GPoolObject>();
				this._typeToObjects[type] = objs;
			}
			GPoolObject obj = objs.Count == 0 ? new T() : objs.Dequeue();
			return ( T )obj;
		}

		public void Push( GPoolObject obj )
		{
			this._typeToObjects[obj.GetType()].Enqueue( obj );
		}

		public void Dispose()
		{
			foreach ( KeyValuePair<Type, Queue<GPoolObject>> kv in this._typeToObjects )
			{
				Queue<GPoolObject> queue = kv.Value;
				foreach ( GPoolObject obj in queue )
					obj.Dispose();
			}
			this._typeToObjects.Clear();
		}
	}
}