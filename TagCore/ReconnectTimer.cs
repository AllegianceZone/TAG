using System;
using System.Diagnostics;
using System.Threading;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Handles TAG's reconnection attempts to Allsrv
	/// </summary>
	public class ReconnectTimer
	{
		private static int		_interval = 60;
		private static int		_retryCount = 0;
		private static int		_maxRetries = 60;

		private static Timer	_reconnectTimer;

		/// <summary>
		/// Initializes the Reconnect timer
		/// </summary>
		/// <param name="interval">The interval between reconnect attempts</param>
		/// <param name="maxRetries">The maximum number of reconnection attempts before TAG shuts down</param>
		public static void Initialize (int interval, int maxRetries)
		{
			_interval = interval;
			_maxRetries = maxRetries;
		}

		/// <summary>
		/// Starts the Reconnect timer with the initalized interval
		/// </summary>
		public static void Start ()
		{
			_retryCount = 0;
			int Interval = _interval * 1000;
			_reconnectTimer = new Timer(new TimerCallback(AttemptReconnect), null, Interval, Interval);
		}

		/// <summary>
		/// Stops the Reconnect timer
		/// </summary>
		public static void Stop ()
		{
			_reconnectTimer.Dispose();
		}

		/// <summary>
		/// Attempts a reconnection with Allsrv
		/// </summary>
		/// <param name="state">Data asynchronously passed to this method</param>
		private static void AttemptReconnect (object state)
		{
			try
			{
				_retryCount++;
				if (_retryCount > _maxRetries)
				{
					TagTrace.WriteLine(TraceLevel.Info, "TAG has reached the maximum amount of Allsrv reconnect attempts. Shutting down...");
					Stop();
					if (TagServiceController.IsTagServiceStarted())
						TagServiceController.StopTagService();
					else
						ShutdownTagEvent(null);

					return;
				}

				TagTrace.WriteLine(TraceLevel.Verbose, "Attempting to reconnect to Allsrv...");
				GameServer.Initialize();

				// If it worked, kill the reconnect timer!
				TagTrace.WriteLine(TraceLevel.Info, "Allsrv reconnection successful.");
				Stop();
			}
			catch (Exception)
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "Allsrv reconnection failed. Will retry in {0} seconds", _interval);
			}
		}

		public static event AsyncCallback ShutdownTagEvent;
	}
}
