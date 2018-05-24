using System.Collections;
using System.Collections.Generic;

namespace RC.Net
{
	public class UserTokenManager<T> : IEnumerable<T> where T : IUserToken
	{
		private readonly List<T> _tokens = new List<T>();
		private readonly Dictionary<ushort, T> _idToTokens = new Dictionary<ushort, T>();
		private readonly UserTokenPool<T> _userTokenPool = new UserTokenPool<T>();

		public T this[int index] => this._tokens[index];
		public int count => this._tokens.Count;

		public void Dispose()
		{
			this._userTokenPool.Dispose();
		}

		public void Clear()
		{
			int c = this._tokens.Count;
			for ( int i = 0; i < c; i++ )
				this._userTokenPool.Push( this._tokens[i] );
			this._tokens.Clear();
			this._idToTokens.Clear();
		}

		public T Create()
		{
			T token = this._userTokenPool.Pop();
			this._tokens.Add( token );
			this._idToTokens.Add( token.id, token );
			return token;
		}

		public void Destroy( int index )
		{
			T token = this._tokens[index];
			this._tokens.RemoveAt( index );
			this._idToTokens.Remove( token.id );
			this._userTokenPool.Push( token );
		}

		public T Get( ushort tokenID )
		{
			this._idToTokens.TryGetValue( tokenID, out T token );
			return token;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this._tokens.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}