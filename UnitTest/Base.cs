using RC.Core.Structure;
using System;
using System.Diagnostics;
using System.Threading;

namespace UnitTest
{
	public abstract class Base
	{
		private bool _disposed;
		private readonly SwitchQueue<string> _inputQueue = new SwitchQueue<string>();

		protected Base()
		{
			Thread inputThread = new Thread( this.InputWorker );
			inputThread.IsBackground = true;
			inputThread.Start();
		}

		protected virtual void Dispose()
		{
			this._disposed = true;
		}

		protected void StartLoopCycle( int sleepMs )
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			long realCost = 0;
			long lastElapsed = 0;
			while ( !this._disposed )
			{
				this.OnUpdate( realCost );
				this.ProcessInput();
				Thread.Sleep( sleepMs );
				long elapsed = sw.ElapsedMilliseconds;
				realCost = elapsed - lastElapsed;
				lastElapsed = elapsed;
			}
		}

		private void InputWorker()
		{
			while ( !this._disposed )
			{
				string cmd = Console.ReadLine();
				this._inputQueue.Push( cmd );
				Thread.Sleep( 10 );
			}
		}

		private void ProcessInput()
		{
			this._inputQueue.Switch();
			while ( !this._inputQueue.isEmpty )
			{
				string cmd = this._inputQueue.Pop();
				this.OnInput( cmd );
			}
		}

		protected virtual void OnUpdate( long dt )
		{
		}

		protected virtual void OnInput( string cmd )
		{
		}
	}
}