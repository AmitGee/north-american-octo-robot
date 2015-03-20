using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsSectionHandler
{
	class Program
	{
		static void Main(string[] args)
		{
			//ConfigurationManager.s
			var v = ConfigurationManager.AppSettings["SettingKey"];
			Console.WriteLine(v);

			// override using proxy
			/*
			var o = ConfigurationManager.GetSection("proxyAppSettings");
			v = ConfigurationManager.AppSettings["SettingKey"];
			Console.WriteLine(v);
			 */

			// overide using code

			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			//if (ConfigurationManager.AppSettings.AllKeys.Contains("SettingKey"))
			//	ConfigurationManager.AppSettings["SettingKey"] = "NewValue";
			//else
			if (config.AppSettings.Settings.AllKeys.Contains("SettingKey"))
				config.AppSettings.Settings.Remove("SettingKey");
			config.AppSettings.Settings.Add("SettingKey", "NewValue");
			config.Save(ConfigurationSaveMode.Minimal);
			ConfigurationManager.RefreshSection("appSettings");
			v = ConfigurationManager.AppSettings["SettingKey"];
			Console.WriteLine(v);

		}
	}

	public class CustomAppSettingsHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			var n = new NameValueConfigurationCollection();
			n.Add(new NameValueConfigurationElement("SettingKey", "NewValue"));
			return n;
		}
	}
}
