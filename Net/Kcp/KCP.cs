using System;
using System.Collections.Generic;
using RC.Core.Misc;

namespace RC.Net.Kcp
{
	public class KCP : IKCP
	{
		public const int IKCP_RTO_NDL = 30; // no delay min rto
		public const int IKCP_RTO_MIN = 100; // normal min rto
		public const int IKCP_RTO_DEF = 200;
		public const int IKCP_RTO_MAX = 60000;
		public const int IKCP_CMD_PUSH = 81; // cmd: push data
		public const int IKCP_CMD_ACK = 82; // cmd: ack
		public const int IKCP_CMD_WASK = 83; // cmd: window probe (ask)
		public const int IKCP_CMD_WINS = 84; // cmd: window size (tell)
		public const int IKCP_ASK_SEND = 1; // need to send IKCP_CMD_WASK
		public const int IKCP_ASK_TELL = 2; // need to send IKCP_CMD_WINS
		public const int IKCP_WND_SND = 32;
		public const int IKCP_WND_RCV = 32;
		//鉴于Internet上的标准MTU值为576字节,所以我建议在进行Internet的UDP编程时.
		//最好将UDP的数据长度控件在548字节(576-8-20)以内
		public const int IKCP_MTU_DEF = 512; //默认MTU 1400
		public const int IKCP_ACK_FAST = 3;
		public const int IKCP_INTERVAL = 100;
		public const int IKCP_OVERHEAD = 24;
		public const int IKCP_DEADLINK = 20; //原来是10
		public const int IKCP_THRESH_INIT = 2;
		public const int IKCP_THRESH_MIN = 2;
		public const int IKCP_PROBE_INIT = 7000; // 7 secs to probe window size
		public const int IKCP_PROBE_LIMIT = 120000; // up to 120 secs to probe window

		#region 内存申请和释放
		private static Func<int, byte[]> _bufferAlloc;
		private static Action<byte[]> _bufferFree;

		public static void InitMemoryPool( int maxArrayLength, int maxArraysPreBucket )
		{
			ArrayPool<byte> bytePool = ArrayPool<byte>.Create( maxArrayLength, maxArraysPreBucket );
			_bufferAlloc = size => bytePool.Rent( size );
			_bufferFree = buf =>
			{
				bytePool.Return( buf );
			};
		}

		private readonly Stack<Segment> _segPool = new Stack<Segment>();
		private Segment PopSegment( int size )
		{
			if ( this._segPool.Count == 0 )
			{
				for ( int i = 0; i < 10; i++ )
					this._segPool.Push( new Segment() );
			}
			Segment ret = this._segPool.Pop();
			byte[] buf = _bufferAlloc( size );
			ret.data = new ArraySegment<byte>( buf, 0, size );
			return ret;
		}

		private void PushSegment( Segment seg )
		{
			_bufferFree( seg.data.Array );
			this._segPool.Push( seg );
		}

		#endregion

		private static uint _imin_( uint a, uint b )
		{
			return a <= b ? a : b;
		}

		private static uint _imax_( uint a, uint b )
		{
			return a >= b ? a : b;
		}

		private static uint _ibound_( uint lower, uint middle, uint upper )
		{
			return _imin_( _imax_( lower, middle ), upper );
		}

		private static int _itimediff( uint later, uint earlier )
		{
			return ( int )( later - earlier );
		}

		private class Segment
		{
			internal uint conv;
			internal uint cmd;
			internal uint frg;
			internal uint wnd;
			internal uint ts;
			internal uint sn;
			internal uint una;
			internal uint resendts;
			internal uint rto;
			internal uint fastack;
			internal uint xmit;
			internal ArraySegment<byte> data;

			// encode a segment into buffer
			internal int Encode( byte[] ptr, int offset )
			{
				int mOffset = offset;

				offset += ByteUtils.Encode32u( ptr, offset, this.conv );
				offset += ByteUtils.Encode8u( ptr, offset, ( byte )this.cmd );
				offset += ByteUtils.Encode8u( ptr, offset, ( byte )this.frg );
				offset += ByteUtils.Encode16u( ptr, offset, ( ushort )this.wnd );
				offset += ByteUtils.Encode32u( ptr, offset, this.ts );
				offset += ByteUtils.Encode32u( ptr, offset, this.sn );
				offset += ByteUtils.Encode32u( ptr, offset, this.una );
				//offset += Encode32u(ptr, offset, (uint)data.Length);
				offset += ByteUtils.Encode32u( ptr, offset, ( uint )this.data.Count );

				return offset - mOffset;
			}
		}

