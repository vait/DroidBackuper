
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace DroidBackuper.NET.Classes.Helpers
{
	[SupportedOSPlatform("windows")]
	internal static class RegistryHelper
	{
		#region Bindings
		public static event EventHandler IsStartupChanged;
		#endregion

		private static readonly string appName;
		private static readonly string exePath;
		private static readonly string registryKey;

		static RegistryHelper()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			appName = assembly.GetName().Name;
			exePath = Process.GetCurrentProcess().MainModule.FileName;
			registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
		}

		public static bool IsStartup
		{
			get
			{
				RegistryKey runKey = Registry.CurrentUser.OpenSubKey(registryKey, false);
				return runKey.GetValue(appName) != null && String.Compare(runKey.GetValue(appName).ToString(), exePath, true) == 0;
			}
			set
			{
				IsStartupChanged?.Invoke(null, EventArgs.Empty);
			}
		}

		public static void SetStartup()
		{
			RegistryKey runKey = Registry.CurrentUser.OpenSubKey(registryKey, true);
			if (runKey.GetValue(appName) == null || runKey.GetValue(appName).ToString() != exePath)
			{
				runKey.DeleteValue(appName, false);
				runKey.SetValue(appName, exePath);
			}
		}

		public static void UnsetStartup()
		{
			RegistryKey runKey = Registry.CurrentUser.OpenSubKey(registryKey, true);

			runKey.DeleteValue(appName, false);
		}
	}
}
