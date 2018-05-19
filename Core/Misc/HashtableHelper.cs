using System;
using System.Collections;
using RC.Core.FMath;
using RC.Core.Math;

namespace RC.Core.Misc
{
	public static class HashtableHelper
	{
		public static void Concat( this Hashtable h1, Hashtable h2 )
		{
			if ( h2 == null )
				return;
			foreach ( DictionaryEntry de in h2 )
			{
				if ( h1.ContainsKey( de.Key ) )
					continue;
				h1[de.Key] = de.Value;
			}
		}

		public static int GetInt( this Hashtable ht, string key )
		{
			return Convert.ToInt32( ht[key] );
		}

		public static long GetLong( this Hashtable ht, string key )
		{
			return Convert.ToInt64( ht[key] );
		}

		public static Hashtable GetMap( this Hashtable ht, string key )
		{
			return ht[key] as Hashtable;
		}

		public static ArrayList GetList( this Hashtable ht, string key )
		{
			return ht[key] as ArrayList;
		}

		public static string GetString( this Hashtable ht, string key )
		{
			return Convert.ToString( ht[key] );
		}

		public static byte GetByte( this Hashtable ht, string key )
		{
			return Convert.ToByte( ht[key] );
		}

		public static double GetDouble( this Hashtable ht, string key )
		{
			return Convert.ToDouble( ht[key] );
		}

		public static ushort GetUShort( this Hashtable ht, string key )
		{
			return Convert.ToUInt16( ht[key] );
		}

		public static short GetShort( this Hashtable ht, string key )
		{
			return Convert.ToInt16( ht[key] );
		}

		public static uint GetUInt( this Hashtable ht, string key )
		{
			return Convert.ToUInt32( ht[key] );
		}

		public static ulong GetULong( this Hashtable ht, string key )
		{
			return Convert.ToUInt64( ht[key] );
		}

		public static bool GetBoolean( this Hashtable ht, string key )
		{
			return Convert.ToBoolean( ht[key] );
		}

		public static float GetFloat( this Hashtable ht, string key )
		{
			return Convert.ToSingle( ht[key] );
		}

		public static char GetChar( this Hashtable ht, string key )
		{
			return Convert.ToChar( ht[key] );
		}

		public static Fix64 GetFix64( this Hashtable ht, string key )
		{
			return ( Fix64 )GetFloat( ht, key );
		}

		public static string[] GetStringArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			string[] f = new string[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToString( v[i] );
			return f;
		}

		public static bool[] GetBooleanArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			bool[] f = new bool[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToBoolean( v[i] );
			return f;
		}

		public static byte[] GetByteArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			byte[] f = new byte[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToByte( v[i] );
			return f;
		}

		public static short[] GetShortArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			short[] f = new short[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToInt16( v[i] );
			return f;
		}

		public static int[] GetIntArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			int[] f = new int[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToInt32( v[i] );
			return f;
		}

		public static float[] GetFloatArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			float[] f = new float[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToSingle( v[i] );
			return f;
		}

		public static double[] GetDoubleArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			double[] f = new double[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToDouble( v[i] );
			return f;
		}

		public static long[] GetLongArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			long[] f = new long[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToInt64( v[i] );
			return f;
		}

		public static Fix64[] GetFix64Array( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			Fix64[] f = new Fix64[c];
			for ( int i = 0; i < c; i++ )
				f[i] = ( Fix64 )Convert.ToSingle( v[i] );
			return f;
		}

		public static Vec2 GetVec2( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return Vec2.zero;
			ArrayList v = ( ArrayList )ht[key];
			return new Vec2( Convert.ToSingle( v[0] ), Convert.ToSingle( v[1] ) );
		}

		public static void SetVec2( this Hashtable ht, string key, Vec2 v )
		{
			float[] al = new float[2];
			al[0] = v.x;
			al[1] = v.y;
			ht[key] = al;
		}

		public static FVec2 GetFVec2( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return FVec2.zero;
			ArrayList v = ( ArrayList )ht[key];
			return new FVec2( Convert.ToSingle( v[0] ), Convert.ToSingle( v[1] ) );
		}

		public static void SetFVec2( this Hashtable ht, string key, FVec2 v )
		{
			float[] al = new float[2];
			al[0] = ( float )v.x;
			al[1] = ( float )v.y;
			ht[key] = al;
		}

		public static Vec3 GetVec3( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return Vec3.zero;
			ArrayList v = ( ArrayList )ht[key];
			return new Vec3( Convert.ToSingle( v[0] ), Convert.ToSingle( v[1] ), Convert.ToSingle( v[2] ) );
		}

		public static void SetVec3( this Hashtable ht, string key, Vec3 v )
		{
			float[] al = new float[3];
			al[0] = v.x;
			al[1] = v.y;
			al[2] = v.z;
			ht[key] = al;
		}

		public static FVec3 GetFVec3( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return FVec3.zero;
			ArrayList v = ( ArrayList )ht[key];
			return new FVec3( Convert.ToSingle( v[0] ), Convert.ToSingle( v[1] ), Convert.ToSingle( v[2] ) );
		}

		public static void SetFVec3( this Hashtable ht, string key, FVec3 v )
		{
			float[] al = new float[3];
			al[0] = ( float )v.x;
			al[1] = ( float )v.y;
			al[2] = ( float )v.z;
			ht[key] = al;
		}

		public static Vec4 GetVec4( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return Vec4.zero;
			ArrayList v = ( ArrayList )ht[key];
			return new Vec4( Convert.ToSingle( v[0] ), Convert.ToSingle( v[1] ), Convert.ToSingle( v[2] ), Convert.ToSingle( v[3] ) );
		}

		public static void SetVec4( this Hashtable ht, string key, Vec4 v )
		{
			float[] al = new float[3];
			al[0] = v.x;
			al[1] = v.y;
			al[2] = v.z;
			al[3] = v.w;
			ht[key] = al;
		}

		public static FVec4 GetFVec4( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return FVec4.zero;
			ArrayList v = ( ArrayList )ht[key];
			return new FVec4( Convert.ToSingle( v[0] ), Convert.ToSingle( v[1] ), Convert.ToSingle( v[2] ), Convert.ToSingle( v[3] ) );
		}

		public static void SetFVec4( this Hashtable ht, string key, FVec4 v )
		{
			float[] al = new float[3];
			al[0] = ( float )v.x;
			al[1] = ( float )v.y;
			al[2] = ( float )v.z;
			al[3] = ( float )v.w;
			ht[key] = al;
		}

		public static Vec2 GetVector2FromString( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return Vec2.zero;
			string v = ( string )ht[key];
			string[] arr = v.Split( ',' );
			return new Vec2( Convert.ToSingle( arr[0] ), Convert.ToSingle( arr[1] ) );
		}

		public static Vec3 GetVec3FromString( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return Vec3.zero;
			string v = ( string )ht[key];
			string[] arr = v.Split( ',' );
			return new Vec3( Convert.ToSingle( arr[0] ), Convert.ToSingle( arr[1] ), Convert.ToSingle( arr[2] ) );
		}

		public static Vec4 GetVector4FromString( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return Vec4.zero;
			string v = ( string )ht[key];
			string[] arr = v.Split( ',' );
			return new Vec4( Convert.ToSingle( arr[0] ), Convert.ToSingle( arr[1] ), Convert.ToSingle( arr[2] ), Convert.ToSingle( arr[3] ) );
		}
	}
}