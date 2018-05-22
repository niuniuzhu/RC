using RC.Core.Misc;
using RC.Core.Structure;
using RC.Game.Protocol;
using RC.Net;
using RC.Net.Protocol;
using System.Collections.Generic;
using System.Diagnostics;

namespace Example
{
	public class StepLocker
	{
		public delegate void ForeachUserHandler( ushort user );

		public const int FRAME_RATE = 50;//20ms/sec
		public const int FRAMES_PER_KEYFRAME = 5;//100ms/keyframe

		public string id { get; }
		public RemoteBattle owner { get; }

		public int userCount
		{
			get
			{
				lock ( this._lockObj )
				{
					return this._users.Count;
				}
			}
		}

		public int createdCount;
		public int endedCount;
		public bool finished;

		private int _frame;
		private long _elapsedSinceLastLogicUpdate;
		private readonly List<ushort> _users = new List<ushort>();
		private readonly SwitchQueue<_DTO_action_info> _actions = new SwitchQueue<_DTO_action_info>();
		private readonly Stopwatch _sw = new Stopwatch();
		private readonly object _lockObj = new object();

		public StepLocker( RemoteBattle owner )
		{
			this.id = GuidHash.GetString();
			this.owner = owner;
		}

		public void Start()
		{
			this._sw.Start();
		}

		public void AddUser( ushort userId )
		{
			lock ( this._lockObj )
			{
				this._users.Add( userId );
			}
		}

		public int RemoveUser( ushort userId )
		{
			lock ( this._lockObj )
			{
				this._users.Remove( userId );
				return this._users.Count;
			}
		}

		public bool HasUser( ushort userID )
		{
			lock ( this._lockObj )
				return this._users.Contains( userID );
		}

		public void ForeachUser( ForeachUserHandler handler )
		{
			lock ( this._lockObj )
			{
				int count = this._users.Count;
				for ( int i = 0; i < count; i++ )
					handler( this._users[i] );
			}
		}

		public void Update()
		{
			this._elapsedSinceLastLogicUpdate += this._sw.ElapsedMilliseconds;

			while ( this._elapsedSinceLastLogicUpdate >= 1000 / FRAME_RATE )
			{
				this._elapsedSinceLastLogicUpdate -= 1000 / FRAME_RATE;

				++this._frame;
				if ( ( this._frame % FRAMES_PER_KEYFRAME ) == 0 )
				{
					this.ProcessActions();
				}
			}

			this._sw.Restart();
		}

		private void ProcessActions()
		{
			this._actions.Switch();
			_DTO_action_info[] actions = new _DTO_action_info[this._actions.count];
			int i = 0;
			while ( !this._actions.isEmpty )
			{
				actions[i++] = this._actions.Pop();
			}
			this.Brocast( ProtocolManager.PACKET_BATTLE_SC_FRAME( actions, this._frame ) );
		}

		public void HandleAction( _DTO_frame_info dto )
		{
			int count = dto.actions.Length;
			for ( int i = 0; i < count; i++ )
				this._actions.Push( dto.actions[i] );
		}

		private void Brocast( Packet packet, IUserToken except = null )
		{
			lock ( this._lockObj )
			{
				this.owner.Brocast( this._users, packet, except );
			}
		}
	}
}