		private readonly uint _conv;
		private readonly uint _user;
		private uint _mtu;
		private uint _mss;
		private uint _sndUna;
		private uint _sndNxt;
		private uint _rcvNxt;
		private uint _ssthresh;
		private uint _rxRttval;
		private uint _rxSrtt;
		private uint _rxRto;
		private uint _rxMinrto;
		private uint _sndWnd;
		private uint _rcvWnd;
		private uint _rmtWnd;
		private uint _cwnd;
		private uint _probe;
		private uint _current;
		private uint _interval;
		private uint _tsFlush;
		private uint _nodelay;
		private uint _updated;
		private uint _tsProbe;
		private uint _probeWait;
		//private uint _deadLink;
		private uint _incr;
		private byte[] _buffer;
		private int _fastresend;
		private int _nocwnd;

		private readonly LinkedList<Segment> _sndQueue = new LinkedList<Segment>();
		private readonly LinkedList<Segment> _rcvQueue = new LinkedList<Segment>();
		private readonly LinkedList<Segment> _sndBuf = new LinkedList<Segment>();
		private readonly LinkedList<Segment> _rcvBuf = new LinkedList<Segment>();


		public delegate int OutputHandler( byte[] data, int size, uint user );
		private readonly OutputHandler _output;

		public delegate void LoggerHandler( string log, uint user );
		private readonly LoggerHandler _writelog;

		// create a new kcp control object, 'conv' must equal in two endpoint
		// from the same connection.
		public KCP( uint conv, uint user, OutputHandler output, LoggerHandler writelog )
		{
			this._conv = conv;
			this._user = user;
			this._sndWnd = IKCP_WND_SND;
			this._rcvWnd = IKCP_WND_RCV;
			this._rmtWnd = IKCP_WND_RCV;
			this._mtu = IKCP_MTU_DEF;
			this._mss = this._mtu - IKCP_OVERHEAD;
			this._rxRto = IKCP_RTO_DEF;
			this._rxMinrto = IKCP_RTO_MIN;
			this._interval = IKCP_INTERVAL;
			this._tsFlush = IKCP_INTERVAL;
			this._ssthresh = IKCP_THRESH_INIT;
			//this._deadLink = IKCP_DEADLINK;
			this._buffer = _bufferAlloc( ( int )( ( this._mtu + IKCP_OVERHEAD ) * 3 ) );
			this._output = output;
			this._writelog = writelog;
		}

		public void Dispose()
		{
			this.Clear();

			if ( this._ackList != null )
			{
				_bufferFree( this._ackList );
				this._ackList = null;
			}

			_bufferFree( this._buffer );
			this._buffer = null;
		}

		public void Clear()
		{
			foreach ( Segment v in this._sndBuf )
				this.PushSegment( v );
			this._sndBuf.Clear();

			foreach ( Segment v in this._sndQueue )
				this.PushSegment( v );
			this._sndQueue.Clear();

			foreach ( Segment v in this._rcvBuf )
				this.PushSegment( v );
			this._rcvBuf.Clear();

			foreach ( Segment v in this._rcvQueue )
				this.PushSegment( v );
			this._rcvQueue.Clear();

			this._segPool.Clear();

			if ( this._ackList != null )
			{
				_bufferFree( this._ackList );
				this._ackList = null;
			}

			this._sndUna = 0;
			this._sndNxt = 0;
			this._rcvNxt = 0;
			this._tsProbe = 0;
			this._probeWait = 0;
			this._cwnd = 0;
			this._incr = 0;
			this._probe = 0;
			this._ackBlock = 0;
			this._ackCount = 0;
			this._rxSrtt = 0;
			this._rxRttval = 0;
			this._current = 0;
			this._updated = 0;
		}

		// check the size of next message in the recv queue
		public int PeekSize()
		{
			if ( 0 == this._rcvQueue.Count )
				return -1;

			Segment seq = this._rcvQueue.First.Value;

			if ( 0 == seq.frg )
				return seq.data.Count;


			if ( this._rcvQueue.Count < seq.frg + 1 )
				return -1;

			int length = 0;

			foreach ( Segment item in this._rcvQueue )
			{
				length += item.data.Count;
				if ( 0 == item.frg )
					break;
			}

			return length;
		}

