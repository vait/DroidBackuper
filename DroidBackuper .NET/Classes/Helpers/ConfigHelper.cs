using DroidBackuper.NET.Classes.Model;
using System;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DroidBackuper.NET.Classes.Helpers
{
	internal static class ConfigHelper
	{
		private static readonly JsonSerializerOptions options = new JsonSerializerOptions
		{
			WriteIndented = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			PropertyNameCaseInsensitive = true,
		};

		private static readonly string _oldFilPath;
		private static readonly string _newFilPath;


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
			var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			_oldFilPath = exePath + ".config";
			_newFilPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "appsettings.json");

			Configuration = Load();
		}

		internal static void Save()
		{
			var settings = Configuration.ToAppConfiguration();
			Save(settings);
		}

		internal static void Save(Settings settings)
		{
			var a = JsonSerializer.Serialize(settings, options);
			File.WriteAllText(_newFilPath, a);
		}

		private static AppConfiguration Load()
		{
			CopyIfNeed();

			using var fs = new FileStream(_newFilPath, FileMode.Open, FileAccess.Read);
			var settings = JsonSerializer.Deserialize<Settings>(fs, options);
			var appConfig = settings.ToAppConfiguration();
			appConfig.Logger = new TextLogger();

			return appConfig;
		}

		private static void CopyIfNeed()
		{
			Settings settings;

			if (File.Exists(_oldFilPath))
			{
				settings = new Settings();
				var appSettings = ConfigurationManager.AppSettings;
				//Константы
				int startInterval = 1;
				if (int.TryParse(appSettings["StartInterval"], out startInterval))
				{
					settings.StartInterval = TimeSpan.FromSeconds(startInterval);
				}

				if (Boolean.TryParse(appSettings["DeviceLog"], out var deviceLog))
				{
					settings.DeviceLog = deviceLog;
				}

				settings.DeviceManufacturer = appSettings["DeviceManufacturer"];
				settings.DeviceName = appSettings["DeviceName"];
				settings.Commands = appSettings["Commands"];

				Save(settings);

				File.Delete(_oldFilPath);
			}
		}

		private static AppConfiguration ToAppConfiguration(this Settings settings)
		{
			var appConfig = new AppConfiguration();
			appConfig.StartInterval = settings.StartInterval;
			appConfig.DeviceLog = settings.DeviceLog;
			appConfig.DeviceManufacturer = settings.DeviceManufacturer;
			appConfig.DeviceName = settings.DeviceName;
			appConfig.Commands = settings.Commands?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

			return appConfig;
		}

		private static Settings ToAppConfiguration(this AppConfiguration appConfig)
		{
			var settings = new Settings();
			settings.StartInterval = appConfig.StartInterval;
			settings.DeviceLog = appConfig.DeviceLog;
			settings.DeviceManufacturer = appConfig.DeviceManufacturer;
			settings.DeviceName = appConfig.DeviceName;
			settings.Commands = (appConfig.Commands != null && appConfig.Commands.Length > 0) ? string.Join(';', appConfig.Commands) : null;

			return settings;
		}
	}
}
