using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Provides runtime logging/tracing services
	/// </summary>
	public class TagTrace
	{
		private static TraceSwitch	_switch = null;					// The switch controlling TAG's current TraceLevel
		private static bool			_isInitialized = false;			// Specifies whether Tracing is initialized or not
		private static string		_archiveDir = string.Empty;		// The path where tracefiles are archived (if not null)
		private static string		_tracePath = string.Empty;		// The path of the tracefile (if not null)
		private static DateTime		_lastTraceTime = DateTime.Now;	// When archiving, stores the last time a Trace message was written

		/// <summary>
		/// Initializes tracing with the specified settings
		/// </summary>
		/// <param name="initialTraceLevel">The initial level of Tracing of the application</param>
		/// <param name="tracePath">The fully-qualified path (if not null) where the tracefile will be written</param>
		/// <param name="archiveDir">The folder (if not null) where tracefiles will be archived each day</param>
		/// <param name="traceConsole">Specifies whether or not to show messages in realtime</param>
		/// <param name="traceTextbox">Specifies which textbox to write Trace messages to (if not null)</param>
		public static void Initialize (TraceLevel initialTraceLevel, string tracePath, string archiveDir, 
										bool traceConsole, TextBox traceTextbox)
		{
			// Create TraceSwitch
			_switch = new TraceSwitch("TagTrace", "Controls the level of trace output by TAG");
			_switch.Level = initialTraceLevel;

			_tracePath = tracePath;
			_archiveDir = archiveDir;
			
			// If they're tracing to a textfile, initialize it
			if (_tracePath != null)
				Trace.Listeners.Add(new TextWriterTraceListener(_tracePath, "TAG Trace File"));

			// If the user doesn't want to see the traces as they happen, remove console.out
			Trace.Listeners.Remove("Default");
			if (traceConsole)
				Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

			// No exception, so we're initialized!
			_isInitialized = true;
		}

		/// <summary>
		/// Stops all trace output and releases resources
		/// </summary>
		public static void Uninitialize ()
		{
			_isInitialized = false;

			// Shut down tracing
			foreach (TraceListener Listener in Trace.Listeners)
				Listener.Close();

			Trace.Listeners.Clear();
		}

		/// <summary>
		/// Writes the specified text with the specified formatting to the trace file
		/// </summary>
		/// <param name="traceLevel">The Level at which this line should be written</param>
		/// <param name="format">The format string for the line to be written</param>
		/// <param name="objects">The objects to format before being written</param>
		public static void WriteLine (TraceLevel traceLevel, string format, params object[] objects)
		{
			// If Tracing isn't initialized, we can't write trace messages!
			if (_isInitialized == false)
				return;

			// If this trace message is too specific, exit.
			if (_switch.Level < traceLevel)
				return;

			string Text = string.Format(format, objects);
			WriteLine(traceLevel, Text);
		}

		/// <summary>
		/// Writes the specified text to the trace file
		/// </summary>
		/// <param name="traceLevel">The Level at which this line should be written</param>
		/// <param name="text">The text to be written</param>
		public static void WriteLine (TraceLevel traceLevel, string text)
		{
			// If Tracing isn't initialized, we can't write trace messages!
			if (_isInitialized == false)
				return;

			// If this trace message is too specific, exit.
			if (_switch.Level < traceLevel)
				return;

			// Prepare "Time: " header
			DateTime Now = DateTime.Now;
			string Line = string.Concat(Now.ToShortDateString(), " ", Now.ToLongTimeString(), ": ", text);

			// If we're archiving, check to see if we need to
			if (_archiveDir != null)
			{
				// If it's "tomorrow", Archive it!
				if (Now.Day != _lastTraceTime.Day)
					ArchiveTracefile();

				// Remember the last trace message for the next archive check
				_lastTraceTime = Now;
			}

			// Write the Trace and flush the buffer
			Trace.WriteLine(Line);
			Trace.Flush();
		}

		/// <summary>
		/// Archives the TraceFile according to the archive settings
		/// </summary>
		private static void ArchiveTracefile ()
		{
			// If we're not logging and not archiving, quit now
			if (_archiveDir == null || _tracePath == null)
				return;

			try
			{
				// Get the path of the new file
				string NewFileName = string.Format("Trace_{0}.{1:00}.{2:00}.txt", _lastTraceTime.Year, _lastTraceTime.Month, _lastTraceTime.Day);
				string Destination = string.Concat(_archiveDir, "\\", NewFileName);

				// Create the new folder if needed
				if (!Directory.Exists(_archiveDir))
					Directory.CreateDirectory(_archiveDir);

				// Remove and close the listener so the file is not held
				TraceListener Listener = Trace.Listeners["TAG Trace File"];
				if (Listener != null)
				{
					Trace.Listeners.Remove(Listener);
					Listener.Close();
				}

				// Move the file
				File.Move(_tracePath, Destination);
			
				// Re-add new listener to continue tracing
				Trace.Listeners.Add(new TextWriterTraceListener(_tracePath, "TAG Trace File"));
			}
			catch (Exception e)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error archiving trace file: {0}", e.Message);
			}
		}
	}
}
