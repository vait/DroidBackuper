using System;

namespace DroidBackuper.NET.Classes.Model
{
	class Settings
	{
		/// <summary>
		/// Название устройства.
		/// </summary>
		public string DeviceName { get; set; }

		//Производитель.
		public string DeviceManufacturer { get; set; }

		/// <summary>
		/// Запуск скрипта не реже чем один раз в
		/// </summary>
		public TimeSpan StartInterval { get; set; } = TimeSpan.FromSeconds(1);

		/// <summary>
		/// Сиполняемые команды.
		/// </summary>
		public string Commands { get; set; }

		/// <summary>
		/// Необходимо ли включать лог.
		/// </summary>
		public bool DeviceLog { get; set; }

	}
}
