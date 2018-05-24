using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Example.Misc;
using Example.Properties;
using RC.Core.Structure;

namespace Example
{
	public abstract class Base
	{
		protected bool _disposed;
		protected readonly SwitchQueue<string> _inputQueue = new SwitchQueue<string>();

		protected Base()
		{
			AssemblyName[] assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			foreach ( AssemblyName assembly in assemblies )
				Assembly.Load( assembly );

			LoggerProxy.Init( Resources.log4net_config );

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