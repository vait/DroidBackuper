using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using DroidBackuper.NET.Classes.Helpers;
using System.Threading;
using System;

namespace DroidBackuper.NET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon tb;

        private Classes.Model.EventHooker evt;

        private Mutex onceInstanceMutex = new Mutex(true, "{F79FD3D4-3977-4FD1-9B25-8B7D026C17CB}");

        private bool isFirstInstance = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!onceInstanceMutex.WaitOne(TimeSpan.Zero, true))
            {
                this.Shutdown(0);
                return;
            }

            isFirstInstance = true;

            //initialize NotifyIcon
            tb = (TaskbarIcon)FindResource("BackuperTrayIcon");

            var evt = new Classes.Model.EventHooker(ConfigHelper.Configuration);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (tb != null)
                tb.Dispose(); //the icon would clean up automatically, but this is cleaner
            if (isFirstInstance)
                onceInstanceMutex.ReleaseMutex();
            if (evt != null)
                evt.Dispose();
            base.OnExit(e);
        }
    }
}
