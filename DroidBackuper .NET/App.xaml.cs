using DroidBackuper.NET.Classes.Helpers;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Threading;
using System.Windows;

namespace DroidBackuper.NET
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly Mutex onceInstanceMutex = new Mutex(true, "{F79FD3D4-3977-4FD1-9B25-8B7D026C17CB}");
		private TaskbarIcon tb;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (!onceInstanceMutex.WaitOne(TimeSpan.Zero, true))
			{
				onceInstanceMutex.Dispose();
				Shutdown(0);
				return;
			}

			tb = (TaskbarIcon)FindResource("BackuperTrayIcon");

			var evt = new Classes.Model.EventHooker(ConfigHelper.Configuration);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			if (tb != null)
			{
				tb.Dispose(); //the icon would clean up automatically, but this is cleaner
			}
			onceInstanceMutex?.ReleaseMutex();
			onceInstanceMutex?.Dispose();
			base.OnExit(e);
		}
	}
}
