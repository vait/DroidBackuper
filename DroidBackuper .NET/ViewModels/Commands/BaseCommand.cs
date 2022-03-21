using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace DroidBackuper.NET.ViewModels.Commands
{
	public abstract class BaseCommand<T> : MarkupExtension, ICommand where T : class, ICommand, new()
	{
		#region MarkupExtension
		/// <summary>
		/// A singleton instance.
		/// </summary>
		private static T command;

		/// <summary>
		/// Gets a shared command instance.
		/// </summary>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (command == null) command = new T();
			return command;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
		/// </summary>
		public BaseCommand()
		{
		}
		#endregion

		#region ICommand Members

		///<summary>
		///Defines the method that determines whether the command can execute in its current state.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		///<returns>
		///true if this command can be executed; otherwise, false.
		///</returns>
		public abstract bool CanExecute(object parameter);

		///<summary>
		///Occurs when changes occur that affect whether or not the command should execute.
		///</summary>
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		///<summary>
		///Defines the method to be called when the command is invoked.
		///</summary>
		///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
		public abstract void Execute(object parameter);

		#endregion

		/// <summary>
		/// Resolves the window that owns the TaskbarIcon class.
		/// </summary>
		/// <param name="commandParameter"></param>
		/// <returns></returns>
		protected Window GetTaskbarWindow(object commandParameter)
		{
			//get the showcase window off the taskbaricon
			var tb = commandParameter as TaskbarIcon;
			return tb == null ? null : tb.TryFindParent<Window>();
		}
	}
}
