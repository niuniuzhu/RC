using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace RC.Net
{
	public sealed class UserTokenPool<T> where T : IUserToken
	{
		private static int _gid;

		private readonly Dictionary<Type, Queue<T>> _typeToObjects = new Dictionary<Type, Queue<T>>();

		public T Pop()
		{
			Type type = typeof( T );
			if ( !this._typeToObjects.TryGetValue( type, out Queue<T> objs ) )
			{
				objs = new Queue<T>();
				this._typeToObjects[type] = objs;
			}
			T obj;
			if ( objs.Count == 0 )
			{
				obj = ( T )Activator.CreateInstance( typeof( T ), BindingFlags.NonPublic | BindingFlags.Instance,
													  Type.DefaultBinder,
													  new object[] { ( ushort )Interlocked.Increment( ref _gid ) }, null );
			}
			else
				obj = objs.Dequeue();
			return obj;
		}

		public void Push( T obj )
		{
			this._typeToObjects[obj.GetType()].Enqueue( obj );
		}

		public void Dispose()
		{
			foreach ( KeyValuePair<Type, Queue<T>> kv in this._typeToObjects )
			{
				Queue<T> queue = kv.Value;
				foreach ( T obj in queue )
					obj.Dispose();
			}
			this._typeToObjects.Clear();
		}
	}
}