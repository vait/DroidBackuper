using DroidBackuper.NET.Classes.Helpers;
using System;
using System.Diagnostics;

namespace DroidBackuper.NET.Classes
{
	/// <summary>
	/// класс с настройками системы
	/// </summary>
	class AppConfiguration
	{
		/// <summary>
		/// Писатель логов
		/// </summary>
		public ILogger Logger { get; set; }

		/// <summary>
		/// Производитель устройства
		/// </summary>
		public string DeviceManufacturer { get; set; }

		/// <summary>
		/// Название устройства
		/// </summary>
		public string DeviceName { get; set; }

		/// <summary>
		/// Периодичность запуска скрипта
		/// </summary>
		public TimeSpan StartInterval { get; set; }

		/// <summary>
		/// Выполняемое действие
		/// </summary>
		public Action ExecAction { get; set; }

		/// <summary>
		/// Записывать расширенную информацию от устройстве
		/// </summary>
		public bool DeviceLog { get; set; }

		/// <summary>
		/// выполняемые команды при обнаружении устройства
		/// </summary>
		public string[] Commands { get; set; }

		/// <summary>
		/// Конструктор
		/// </summary>
		public AppConfiguration()
		{
			StartInterval = TimeSpan.FromMinutes(1);
			DeviceLog = false;
			ExecAction = () =>
			 {
				 foreach (var cmd in Commands)
				 {
					 var args = cmd.Trim().Split(new[] { ' ' });
					 ProcessStartInfo pi = new ProcessStartInfo(args[0]);
					 if (args.Length > 1)
					 {
						 pi.Arguments = String.Join(" ", args, 1, args.Length - 1);
					 }
					 Process.Start(pi);
				 }
			 };
		}
	}
}
