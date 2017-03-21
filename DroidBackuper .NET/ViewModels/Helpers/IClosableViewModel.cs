using System;


namespace DroidBackuper.NET.ViewModels.Helpers
{
    interface IClosableViewModel
    {
        event EventHandler CloseWindowEvent;
    }
}
