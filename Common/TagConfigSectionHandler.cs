using System;
using System.Xml;
using System.Configuration;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Parses the TagConfig configuration file section into an TagConfig object
	/// </summary>
	public class TagConfigSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public TagConfigSectionHandler () {}

		#region IConfigurationSectionHandler Members
		/// <summary>
		/// Creates the TagConfig object
		/// </summary>
		/// <param name="parent">The parent node of the configuration file</param>
		/// <param name="configContext">The context of the node</param>
		/// <param name="section">The root node of the config file section being parsed</param>
		/// <returns>An TagConfig object encapsulating all settings defined in the specified configuration file section</returns>
		public object Create (object parent, object configContext, XmlNode section)
		{
			XmlElement root = (XmlElement)section;
  
			TagConfig Config = TagConfig.ParseXml(root);

			return Config;
		}
		#endregion
	}
}