		// user/upper level recv: returns size, returns below zero for EAGAIN
		public int Recv( byte[] buffer, int size )
		{
			if ( 0 == this._rcvQueue.Count )
				return -1;

			int peekSize = this.PeekSize();
			if ( 0 > peekSize )
				return -2;

			if ( peekSize > buffer.Length )
				return -3;

			bool fastRecover = this._rcvQueue.Count >= this._rcvWnd;

			// merge fragment.
			int n = 0;
			LinkedListNode<Segment> node = this._rcvQueue.First;
			while ( node != null )
			{
				Segment seg = node.Value;
				Buffer.BlockCopy( seg.data.Array, seg.data.Offset, buffer, n, seg.data.Count );
				n += seg.data.Count;
				uint frg = seg.frg;
				LinkedListNode<Segment> next = node.Next;
				this._rcvQueue.Remove( node );
				this.PushSegment( seg );
				node = next;
				if ( 0 == frg )
					break;
			}
			node = this._rcvBuf.First;
			while ( node != null )
			{
				Segment seg = node.Value;
				if ( seg.sn == this._rcvNxt &&
					 this._rcvQueue.Count < this._rcvWnd )
				{
					LinkedListNode<Segment> tmp = node.Next;
					this._rcvBuf.Remove( node );
					this._rcvQueue.AddLast( node );
					node = tmp;
					this._rcvNxt++;
				}
				else
					break;
			}

			if ( this._rcvQueue.Count < this._rcvWnd && fastRecover )
			{
				// ready to send back IKCP_CMD_WINS in ikcp_flush
				// tell remote my window size
				this._probe |= IKCP_ASK_TELL;
			}

			return n;
		}

		public int Send( byte[] buffer, int offset, int size )
		{
			if ( 0 == size )
				return -1;

			int count;

			if ( size < this._mss )
				count = 1;
			else
				count = ( int )( size + this._mss - 1 ) / ( int )this._mss;

			if ( 255 < count )
				return -2;

			if ( 0 == count )
				count = 1;

			int mOffset = offset;

			for ( int i = 0; i < count; i++ )
			{
				int mSize = size > this._mss ? ( int )this._mss : size;
				Segment seg = this.PopSegment( mSize );
				Buffer.BlockCopy( buffer, mOffset, seg.data.Array, seg.data.Offset, mSize );
				mOffset += mSize;
				seg.frg = ( uint )( count - i - 1 );
				this._sndQueue.AddLast( seg );
			}

			return 0;
		}

		// update ack.
		private void update_ack( int rtt )
		{
			if ( 0 == this._rxSrtt )
			{
				this._rxSrtt = ( uint )rtt;
				this._rxRttval = ( uint )rtt / 2;
			}
			else
			{
				int delta = ( int )( ( uint )rtt - this._rxSrtt );
				if ( 0 > delta )
					delta = -delta;

				this._rxRttval = ( 3 * this._rxRttval + ( uint )delta ) / 4;
				this._rxSrtt = ( uint )( ( 7 * this._rxSrtt + rtt ) / 8 );
				if ( this._rxSrtt < 1 )
					this._rxSrtt = 1;
			}

			int rto = ( int )( this._rxSrtt + _imax_( 1, 4 * this._rxRttval ) );
			this._rxRto = _ibound_( this._rxMinrto, ( uint )rto, IKCP_RTO_MAX );
		}

		private void shrink_buf()
		{
			this._sndUna = this._sndBuf.Count > 0 ? this._sndBuf.First.Value.sn : this._sndNxt;
		}

		private void parse_ack( uint sn )
		{
			if ( _itimediff( sn, this._sndUna ) < 0 ||
				 _itimediff( sn, this._sndNxt ) >= 0 )
				return;

			LinkedListNode<Segment> node = this._sndBuf.First;
			while ( node != null )
			{
				Segment seg = node.Value;
				LinkedListNode<Segment> tmp = node.Next;
				if ( sn == seg.sn )
				{
					this._sndBuf.Remove( node );
					this.PushSegment( seg );
					break;
				}
				seg.fastack++;
				node = tmp;
			}
		}

