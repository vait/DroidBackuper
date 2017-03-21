
using System;

namespace DroidBackuper.NET.ViewModels.Commands
{
    public class OpenSettingsCommand : BaseCommand<OpenSettingsCommand>
    {
        public override bool CanExecute(object parameter)
        {
            return App.Current.MainWindow == null;
        }

        public override void Execute(object parameter)
        {
            App.Current.MainWindow = new MainWindow { DataContext = new VWMainWindow() };
            App.Current.MainWindow.Show();
        }
    }
}
