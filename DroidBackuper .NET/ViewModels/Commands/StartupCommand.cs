using DroidBackuper.NET.Classes.Helpers;
using System.Windows.Controls;

namespace DroidBackuper.NET.ViewModels.Commands
{
    public class StartupCommand : BaseCommand<StartupCommand>
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if(RegistryHelper.IsStartup)
            {
                RegistryHelper.UnsetStartup();
                RegistryHelper.IsStartup = false;
            }
            else
            {
                RegistryHelper.SetStartup();
                RegistryHelper.IsStartup = true;
            }
        }
    }
}