		private void parse_una( uint una )
		{
			LinkedListNode<Segment> node = this._sndBuf.First;
			while ( node != null )
			{
				Segment seg = node.Value;
				LinkedListNode<Segment> tmp = node.Next;
				if ( _itimediff( una, seg.sn ) > 0 )
				{
					this._sndBuf.Remove( node );
					this.PushSegment( seg );
				}
				else
				{
					break;
				}
				node = tmp;
			}
		}

		private uint _ackCount, _ackBlock;
		private byte[] _ackList;

		private unsafe void ack_push( uint sn, uint ts )
		{
			uint newsize = this._ackCount + 1;
			if ( newsize > this._ackBlock )
			{
				uint newblock;
				for ( newblock = 8; newblock < newsize; newblock <<= 1 )
				{
				}
				byte[] tmpAcklist = _bufferAlloc( ( int )( newblock * sizeof( uint ) * 2 ) );
				if ( this._ackList != null )
				{
					uint x;
					fixed ( byte* ptmp = tmpAcklist )
					{
						fixed ( byte* pcur = this._ackList )
						{
							uint* ptrtmp = ( uint* )ptmp;
							uint* ptrCur = ( uint* )pcur;

							for ( x = 0; x < this._ackCount; x++ )
							{
								ptrtmp[x * 2 + 0] = ptrCur[x * 2 + 0];
								ptrtmp[x * 2 + 1] = ptrCur[x * 2 + 1];
							}
						}
					}
					_bufferFree( this._ackList );
				}

				this._ackList = tmpAcklist;
				this._ackBlock = newblock;
			}

			fixed ( byte* p = this._ackList )
			{
				uint* pint = ( uint* )p;
				uint offset = this._ackCount * 2;
				pint[offset + 0] = sn;
				pint[offset + 1] = ts;
			}
			this._ackCount++;
		}

		private unsafe void ack_get( int p, ref uint sn, ref uint ts )
		{
			fixed ( byte* pbyte = this._ackList )
			{
				uint* ptr = ( uint* )pbyte;
				sn = ptr[p * 2 + 0];
				ts = ptr[p * 2 + 1];
			}
		}

		private void parse_data( Segment newseg )
		{
			uint sn = newseg.sn;
			if ( _itimediff( sn, this._rcvNxt + this._rcvWnd ) >= 0 ||
				 _itimediff( sn, this._rcvNxt ) < 0 )
			{
				this.PushSegment( newseg );
				return;
			}


			bool repeat = false;
			LinkedListNode<Segment> p, prev;
			for ( p = this._rcvBuf.Last; p != this._rcvBuf.First; p = prev )
			{
				Segment seg = p.Value;
				prev = p.Previous;
				if ( seg.sn == sn )
				{
					repeat = true;
					break;
				}

				if ( _itimediff( sn, seg.sn ) > 0 )
					break;
			}

			if ( repeat == false )
			{
				if ( p == null )
					this._rcvBuf.AddFirst( newseg );
				else
					this._rcvBuf.AddBefore( p, newseg );
			}
			else
				this.PushSegment( newseg );

			LinkedListNode<Segment> node = this._rcvBuf.First;
			while ( node != null )
			{
				Segment seg = node.Value;
				LinkedListNode<Segment> tmp = node.Next;
				if ( seg.sn == this._rcvNxt &&
					 this._rcvQueue.Count < this._rcvWnd )
				{
					this._rcvBuf.Remove( node );
					this._rcvQueue.AddLast( node );
					this._rcvNxt++;
				}
				else
					break;
				node = tmp;
			}
		}

