using DroidBackuper.NET.Classes.Helpers;
using System.Runtime.Versioning;

namespace DroidBackuper.NET.ViewModels.Commands
{
	[SupportedOSPlatform("windows")]
	public class StartupCommand : BaseCommand<StartupCommand>
	{
		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			if (RegistryHelper.IsStartup)
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
