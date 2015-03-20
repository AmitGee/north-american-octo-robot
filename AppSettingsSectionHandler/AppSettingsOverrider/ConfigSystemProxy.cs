using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Internal;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsOverrider
{
	public class ConfigSystemProxySectionHandler : IConfigurationSectionHandler
	{
		static ConfigSystemProxySectionHandler()
		{
			ConfigSystemProxy.SetProxy();
		}
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			return null;
		}
	}
	public class ConfigSystemProxy : IInternalConfigSystem
	{
		private static IInternalConfigSystem _baseConfigSystem;
		private static bool _initialised = false;
		private static readonly object _sync = new object();

		private object _appSettings;
 
		public ConfigSystemProxy(IInternalConfigSystem baseConfigSystem)
		{
			_baseConfigSystem = baseConfigSystem;
		}

		public object GetSection(string configKey)
		{
			if (configKey == "appSettings")
			{
				if (_appSettings != null) return _appSettings;
				lock (_sync)
				{
					if (_appSettings != null) return _appSettings;

					var baseAppSettings = _baseConfigSystem.GetSection(configKey);
					var baseAppSettingsNameValueCollection = baseAppSettings as NameValueCollection;
					if (baseAppSettingsNameValueCollection == null)
					{
						_appSettings = baseAppSettings;
					}
					else
					{
						var newSettings = new NameValueCollection(baseAppSettingsNameValueCollection);

						// override 
						newSettings["SettingKey"] = "NewValue";
						_appSettings = newSettings;
					}
					return _appSettings;
				}

			}
			return _baseConfigSystem.GetSection(configKey); 
		}

		public void RefreshConfig(string sectionName)
		{
			if (sectionName == "appSettings")
			{
				lock (_sync)
					_appSettings = null;
			}
			_baseConfigSystem.RefreshConfig(sectionName);
		}

		public bool SupportsUserConfig
		{
			get { return _baseConfigSystem.SupportsUserConfig; }
		}

		public static void SetProxy()
		{
			if (_initialised) return;
			lock (_sync)
			{
				if (_initialised) return;

				var o = ConfigurationManager.AppSettings;
				var field = typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.Static | BindingFlags.NonPublic);
				if (field == null)
					throw new Exception("Oops - System.Configuration.ConfigurationManager doesn't have a s_configSystem field anymore");
				field.SetValue(null, new ConfigSystemProxy((IInternalConfigSystem)field.GetValue(null)));
			}
		}
	}
}
