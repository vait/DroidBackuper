using System;
using DroidBackuper.NET.ViewModels.Helpers;

namespace DroidBackuper.NET.ViewModels.Commands
{
    public class ExitCommand: BaseCommand<ExitCommand>
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            App.Current.Shutdown();
        }
    }
}
