using System;
using System.ServiceProcess;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Controls the TAG and Allsrv services
	/// </summary>
	public class TagServiceController
	{
		public const string ALLSRVNAME = "allsrv";
		public const string ALLSRVNAME32 = "allsrv32";
		public const string TAGSERVICENAME = "TAGService";

		private static ServiceController _allsrvServiceController = null;
		private static ServiceController _tagServiceController = null; // The controller for TAG in service mode, or null if in console mode

		/// <summary>
		/// Checks to see if the Allsrv service is started (if instlled)
		/// </summary>
		/// <returns>True if Allsrv is started in service mode, false if not</returns>
		public static bool IsAllsrvServiceStarted ()
		{
			bool Result = false;

			if (AllsrvService != null)
				Result = (_allsrvServiceController.Status == ServiceControllerStatus.Running);

			return Result;
		}
		
		/// <summary>
		/// Checks to see if the TAG service is started (if instlled)
		/// </summary>
		/// <returns>True if TAG is started in service mode, false if not</returns>
		public static bool IsTagServiceStarted ()
		{
			bool Result = false;

			if (TagService != null)
				Result = (_tagServiceController.Status == ServiceControllerStatus.Running);

			return Result;
		}

		/// <summary>
		/// Starts the Allsrv Service (if installed)
		/// </summary>
		public static void StartAllsrvService ()
		{
			if (AllsrvService != null)
				_allsrvServiceController.Start();
		}

		/// <summary>
		/// Stops the Allsrv Service (if installed)
		/// </summary>
		public static void StopAllsrvService ()
		{
			if (AllsrvService != null)
				_allsrvServiceController.Stop();
		}

		/// <summary>
		/// Starts the TAG Service (if installed)
		/// </summary>
		public static void StartTagService ()
		{
			if (TagService != null)
				_tagServiceController.Start();
		}

		/// <summary>
		/// Stops the TAG Service (if installed)
		/// </summary>
		public static void StopTagService ()
		{
			if (TagService != null)
				_tagServiceController.Stop();
		}

		/// <summary>
		/// Retrieves the Allsrv service, looking it up if necessary
		/// </summary>
		public static ServiceController AllsrvService
		{
			get
			{
				// If we already know Allsrv is a service, don't bother checking
				if (_allsrvServiceController != null)
					return _allsrvServiceController;

				_allsrvServiceController = LookupAllsrvService();

				return _allsrvServiceController;
			}
		}

		/// <summary>
		/// Looks up the Allsrv Service
		/// </summary>
		/// <returns>The Allsrv servicecontroller (if found)</returns>
		public static ServiceController LookupAllsrvService ()
		{
			ServiceController Result = null;

			// Get the Allsrv service
			Result = new ServiceController(ALLSRVNAME);

			if (Result != null)
			{
				try
				{
					string Test = Result.ServiceName;
				}
				catch (Exception)
				{
					Result = null;
				}
			}

			// Allsrv not found. Get the allsrv32 service
			if (Result == null)
			{
				Result = new ServiceController(ALLSRVNAME32);
				
				if (Result != null)
				{
					try
					{
						string Test = Result.ServiceName;
					}
					catch (Exception)
					{
						Result = null;
					}
				}
			}
			

			return Result;
		}

		/// <summary>
		/// Looks up the TAG Service
		/// </summary>
		/// <returns>The TAG servicecontroller (if found)</returns>
		public static ServiceController LookupTagService ()
		{
			ServiceController Result = null;

			// Get the TAG service
			Result = new ServiceController(TAGSERVICENAME);

			if (Result != null)
			{
				try
				{
					string Test = Result.ServiceName;
				}
				catch (Exception)
				{
					Result = null;
				}
			}

			return Result;
		}

		/// <summary>
		/// Retrieves the name of the Allsrv Service
		/// </summary>
		public static string AllsrvServiceName
		{
			get
			{
				string Result = null;

				if (_allsrvServiceController != null)
					Result = _allsrvServiceController.ServiceName;

				return Result;
			}
		}

		/// <summary>
		/// Retrieves the TAG service, looking it up if necessary
		/// </summary>
		public static ServiceController TagService
		{
			get
			{
				// If we already know TAG is a service, don't bother checking
				if (_tagServiceController != null)
					return _tagServiceController;

				_tagServiceController = LookupTagService();

				return _tagServiceController;
			}
		}

		/// <summary>
		/// Retrieves the name of the TAG Service
		/// </summary>
		public static string TagServiceName
		{
			get
			{
				string Result = null;

				if (_tagServiceController != null)
					Result = _tagServiceController.ServiceName;

				return Result;
			}
		}
	}
}
