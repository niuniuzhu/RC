using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RC.Net.Kcp
{
	public class KCPNative : IKCP
	{
		private static readonly Dictionary<IntPtr, KCPNative> NATIVE_TO_KCP = new Dictionary<IntPtr, KCPNative>();
		private static readonly DistendBuffer BUF_POOL = new DistendBuffer( 3 * NetworkConfig.BUFFER_SIZE );
		private static readonly Output OUTPUT_DELEGATE = OutputHandler;

		private static void OutputHandler( IntPtr buf, int len, IntPtr kcp, IntPtr userPtr )
		{
			if ( !NATIVE_TO_KCP.TryGetValue( kcp, out KCPNative kcpNative ) )
				throw new Exception( "Could not found kcp_native instance" );

			uint user = Marshal.PtrToStructure<uint>( userPtr );
			byte[] data = BUF_POOL.Get( len );
			Marshal.Copy( buf, data, 0, len );
			kcpNative._output?.Invoke( data, len, user );
		}

		private static readonly WriteLog LOG_DELEGATE = LogHandler;

		private static void LogHandler( string log, IntPtr kcp, IntPtr userPtr )
		{
			if ( !NATIVE_TO_KCP.TryGetValue( kcp, out KCPNative kcpNative ) )
				throw new Exception( "Could not found kcp_native instance" );

			uint user = Marshal.PtrToStructure<uint>( userPtr );
			kcpNative._writelog?.Invoke( log, user );
		}

		private readonly IntPtr _ikcpcb;
		private KCP.OutputHandler _output;
		private KCP.LoggerHandler _writelog;

		public KCPNative( uint conv, uint user, KCP.OutputHandler output, KCP.LoggerHandler writelog )
		{
			int size = Marshal.SizeOf( user );
			IntPtr intPtr = Marshal.AllocHGlobal( size );
			Marshal.StructureToPtr( user, intPtr, false );
			this._ikcpcb = KCPInterface.ikcp_create( conv, intPtr );
			Marshal.FreeHGlobal( intPtr );
			NATIVE_TO_KCP[this._ikcpcb] = this;
			this._output = output;
			this._writelog = writelog;
			IntPtr outputPtr = Marshal.GetFunctionPointerForDelegate( OUTPUT_DELEGATE );
			KCPInterface.ikcp_setoutput( this._ikcpcb, outputPtr );
			IntPtr logPtr = Marshal.GetFunctionPointerForDelegate( LOG_DELEGATE );
			KCPInterface.ikcp_setlog( this._ikcpcb, logPtr );
		}

		public void Dispose()
		{
			this._output = null;
			this._writelog = null;
			NATIVE_TO_KCP.Remove( this._ikcpcb );
			KCPInterface.ikcp_release( this._ikcpcb );
		}

		public void Clear()
		{
			KCPInterface.ikcp_clear( this._ikcpcb );
		}

		public void SetLogMask( int mask )
		{
			KCPInterface.ikcp_setlogmask( this._ikcpcb, mask );
		}

		public int PeekSize()
		{
			return KCPInterface.ikcp_peeksize( this._ikcpcb );
		}

		public int Recv( byte[] buffer, int size )
		{
			int result = KCPInterface.ikcp_recv( this._ikcpcb, ref buffer[0], size );
			return result;
		}

		public int Send( byte[] buffer, int offset, int size )
		{
			int result = KCPInterface.ikcp_send( this._ikcpcb, ref buffer[offset], size );
			return result;
		}

		public int Input( byte[] data, int offset, int size )
		{
			int result = KCPInterface.ikcp_input( this._ikcpcb, ref data[offset], size );
			return result;
		}

		public void Update( uint current )
		{
			KCPInterface.ikcp_update( this._ikcpcb, current );
		}

		public uint Check( uint current )
		{
			return KCPInterface.ikcp_check( this._ikcpcb, current );
		}

		public int SetMtu( int mtu )
		{
			return KCPInterface.ikcp_setmtu( this._ikcpcb, mtu );
		}

		public int NoDelay( int nodelay, int interval, int resend, int nc )
		{
			return KCPInterface.ikcp_nodelay( this._ikcpcb, nodelay, interval, resend, nc );
		}

		public int WndSize( int sndwnd, int rcvwnd )
		{
			return KCPInterface.ikcp_wndsize( this._ikcpcb, sndwnd, rcvwnd );
		}

		public int Interval( int interval )
		{
			return KCPInterface.ikcp_interval( this._ikcpcb, interval );
		}

		public int WaitSnd()
		{
			return KCPInterface.ikcp_waitsnd( this._ikcpcb );
		}
	}
}