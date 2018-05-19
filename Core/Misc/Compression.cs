using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using RC.Core.Crypto;

namespace RC.Core.Misc
{
	/// <summary>
	/// 压缩解压相关功能便捷方法
	/// </summary>
	public static class Compression
	{
		/// <summary>
		/// 数据压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <param name="level">压缩等级0~9，设置其它值表示默认等级</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		/// <returns>返回压缩后的数据</returns>
		public static byte[] Compress( byte[] data, int level = -1, bool nowrap = false )
		{
			return Compress( data, 0, data.Length, level, nowrap );
		}

		/// <summary>
		/// 数据压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <param name="level">压缩等级0~9，设置其它值表示默认等级</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		/// <returns>返回压缩后的数据</returns>
		public static byte[] Compress( byte[] data, int offset, int len, int level = -1, bool nowrap = false )
		{
			MemoryStream os = new MemoryStream();
			Compress( data, offset, len, os, level, nowrap );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// 数据压缩
		/// </summary>
		/// <param name="s">待压缩数据输入流</param>
		/// <param name="level">压缩等级0~9，设置其它值表示默认等级</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		/// <returns>返回压缩后的数据</returns>
		public static byte[] Compress( Stream s, int level = -1, bool nowrap = false )
		{
			MemoryStream os = new MemoryStream();
			Compress( s, os, level, nowrap );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// 数据压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <param name="os">压缩后的数据输出流压缩后的数据输出流</param>
		/// <param name="level">压缩等级0~9，设置其它值表示默认等级</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		public static void Compress( byte[] data, Stream os, int level = -1, bool nowrap = false )
		{
			Compress( data, 0, data.Length, os, level, nowrap );
		}

		/// <summary>
		/// 数据压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <param name="os">压缩后的数据输出流</param>
		/// <param name="level">压缩等级0~9，设置其它值表示默认等级</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		public static void Compress( byte[] data, int offset, int len, Stream os, int level = -1, bool nowrap = false )
		{
			if ( level < 0 || level > 9 )
				level = -1;
			Compress( data, offset, len, os, new Deflater( level, nowrap ) );
		}

		/// <summary>
		/// 数据压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <param name="os">压缩后的数据输出流</param>
		/// <param name="deflater">压缩器</param>
		public static void Compress( byte[] data, int offset, int len, Stream os, Deflater deflater )
		{
			DeflaterOutputStream dos = new DeflaterOutputStream( os, deflater, 1024 * 4 );
			dos.Write( data, offset, len );
			dos.Finish();
		}

		/// <summary>
		/// 数据压缩
		/// </summary>
		/// <param name="s">待压缩数据输入流</param>
		/// <param name="os">压缩后的数据输出流</param>
		/// <param name="level">压缩等级0~9，设置其它值表示默认等级</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		public static void Compress( Stream s, Stream os, int level = -1, bool nowrap = false )
		{
			if ( level < 0 || level > 9 )
				level = -1;
			Compress( s, os, new Deflater( level, nowrap ) );
		}

		/// <summary>
		/// 数据压缩
		/// </summary>
		/// <param name="s">待压缩数据输入流</param>
		/// <param name="os">压缩后的数据输出流</param>
		/// <param name="deflater">压缩器</param>
		public static void Compress( Stream s, Stream os, Deflater deflater )
		{
			DeflaterOutputStream dos = new DeflaterOutputStream( os, deflater, 1024 * 4 );
			Stdio.CopyStream( s, dos );
			dos.Finish();
		}

		/// <summary>
		/// 数据解压
		/// </summary>
		/// <param name="data">待解压数据</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		/// <returns>返回解压后的数据</returns>
		public static byte[] Decompress( byte[] data, bool nowrap = false )
		{
			return Decompress( data, 0, data.Length, nowrap );
		}

		/// <summary>
		/// 数据解压
		/// </summary>
		/// <param name="data">待解压数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		/// <returns>返回解压后的数据</returns>
		public static byte[] Decompress( byte[] data, int offset, int len, bool nowrap = false )
		{
			MemoryStream os = new MemoryStream();
			Decompress( data, offset, len, os, nowrap );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// 数据解压
		/// </summary>
		/// <param name="s">待解压数据输入流</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		/// <returns>返回解压后的数据</returns>
		public static byte[] Decompress( Stream s, bool nowrap = false )
		{
			MemoryStream os = new MemoryStream();
			Decompress( s, os, nowrap );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// 数据解压
		/// </summary>
		/// <param name="data">待解压数据</param>
		/// <param name="os">解压后的数据输出流</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		public static void Decompress( byte[] data, Stream os, bool nowrap = false )
		{
			Decompress( data, 0, data.Length, os, nowrap );
		}

		/// <summary>
		/// 数据解压
		/// </summary>
		/// <param name="data">待解压数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <param name="os">解压后的数据输出流</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		public static void Decompress( byte[] data, int offset, int len, Stream os, bool nowrap = false )
		{
			Decompress( new MemoryStream( data, offset, len ), os, nowrap );
		}

		/// <summary>
		/// 数据解压
		/// </summary>
		/// <param name="s">待解压数据输入流</param>
		/// <param name="os">解压后的数据输出流</param>
		/// <param name="nowrap">如果为true，则不使用ZLIB头和校验和字段，仅使用GZIP兼容的压缩</param>
		public static void Decompress( Stream s, Stream os, bool nowrap = false )
		{
			Decompress( s, os, new Inflater( nowrap ) );
		}

		/// <summary>
		/// 数据解压
		/// </summary>
		/// <param name="s">待解压数据输入流</param>
		/// <param name="os">解压后的数据输出流</param>
		/// <param name="inflater">解压器</param>
		public static void Decompress( Stream s, Stream os, Inflater inflater )
		{
			InflaterInputStream iis = new InflaterInputStream( s, inflater, 1024 * 4 );
			Stdio.CopyStream( iis, os );
			os.Flush();
		}

		/// <summary>
		/// GZip压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <returns>返回压缩后的数据</returns>
		public static byte[] GZipCompress( byte[] data )
		{
			return GZipCompress( data, 0, data.Length );
		}

		/// <summary>
		/// GZip压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <returns>返回压缩后的数据</returns>
		public static byte[] GZipCompress( byte[] data, int offset, int len )
		{
			MemoryStream os = new MemoryStream();
			GZipCompress( data, offset, len, os );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// GZip压缩
		/// </summary>
		/// <param name="s">待压缩数据输入流</param>
		/// <returns>返回压缩后的数据</returns>
		public static byte[] GZipCompress( Stream s )
		{
			MemoryStream os = new MemoryStream();
			GZipCompress( s, os );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// GZip压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <param name="os">压缩后的数据输出流</param>
		public static void GZipCompress( byte[] data, Stream os )
		{
			GZipCompress( data, 0, data.Length, os );
		}

		/// <summary>
		/// GZip压缩
		/// </summary>
		/// <param name="data">待压缩数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <param name="os">压缩后的数据输出流</param>
		public static void GZipCompress( byte[] data, int offset, int len, Stream os )
		{
			GZipOutputStream gos = new GZipOutputStream( os, 1024 * 4 );
			gos.Write( data, offset, len );
			gos.Finish();
		}

		/// <summary>
		/// GZip压缩
		/// </summary>
		/// <param name="s">待压缩数据输入流</param>
		/// <param name="os">压缩后的数据输出流</param>
		public static void GZipCompress( Stream s, Stream os )
		{
			GZipOutputStream gos = new GZipOutputStream( os, 1024 * 4 );
			Stdio.CopyStream( s, gos );
			gos.Finish();
		}

		/// <summary>
		/// GZip解压
		/// </summary>
		/// <param name="data">待解压数据</param>
		/// <returns>返回解压后的数据</returns>
		public static byte[] GZipDecompress( byte[] data )
		{
			return GZipDecompress( data, 0, data.Length );
		}

		/// <summary>
		/// GZip解压
		/// </summary>
		/// <param name="data">待解压数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <returns>返回解压后的数据</returns>
		public static byte[] GZipDecompress( byte[] data, int offset, int len )
		{
			MemoryStream os = new MemoryStream();
			GZipDecompress( data, offset, len, os );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// GZip解压
		/// </summary>
		/// <param name="s">待解压数据输入流</param>
		/// <returns>返回压缩后的数据</returns>
		public static byte[] GZipDecompress( Stream s )
		{
			MemoryStream os = new MemoryStream();
			GZipDecompress( s, os );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// GZip解压
		/// </summary>
		/// <param name="data">待解压数据</param>
		/// <param name="os">解压后的数据输出流</param>
		public static void GZipDecompress( byte[] data, Stream os )
		{
			GZipDecompress( data, 0, data.Length, os );
		}

		/// <summary>
		/// GZip解压
		/// </summary>
		/// <param name="data">待解压数据</param>
		/// <param name="offset">数据起始位置</param>
		/// <param name="len">数据长度</param>
		/// <param name="os">解压后的数据输出流</param>
		public static void GZipDecompress( byte[] data, int offset, int len, Stream os )
		{
			MemoryStream ms = new MemoryStream( data, offset, len );
			GZipDecompress( ms, os );
			ms.Close();
		}

		/// <summary>
		/// GZip解压
		/// </summary>
		/// <param name="s">待解压数据输入流</param>
		/// <param name="os">解压后的数据输出流</param>
		public static void GZipDecompress( Stream s, Stream os )
		{
			GZipInputStream gis = new GZipInputStream( s );
			Stdio.CopyStream( gis, os );
			os.Flush();
		}

	}
}