using RC.Core.FMath;

namespace RC.Game.Components
{
	public class Transform : Component
	{
		private FVec3 _position;
		private FQuat _rotation;

		[Synchronize( 0 )]
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

		[Synchronize( 1 )]
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
	}
}