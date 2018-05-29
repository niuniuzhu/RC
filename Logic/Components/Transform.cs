using RC.Core.FMath;
using RC.Game.Core;
using RC.Game.Protocol.LV;

namespace RC.Game.Logic.Components
{
	public class Transform : Component
	{
		private FVec3 _position;
		private FQuat _rotation;

		public FVec3 position
		{
			get => this._position;
			set
			{
				if ( this._position == value )
					return;
				this._position = value;
				this.OnPositionChanged();
			}
		}

		public FQuat rotation
		{
			get => this._rotation;
			set
			{
				if ( this._rotation == value )
					return;
				this._rotation = value;
				this.OnRotationChanged();
			}
		}

		private void OnPositionChanged()
		{
		}

		private void OnRotationChanged()
		{
		}

		protected override void OnSynchronize()
		{
			this.owner.battle.transmitter.SendAll( LVProtoMgr._PACKET_BATTLE_TRANSFORM(
													   ( float )this.position.x, ( float )this.position.y,
													   ( float )this.position.z,
													   ( float )this.rotation.x, ( float )this.rotation.y,
													   ( float )this.rotation.z ) );
		}
	}
}