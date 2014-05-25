using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Encapsulates TAG's configuration settings
	/// </summary>
	public class TagConfig
	{

		/// <summary>
		/// The default URL for ACSS's services
		/// </summary>
		public const string DEFAULTCSSURL = "http://asgs.alleg.net/css/tag.svc";

		/// <summary>
		/// The default URL for ASGS's services
		/// </summary>
		public const string DEFAULTASGSURL = "http://asgs.alleg.net/asgs/services.asmx";

		public const int	DEFAULTTIMEOUT = 180000;	// 3 Minutes

		// TAG
		private DateTime	_updateTime = DateTime.Now;
		private Boolean		_skipUpdates = false;
		private string		_asgsUrl = DEFAULTASGSURL;
		private string		_cssUrl = DEFAULTCSSURL;
		private int			_postTimeout = DEFAULTTIMEOUT;
		private bool		_useCss = false;
		

		// Reconnect
		private int			_reconnectIntervalSeconds = 60;
		private int			_maxRetries = 60;

		// Logging
		private string		_xmlPath = null;

		// Trace
		private TraceLevel	_traceLevel = TraceLevel.Off;
		private string		_tracePath = null;
		private string		_traceArchiveDir = null;
		private bool		_traceConsole = false;

		// ServerAdmins
		private ArrayList	_serverAdmins = new ArrayList();

		/// <summary>
		/// Default hidden constructor
		/// </summary>
		private TagConfig () {}

		/// <summary>
		/// Parses Xml configuration data into an instance of TagConfig
		/// </summary>
		/// <param name="root">The root Xml node of the configuration section</param>
		public static TagConfig ParseXml (XmlNode root)
		{
			TagConfig Result = new TagConfig();

			try
			{
				foreach (XmlNode Node in root.ChildNodes)
				{
					if (!(Node is XmlElement))
						continue;

					XmlElement Element = (XmlElement)Node;

					switch (Element.Name)
					{
						case "TAG":
							if (Element.HasAttribute("UpdateTime"))
							{
								string Time = Element.GetAttribute("UpdateTime");	
								DateTime TempTime = DateTime.Parse(Time);
								
								// Add a day if the update time has already passed
								if (DateTime.Now > TempTime)
									TempTime = TempTime.AddDays(1);

								Result._updateTime = TempTime;
							}

							if (Element.HasAttribute("skipUpdates"))
								Result._skipUpdates = Boolean.Parse(Element.GetAttribute("skipUpdates"));

							if (Element.HasAttribute("ASGSUrl"))
								Result._asgsUrl = Element.GetAttribute("ASGSUrl");

							if (Element.HasAttribute("CSSUrl"))
								Result._cssUrl = Element.GetAttribute("CSSUrl");

							if (Element.HasAttribute("useCss"))
								Result._useCss = Boolean.Parse(Element.GetAttribute("useCss"));

							if (Element.HasAttribute("PostTimeout"))
								Result._postTimeout = int.Parse(Element.GetAttribute("PostTimeout"));

							break;
						case "ReconnectTimer":
							if (Element.HasAttribute("Interval"))
							{
								string SecondsString = Element.GetAttribute("Interval");
								Result._reconnectIntervalSeconds = int.Parse(SecondsString);
							}
							if (Element.HasAttribute("MaxRetries"))
								Result._maxRetries = int.Parse(Element.GetAttribute("MaxRetries"));

							break;
						case "Log":
							foreach (XmlNode SubNode in Node.ChildNodes)
							{
								if (!(SubNode is XmlElement))
									continue;

								XmlElement SubElement = (XmlElement) SubNode;

								switch (SubElement.Name)
								{
									case "XmlFile":
										if (SubElement.HasAttribute("Path"))
										{
											string TempPath = SubElement.GetAttribute("Path");
											Result._xmlPath = (Path.IsPathRooted(TempPath)) ? TempPath : Application.StartupPath + "\\" + TempPath;
										}
										break;
								}
							}
							break;
						case "Trace":
							if (Element.HasAttribute("Level"))
								Result._traceLevel = (TraceLevel)Enum.Parse(typeof(TraceLevel), Element.GetAttribute("Level"));
							if (Element.HasAttribute("Path"))
							{
								string TempPath = Element.GetAttribute("Path");
								Result._tracePath = (Path.IsPathRooted(TempPath)) ? TempPath : Application.StartupPath + "\\" + TempPath;
							}
							if (Element.HasAttribute("ArchiveDir"))
							{
								string TempPath = Element.GetAttribute("ArchiveDir");
								Result._traceArchiveDir = (Path.IsPathRooted(TempPath)) ? TempPath : Application.StartupPath + "\\" + TempPath;
							}
							if (Element.HasAttribute("Console"))
								Result._traceConsole = bool.Parse(Element.GetAttribute("Console"));

							break;
						case "ServerAdmins":
							foreach (XmlNode SubNode in Node.ChildNodes)
							{
								if (!(SubNode is XmlElement))
									continue;

								XmlElement SubElement = (XmlElement) SubNode;

								switch (SubElement.Name)
								{
									case "ServerAdmin":
										Result._serverAdmins.Add(SubElement.InnerText.ToLower());
										break;
								}
							}
							break;
					}
				}
			}
			catch (Exception e)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error parsing Xml Config File: {0}", e.Message);
			}

			return Result;
		}

		/// <summary>
		/// Saves the specified configuration settings to the specified path
		/// </summary>
		public void Save (string path)
		{
			// TODO: Save configuration to specified path
		}

		#region Properties

		/// <summary>
		/// The date/time of the next Update check
		/// </summary>
		public DateTime UpdateTime
		{
			get {return _updateTime;}
			set {_updateTime = value;}
		}

		/// <summary>
		/// When set to true, TAG will not get updates from the central source. 
		/// </summary>
		public Boolean SkipUpdates
		{
			get { return _skipUpdates; }
			set { _skipUpdates = value; }
		}

		/// <summary>
		/// The Url to the ASGS web services
		/// </summary>
		public string AsgsUrl
		{
			get {return _asgsUrl;}
			set {_asgsUrl = value;}
		}

		/// <summary>
		/// The Url to the CSS web services
		/// </summary>
		public string CssUrl
		{
			get { return _cssUrl; }
			set { _cssUrl = value; }
		}

		/// <summary>
		/// If this flag is set, then stats are delivered to CSS instead of to ASGS.
		/// </summary>
		public bool UseCss
		{
			get { return _useCss; }
			set { _useCss = value; }
		}

		/// <summary>
		/// The timeout value (in milliseconds) for ASGS posts
		/// </summary>
		public int PostTimeout
		{
			get {return _postTimeout;}
			set {_postTimeout = value;}
		}

		/// <summary>
		/// The amount of seconds between reconnect attempts
		/// </summary>
		public int ReconnectInterval
		{
			get {return _reconnectIntervalSeconds;}
			set {_reconnectIntervalSeconds = value;}
		}

		/// <summary>
		/// The maximum amount of reconnects to attempt before stopping TAG
		/// </summary>
		public int MaxRetries
		{
			get {return _maxRetries;}
			set {_maxRetries = value;}
		}

		/// <summary>
		/// The path to a folder where Xml Gamelogs are stored, or null for no Xml gamelogging
		/// </summary>
		public string XmlPath
		{
			get {return _xmlPath;}
			set {_xmlPath = value;}
		}

		/// <summary>
		/// The level of tracing to be output
		/// </summary>
		public TraceLevel TraceLevel
		{
			get {return _traceLevel;}
			set {_traceLevel = value;}
		}

		/// <summary>
		/// The path to a file where Trace messages should be written, or null for no tracelog
		/// </summary>
		public string TracePath
		{
			get {return _tracePath;}
			set {_tracePath = value;}
		}

		/// <summary>
		/// The path to a folder where tracefiles are archived, or null for no archiving
		/// </summary>
		public string TraceArchiveDir
		{
			get {return _traceArchiveDir;}
			set {_traceArchiveDir = value;}
		}

		/// <summary>
		/// Indicates whether or not tracing should be output to the current console window
		/// </summary>
		public bool TraceConsole
		{
			get {return _traceConsole;}
			set {_traceConsole = value;}
		}

		/// <summary>
		/// A list of callsigns who will be treated as administrators even without an administrative token or tag
		/// </summary>
		public ArrayList ServerAdmins
		{
			get {return _serverAdmins;}
			set {_serverAdmins = value;}
		}
		#endregion
	}
}
