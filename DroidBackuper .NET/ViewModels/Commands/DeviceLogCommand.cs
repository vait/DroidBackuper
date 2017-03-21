using DroidBackuper.NET.Classes.Helpers;
using System.Windows.Controls;

namespace DroidBackuper.NET.ViewModels.Commands
{
    public class DeviceLogCommand : BaseCommand<DeviceLogCommand>
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            ConfigHelper.DeviceLog = !ConfigHelper.DeviceLog;
            ConfigHelper.Save();
        }
    }
}
