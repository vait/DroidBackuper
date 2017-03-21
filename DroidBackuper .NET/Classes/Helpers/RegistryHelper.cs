
using Microsoft.Win32;
using System;

namespace DroidBackuper.NET.Classes.Helpers
{
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
            exePath = new Uri(assembly.CodeBase).LocalPath;
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
