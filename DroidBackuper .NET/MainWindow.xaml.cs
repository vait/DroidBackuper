using DroidBackuper.NET.ViewModels.Helpers;
using System;
using System.Windows;

namespace DroidBackuper.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(Window_DataContextChanged);
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dc = DataContext as IClosableViewModel;
            if (dc == null)
                return;
            dc.CloseWindowEvent += new EventHandler(vm_CloseWindowEvent);
        }

        private void vm_CloseWindowEvent(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