		// when you received a low level packet (eg. UDP packet), call it
		public int Input( byte[] data, int offset, int size )
		{
			uint sUna = this._sndUna;
			if ( size < IKCP_OVERHEAD )
				return -1;

			int mOffset = offset;
			while ( true )
			{
				uint ts = 0;
				uint sn = 0;
				uint length = 0;
				uint una = 0;
				uint conv = 0;

				ushort wnd = 0;

				byte cmd = 0;
				byte frg = 0;

				if ( size - mOffset < IKCP_OVERHEAD )
					break;

				mOffset += ByteUtils.Decode32u( data, mOffset, ref conv );

				if ( this._conv != conv )
					return -1;

				mOffset += ByteUtils.Decode8u( data, mOffset, ref cmd );
				mOffset += ByteUtils.Decode8u( data, mOffset, ref frg );
				mOffset += ByteUtils.Decode16u( data, mOffset, ref wnd );
				mOffset += ByteUtils.Decode32u( data, mOffset, ref ts );
				mOffset += ByteUtils.Decode32u( data, mOffset, ref sn );
				mOffset += ByteUtils.Decode32u( data, mOffset, ref una );
				mOffset += ByteUtils.Decode32u( data, mOffset, ref length );

				if ( size - mOffset < length )
					return -2;

				switch ( cmd )
				{
					case IKCP_CMD_PUSH:
					case IKCP_CMD_ACK:
					case IKCP_CMD_WASK:
					case IKCP_CMD_WINS:
						break;
					default:
						return -3;
				}

				this._rmtWnd = wnd;
				this.parse_una( una );
				this.shrink_buf();

				if ( IKCP_CMD_ACK == cmd )
				{
					if ( _itimediff( this._current, ts ) >= 0 )
					{
						this.update_ack( _itimediff( this._current, ts ) );
					}
					this.parse_ack( sn );
					this.shrink_buf();
				}
				else if ( IKCP_CMD_PUSH == cmd )
				{
					if ( _itimediff( sn, this._rcvNxt + this._rcvWnd ) < 0 )
					{
						this.ack_push( sn, ts );
						if ( _itimediff( sn, this._rcvNxt ) >= 0 )
						{
							Segment seg = this.PopSegment( ( int )length ); // new Segment((int)length);
							seg.conv = conv;
							seg.cmd = cmd;
							seg.frg = frg;
							seg.wnd = wnd;
							seg.ts = ts;
							seg.sn = sn;
							seg.una = una;

							if ( length > 0 )
								Buffer.BlockCopy( data, mOffset, seg.data.Array, seg.data.Offset, ( int )length );

							this.parse_data( seg );
						}
					}
				}
				else if ( IKCP_CMD_WASK == cmd )
				{
					// ready to send back IKCP_CMD_WINS in Ikcp_flush
					// tell remote my window size
					this._probe |= IKCP_ASK_TELL;
				}
				else if ( IKCP_CMD_WINS == cmd )
				{
					// do nothing
				}
				else
					return -3;

				mOffset += ( int )length;
			}

			if ( _itimediff( this._sndUna, sUna ) > 0 )
			{
				if ( this._cwnd < this._rmtWnd )
				{
					uint mss = this._mss;
					if ( this._cwnd < this._ssthresh )
					{
						this._cwnd++;
						this._incr += mss;
					}
					else
					{
						if ( this._incr < mss )
						{
							this._incr = mss;
						}
						this._incr += mss * mss / this._incr + mss / 16;
						if ( ( this._cwnd + 1 ) * mss <= this._incr )
							this._cwnd++;
					}
					if ( this._cwnd > this._rmtWnd )
					{
						this._cwnd = this._rmtWnd;
						this._incr = this._rmtWnd * mss;
					}
				}
			}

			return 0;
		}

		private int wnd_unused()
		{
			if ( this._rcvQueue.Count < this._rcvWnd )
				return ( int )this._rcvWnd - this._rcvQueue.Count;
			return 0;
		}

