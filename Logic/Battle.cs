using RC.Core.Misc;
using RC.Core.Structure;
using RC.Game.Core;
using RC.Game.Protocol;
using RC.Net;

namespace RC.Game.Logic
{
	public sealed class Battle : IBattle
	{
		public int frame { get; private set; }
		public long deltaTime { get; private set; }
		public long time { get; private set; }
		public EntityManager entityManager { get; }
		public INetServer transmitter { get; }

		private readonly UpdateContext _context;
		private readonly int _msPerFrame;
		private readonly int _framesPerKeyFrame;
		private long _lastElapsed;
		private int _nextKeyFrame;
		private static readonly SwitchQueue<_DTO_frame_info> SERVER_KEYFRAMES = new SwitchQueue<_DTO_frame_info>();

		public Battle( int frameRate, int framesPerKeyFrame, INetServer transmitter )
		{
			this._framesPerKeyFrame = framesPerKeyFrame;
			this._msPerFrame = 1000 / frameRate;
			this._lastElapsed = 0;
			this._nextKeyFrame = this._framesPerKeyFrame;

			this.transmitter = transmitter;
			this._context = new UpdateContext();
			this.entityManager = new EntityManager( this );
		}

		public void Dispose()
		{
			this.entityManager.Dispose();
		}

		public void ProcessServerKeyFrame( _DTO_frame_info dto )
		{
			SERVER_KEYFRAMES.Push( dto );
		}

		public void Update( long dt )
		{
			//如果本地frame比服务端慢，则需要快速步进追赶服务端帧数
			SERVER_KEYFRAMES.Switch();
			while ( !SERVER_KEYFRAMES.isEmpty )
			{
				_DTO_frame_info dto = SERVER_KEYFRAMES.Pop();
				int length = dto.frameId - this.frame;
				while ( length >= 0 )
				{
					if ( length == 0 )
						this.HandleAction( dto );
					else
					{
						this.Simulate( this._msPerFrame );
					}
					--length;
				}

				this._nextKeyFrame = dto.frameId + this._framesPerKeyFrame;
			}

			if ( this.frame < this._nextKeyFrame )
			{
				this._lastElapsed += dt;

				while ( this._lastElapsed >= this._msPerFrame )
				{
					if ( this.frame >= this._nextKeyFrame )
						break;

					this.Simulate( this._msPerFrame );

					if ( this.frame == this._nextKeyFrame )
						this.transmitter.SendAll( ProtocolManager.PACKET_LV_BATTLE_KEYFRAME( this.frame ) );

					this._lastElapsed -= this._msPerFrame;
				}
			}
		}

		private void HandleAction( _DTO_frame_info dto )
		{
			//int count = dto.actions.Length;
			//for ( int i = 0; i < count; i++ )
			//{
			//	_DTO_action_info action = dto.actions[i];
			//}
		}

		private void Simulate( long fdt )
		{
			++this.frame;

			this.deltaTime = fdt;
			this.time += this.deltaTime;

			this._context.deltaTime = this.deltaTime;
			this._context.time = this.time;
			this._context.frame = this.frame;

			this.entityManager.Update( this._context );
			this.transmitter.Update( this.deltaTime );

			Logger.Log( this.frame );
		}
	}
}