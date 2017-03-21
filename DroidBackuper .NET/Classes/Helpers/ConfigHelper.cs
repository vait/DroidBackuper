
using System;
using System.Configuration;

namespace DroidBackuper.NET.Classes.Helpers
{
    internal static class ConfigHelper
    {
        #region Bindings
        public static event EventHandler DeviceLogChanged;
        #endregion

        public static bool DeviceLog
        {
            get
            {
                return Configuration != null ? Configuration.DeviceLog : false;
            }
            set
            {
                if (Configuration.DeviceLog != value)
                {
                    Configuration.DeviceLog = value;
                    DeviceLogChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static AppConfiguration Configuration { get; private set; }

        static ConfigHelper()
        {
            var appSettings = ConfigurationManager.AppSettings;
            Configuration = new AppConfiguration();
            //Константы
            int startInterval = 1;
            if (int.TryParse(appSettings["StartInterval"], out startInterval)) {
                Configuration.StartInterval = startInterval;
            }

            bool deviceLog = false;
            if (Boolean.TryParse(appSettings["DeviceLog"], out deviceLog)) {
                Configuration.DeviceLog = deviceLog;
            }

            Configuration.DeviceManufacturer = appSettings["DeviceManufacturer"];
            Configuration.DeviceName = appSettings["DeviceName"];
            Configuration.Commands = appSettings["Commands"]?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            Configuration.Logger = new TextLogger();
        }

        internal static void Save()
        {
            var execfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            execfg.AppSettings.Settings.Remove("StartInterval");
            execfg.AppSettings.Settings.Add("StartInterval", Configuration.StartInterval.ToString());

            execfg.AppSettings.Settings.Remove("DeviceLog");
            execfg.AppSettings.Settings.Add("DeviceLog", Configuration.DeviceLog.ToString());

            execfg.AppSettings.Settings.Remove("DeviceManufacturer");
            execfg.AppSettings.Settings.Add("DeviceManufacturer", Configuration.DeviceManufacturer);

            execfg.AppSettings.Settings.Remove("DeviceName");
            execfg.AppSettings.Settings.Add("DeviceName", Configuration.DeviceName);

            execfg.AppSettings.Settings.Remove("Commands");
            execfg.AppSettings.Settings.Add("Commands", String.Join(";", Configuration.Commands));

            execfg.Save();
        }
    }
}