		// flush pending data
		private void Flush()
		{
			uint current = this._current;
			int change = 0;
			int lost = 0;

			if ( 0 == this._updated )
				return;

			Segment seg = this.PopSegment( 0 ); // new Segment(0);
			seg.conv = this._conv;
			seg.cmd = IKCP_CMD_ACK;
			seg.wnd = ( uint )this.wnd_unused();
			seg.una = this._rcvNxt;

			// flush acknowledges
			uint count = this._ackCount;
			int offset = 0;
			for ( int i = 0; i < count; i++ )
			{
				if ( offset + IKCP_OVERHEAD > this._mtu )
				{
					this.Output( this._buffer, offset );
					offset = 0;
				}
				this.ack_get( i, ref seg.sn, ref seg.ts );
				offset += seg.Encode( this._buffer, offset );
			}
			this._ackCount = 0;

			// probe window size (if remote window size equals zero)
			if ( 0 == this._rmtWnd )
			{
				if ( 0 == this._probeWait )
				{
					this._probeWait = IKCP_PROBE_INIT;
					this._tsProbe = this._current + this._probeWait;
				}
				else
				{
					if ( _itimediff( this._current, this._tsProbe ) >= 0 )
					{
						if ( this._probeWait < IKCP_PROBE_INIT )
							this._probeWait = IKCP_PROBE_INIT;
						this._probeWait += this._probeWait / 2;
						if ( this._probeWait > IKCP_PROBE_LIMIT )
							this._probeWait = IKCP_PROBE_LIMIT;
						this._tsProbe = this._current + this._probeWait;
						this._probe |= IKCP_ASK_SEND;
					}
				}
			}
			else
			{
				this._tsProbe = 0;
				this._probeWait = 0;
			}

			// flush window probing commands
			if ( ( this._probe & IKCP_ASK_SEND ) != 0 )
			{
				seg.cmd = IKCP_CMD_WASK;
				if ( offset + IKCP_OVERHEAD > ( int )this._mtu )
				{
					this.Output( this._buffer, offset );
					offset = 0;
				}
				offset += seg.Encode( this._buffer, offset );
			}

			this._probe = 0;

			// calculate window size
			uint cwnd = _imin_( this._sndWnd, this._rmtWnd );
			if ( 0 == this._nocwnd )
				cwnd = _imin_( this._cwnd, cwnd );

			//count = 0;
			//将发送队列（snd_queue)中的数据添加到发送缓存(snd_bu)
			// move data from snd_queue to snd_buf
			LinkedListNode<Segment> node = this._sndQueue.First;
			while ( node != null )
			{
				Segment newseg = node.Value;
				LinkedListNode<Segment> next = node.Next;

				if ( _itimediff( this._sndNxt, this._sndUna + cwnd ) >= 0 )
					break;
				this._sndQueue.Remove( node );
				newseg.conv = this._conv;
				newseg.cmd = IKCP_CMD_PUSH;
				newseg.wnd = seg.wnd;
				newseg.ts = current;
				newseg.sn = this._sndNxt;
				newseg.una = this._rcvNxt;
				newseg.resendts = current;
				newseg.rto = this._rxRto;
				newseg.fastack = 0;
				newseg.xmit = 0;
				this._sndBuf.AddLast( node );
				node = next;
				this._sndNxt++;
			}

			// calculate resent
			uint resent = ( uint )this._fastresend;
			if ( this._fastresend <= 0 )
				resent = 0xffffffff;
			uint rtomin = this._rxRto >> 3;
			if ( this._nodelay != 0 )
				rtomin = 0;

			// flush data segments
			foreach ( Segment segment in this._sndBuf )
			{
				bool needsend = false;
				int debug = _itimediff( current, segment.resendts );
				if ( 0 == segment.xmit )
				{
					needsend = true;
					segment.xmit++;
					segment.rto = this._rxRto;
					segment.resendts = current + segment.rto + rtomin;
				}
				else if ( _itimediff( current, segment.resendts ) >= 0 )
				{
					needsend = true;
					segment.xmit++;
					if ( 0 == this._nodelay )
						segment.rto += this._rxRto;
					else
						segment.rto += this._rxRto / 2;
					segment.resendts = current + segment.rto;
					lost = 1;
				}
				else if ( segment.fastack >= resent )
				{
					needsend = true;
					segment.xmit++;
					segment.fastack = 0;
					segment.resendts = current + segment.rto;
					change++;
				}

				if ( needsend )
				{
					segment.ts = current;
					segment.wnd = seg.wnd;
					segment.una = this._rcvNxt;

					int need = IKCP_OVERHEAD + segment.data.Count;
					if ( offset + need >= this._mtu )
					{
						this.Output( this._buffer, offset );
						offset = 0;
					}

					offset += segment.Encode( this._buffer, offset );
					if ( segment.data.Count > 0 )
					{
						Buffer.BlockCopy( segment.data.Array, segment.data.Offset, this._buffer, offset, segment.data.Count );
						offset += segment.data.Count;
					}
				}
			}

			// flash remain segments
			if ( offset > 0 )
				this.Output( this._buffer, offset );

			// update ssthresh
			if ( change != 0 )
			{
				uint inflight = this._sndNxt - this._sndUna;
				this._ssthresh = inflight / 2;
				if ( this._ssthresh < IKCP_THRESH_MIN )
					this._ssthresh = IKCP_THRESH_MIN;
				this._cwnd = this._ssthresh + resent;
				this._incr = this._cwnd * this._mss;
			}

			if ( lost != 0 )
			{
				this._ssthresh = this._cwnd / 2;
				if ( this._ssthresh < IKCP_THRESH_MIN )
					this._ssthresh = IKCP_THRESH_MIN;
				this._cwnd = 1;
				this._incr = this._mss;
			}

			if ( this._cwnd < 1 )
			{
				this._cwnd = 1;
				this._incr = this._mss;
			}

			this.PushSegment( seg );
		}

