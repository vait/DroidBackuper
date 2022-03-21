using DroidBackuper.NET.Classes.Helpers;
using DroidBackuper.NET.Classes.Model;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows;

namespace DroidBackuper.NET
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly Mutex _onceInstanceMutex = new Mutex(true, "{F79FD3D4-3977-4FD1-9B25-8B7D026C17CB}");
		private TaskbarIcon _tb;
		private EventHooker _eventHooker;

		[SupportedOSPlatform("windows")]
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (!_onceInstanceMutex.WaitOne(TimeSpan.Zero, true))
			{
				_onceInstanceMutex.Dispose();
				Shutdown(0);
				return;
			}

			_tb = (TaskbarIcon)FindResource("BackuperTrayIcon");

			_eventHooker = new EventHooker(ConfigHelper.Configuration);
		}

		[SupportedOSPlatform("windows7.0")]
		protected override void OnExit(ExitEventArgs e)
		{
			_tb?.Dispose(); //the icon would clean up automatically, but this is cleaner
			_onceInstanceMutex?.ReleaseMutex();
			_onceInstanceMutex?.Dispose();
			_eventHooker?.Dispose();
			base.OnExit(e);
		}
	}
}
