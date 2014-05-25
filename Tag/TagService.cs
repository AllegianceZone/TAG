using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// The Tag Windows Service
	/// </summary>
	public class TagService : System.ServiceProcess.ServiceBase
	{
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public TagService ()
		{
			InitializeComponent();
		}

		/// <summary>
		/// The main entry point for the service
		/// </summary>
		public static void StartService ()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new TagService() };
			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			components = new System.ComponentModel.Container();
			this.ServiceName = "TagService";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart (string[] args)
		{
			Tag.Start();
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop ()
		{
			Tag.Stop();
		}
	}
}