		// update state (call it repeatedly, every 10ms-100ms), or you can ask
		// ikcp_check when to call it again (without ikcp_input/_send calling).
		// 'current' - current timestamp in millisec.
		public void Update( uint current )
		{
			this._current = current;

			if ( 0 == this._updated )
			{
				this._updated = 1;
				this._tsFlush = this._current;
			}

			int slap = _itimediff( this._current, this._tsFlush );
			if ( slap >= 10000 || slap < -10000 )
			{
				this._tsFlush = this._current;
				slap = 0;
			}

			if ( slap >= 0 )
			{
				if ( _itimediff( this._current, this._tsFlush ) >= 0 )
					this._tsFlush = this._current + this._interval;
				else
					this._tsFlush += this._interval;
				this.Flush();
			}
		}

		// Determine when should you invoke ikcp_update:
		// returns when you should invoke ikcp_update in millisec, if there
		// is no ikcp_input/_send calling. you can call ikcp_update in that
		// time, instead of call update repeatly.
		// Important to reduce unnacessary ikcp_update invoking. use it to
		// schedule ikcp_update (eg. implementing an epoll-like mechanism,
		// or optimize ikcp_update when handling massive kcp connections)
		public uint Check( uint current )
		{
			if ( 0 == this._updated )
				return current;

			uint tsFlush = this._tsFlush;
			int tmPacket = 0x7fffffff;

			if ( _itimediff( current, tsFlush ) >= 10000 ||
				 _itimediff( current, tsFlush ) < -10000 )
				tsFlush = current;

			if ( _itimediff( current, tsFlush ) >= 0 )
				return current;

			int tmFlush = _itimediff( tsFlush, current );

			foreach ( Segment seg in this._sndBuf )
			{
				int diff = _itimediff( seg.resendts, current );
				if ( diff <= 0 )
					return current;
				if ( diff < tmPacket )
					tmPacket = diff;
			}

			int minimal = tmPacket;

			if ( tmPacket >= tmFlush )
				minimal = tmFlush;

			if ( minimal >= this._interval )
				minimal = ( int )this._interval;

			return current + ( uint )minimal;
		}

		private int Output( byte[] data, int size )
		{
			return this._output( data, size, this._user );
		}

		//change MTU size, default is 1400
		public int SetMtu( int mtu )
		{
			if ( mtu < 50 || mtu < IKCP_OVERHEAD )
				return -1;

			this._mtu = ( uint )mtu;
			this._mss = this._mtu - IKCP_OVERHEAD;
			_bufferFree( this._buffer );
			this._buffer = _bufferAlloc( ( mtu + IKCP_OVERHEAD ) * 3 );
			return 0;
		}

		// fastest: ikcp_nodelay(kcp, 1, 20, 2, 1)
		// nodelay: 0:disable(default), 1:enable
		// interval: internal update timer interval in millisec, default is 100ms
		// resend: 0:disable fast resend(default), 1:enable fast resend
		// nc: 0:normal congestion control(default), 1:disable congestion control
		public int NoDelay( int nodelay, int interval, int resend, int nc )
		{
			if ( nodelay > 0 )
			{
				this._nodelay = ( uint )nodelay;
				this._rxMinrto = nodelay != 0 ? ( uint ) IKCP_RTO_NDL : ( uint ) IKCP_RTO_MIN;
			}

			if ( interval >= 0 )
			{
				if ( interval > 5000 )
					interval = 5000;
				else if ( interval < 1 )
					interval = 1;
				this._interval = ( uint )interval;
			}

			if ( resend >= 0 )
				this._fastresend = resend;

			if ( nc >= 0 )
				this._nocwnd = nc;

			return 0;
		}

		// set maximum window size: sndwnd=32, rcvwnd=32 by default
		public int WndSize( int sndwnd, int rcvwnd )
		{
			if ( sndwnd > 0 )
				this._sndWnd = ( uint )sndwnd;

			if ( rcvwnd > 0 )
				this._rcvWnd = ( uint )rcvwnd;
			return 0;
		}

		// get how many packet is waiting to be sent
		public int WaitSnd()
		{
			return this._sndBuf.Count + this._sndQueue.Count;
		}
	}
}