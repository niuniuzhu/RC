using System;
using System.Runtime.InteropServices;

namespace RC.Net.Kcp
{
	[StructLayout( LayoutKind.Sequential )]
	public struct iqueue_head
	{
		private IntPtr next;//iqueue_head*
		private IntPtr prev;//iqueue_head*
	}

	public delegate void Output( IntPtr buf, int len, IntPtr kcp, IntPtr userPtr );
	public delegate void WriteLog( string log, IntPtr kcp, IntPtr userPtr );

	[StructLayout( LayoutKind.Sequential )]
	public struct ikcpcb
	{
		private uint conv, mtu, mss, state;
		private uint snd_una, snd_nxt, rcv_nxt;
		private uint ts_recent, ts_lastack, ssthresh;
		private uint rx_rttval, rx_srtt, rx_rto, rx_minrto;
		private uint snd_wnd, rcv_wnd, rmt_wnd, cwnd, probe;
		private uint current, interval, ts_flush, xmit;
		private uint nrcv_buf, nsnd_buf;
		private uint nrcv_que, nsnd_que;
		private uint nodelay, updated;
		private uint ts_probe, probe_wait;
		private uint dead_link, incr;
		private iqueue_head snd_queue;
		private iqueue_head rcv_queue;
		private iqueue_head snd_buf;
		private iqueue_head rcv_buf;
		private IntPtr acklist;//uint*
		private uint ackcount;
		private uint ackblock;
		private IntPtr user;//void*
		private IntPtr buffer;//char*
		private int fastresend;
		private int nocwnd, stream;
		private int logmask;
		private IntPtr output;
		private IntPtr writeLog;

		public override string ToString()
		{
			return this.mtu.ToString();
		}
	}

	public static class KCPInterface
	{
		public static void InitMemoryPool()
		{
			ikcp_init_allocator();
		}
#if UNITY_IOS
		private const string DLL_PATH = "__Internal";
#else
		private const string DLL_PATH = "kcplib";
#endif

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern unsafe IntPtr ikcp_encode8u( char* p, byte c );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern unsafe IntPtr ikcp_decode8u( char* p, byte c );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern unsafe IntPtr ikcp_encode16u( char* p, ushort c );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern unsafe IntPtr ikcp_decode16u( char* p, ushort c );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern unsafe IntPtr ikcp_encode32u( char* p, uint c );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern unsafe IntPtr ikcp_decode32u( char* p, uint c );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern IntPtr ikcp_create( uint conv, IntPtr user );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern void ikcp_release( IntPtr kcp );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern void ikcp_clear( IntPtr kcp );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern void ikcp_setlogmask( IntPtr kcp, int mask );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern void ikcp_setlog( IntPtr kcp, IntPtr logFun );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern void ikcp_setoutput( IntPtr kcp, IntPtr outputFun );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_peeksize( IntPtr kcp );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_recv( IntPtr kcp, ref byte buffer, int len );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_send( IntPtr kcp, ref byte buffer, int len );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern void ikcp_update( IntPtr kcp, uint current );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_input( IntPtr kcp, ref byte data, int size );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern uint ikcp_check( IntPtr kcp, uint current );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_setmtu( IntPtr kcp, int mtu );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_wndsize( IntPtr kcp, int sndwnd, int rcvwnd );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_waitsnd( IntPtr kcp );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_nodelay( IntPtr kcp, int nodelay, int interval, int resend, int nc );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_interval( IntPtr kcp, int interval );

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif

		#endregion
		public static extern int ikcp_init_allocator();

		#region dllimport
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_ANDROID
		[DllImport( DLL_PATH )]
#else
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
#endif
		#endregion
		public static extern int ikcp_release_allocator();
	}
}