using DroidBackuper.NET.Classes.Helpers;
using DroidBackuper.NET.ViewModels.Helpers;
using System;
using System.Runtime.Versioning;

namespace DroidBackuper.NET.ViewModels
{
	[SupportedOSPlatform("windows")]
	internal class VWMainWindow : ViewModelBase
	{
		#region Свойства

		/// <summary>
		/// Производитель
		/// </summary>
		private string deviceManufacturer;

		/// <summary>
		/// Производитель
		/// </summary>
		public string DeviceManufacturer
		{
			get { return deviceManufacturer; }
			set
			{
				if (!String.IsNullOrEmpty(value.Trim()) && String.Compare(value.Trim(), deviceManufacturer, false) != 0)
				{
					deviceManufacturer = value;
					RaisePropertyChanged("DeviceManufacturer");
				}
			}
		}

		/// <summary>
		/// Название устройства
		/// </summary>
		private string deviceName;

		/// <summary>
		/// Название устройства
		/// </summary>
		public string DeviceName
		{
			get { return deviceName; }
			set
			{
				if (!String.IsNullOrEmpty(value.Trim()) && String.Compare(value.Trim(), deviceName, false) != 0)
				{
					deviceName = value;
					RaisePropertyChanged("DeviceName");
				}
			}
		}

		/// <summary>
		/// Минимальный интервал следующего запуска
		/// </summary>
		private int startInterval;

		/// <summary>
		/// Минимальный интервал следующего запуска
		/// </summary>
		public int StartInterval
		{
			get { return startInterval; }
			set
			{
				if (value > 0 && value <= 500 && value != startInterval)
				{
					startInterval = value;
					RaisePropertyChanged("StartInterval");
				}
			}
		}

		/// <summary>
		/// Формировать полные логи?
		/// </summary>
		private bool deviceLog;

		/// <summary>
		/// Формировать полные логи?
		/// </summary>
		public bool DeviceLog
		{
			get
			{
				return deviceLog;
			}

			set
			{
				if (value != deviceLog)
				{
					deviceLog = value;
					RaisePropertyChanged("DeviceLog");
				}
			}
		}

		/// <summary>
		/// Зпаускать при старте?
		/// </summary>
		private bool isStartap;

		/// <summary>
		/// Зпаускать при старте?
		/// </summary>
		public bool IsStartup
		{
			get
			{
				return isStartap;
			}

			set
			{
				if (value != isStartap)
				{
					isStartap = value;
					RaisePropertyChanged("IsStartup");
				}
			}
		}

		/// <summary>
		/// Команды для выполнения
		/// </summary>
		private string commands;

		/// <summary>
		/// Команды для выполнения
		/// </summary>
		public string Commands
		{
			get { return commands; }
			set
			{
				if (!String.IsNullOrEmpty(value.Trim()) && String.Compare(value.Trim(), commands, false) != 0)
				{
					commands = value;
					RaisePropertyChanged("Commands");
				}
			}
		}

		#endregion

		/// <summary>
		/// Консруктор
		/// </summary>
		public VWMainWindow()
			: base()
		{
			//Заполняем значения
			var cfg = ConfigHelper.Configuration;
			deviceManufacturer = cfg.DeviceManufacturer;
			deviceName = cfg.DeviceName;
			startInterval = (int)cfg.StartInterval.TotalMinutes;
			deviceLog = cfg.DeviceLog;
			isStartap = RegistryHelper.IsStartup;
			commands = string.Join(";", cfg.Commands);


			//обработчик кнопок редакторов простых справочников
			//SelectedItemDoubleCommandClick = new RelayCommand<object>(OnSelectedItemDoubleCommandClick);
			ApplyBtnClick = new RelayCommand<object>(OnApplyBtnClick);
		}

		/// <summary>
		/// Выход из приложения
		/// </summary>
		public RelayCommand<object> ApplyBtnClick { get; set; }

		/// <summary>
		/// Обработчик события выход из приложения
		/// </summary>
		/// <param name="window">Не используется</param>
		protected virtual void OnApplyBtnClick(object window)
		{
			if (isStartap != RegistryHelper.IsStartup)
			{
				if (isStartap)
				{
					RegistryHelper.SetStartup();
				}
				else
				{
					RegistryHelper.UnsetStartup();
				}

				RegistryHelper.IsStartup = isStartap;
			}

			var cfg = ConfigHelper.Configuration;
			cfg.DeviceManufacturer = deviceManufacturer;
			cfg.DeviceName = deviceName;
			cfg.StartInterval = TimeSpan.FromMinutes(startInterval);
			ConfigHelper.DeviceLog = deviceLog;
			cfg.Commands = commands.Split(new[] { ';' });

			ConfigHelper.Save();

			this.CloseWindow();
		}

		/// <summary>

		/// <summary>
		/// Обработчик нажатия печати
		/// </summary>
		/// <param name="selectedItem">Выбранный элемент</param>
		void Test(object window)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine("");

			System.Diagnostics.Debug.WriteLine("fff");

			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine("");
			/*
			System.Collections.Generic.IEnumerable<Classes.Budgeting.DepartmentName> dpt;

			DB.Collections.BaseMappedCollection<Classes.Budgeting.DepartmentName> maps =
				DB.Collections.BaseMappedCollection<Classes.Budgeting.DepartmentName>.Instance;
			maps.DBController = DB.Controllers.DepartmentDBController.Instance;

			this.customEndTaskAction = () =>
			{
				System.Diagnostics.Debug.WriteLine("");
				System.Diagnostics.Debug.WriteLine("");
				System.Diagnostics.Debug.WriteLine("");

				foreach (Classes.Budgeting.DepartmentName dp in maps.GetItems())
				{
					System.Diagnostics.Debug.WriteLine(dp + " " + dp.Branches.Count.ToString());
				}

				System.Diagnostics.Debug.WriteLine("");
				System.Diagnostics.Debug.WriteLine("");
				System.Diagnostics.Debug.WriteLine("");
			};

			this.tasks.Enqueue(new CustomAction(maps.LoadAll, "Загрузка подразделений"));

			this.StartBackgroundTask();

			//;
			 * */

			/*            System.Diagnostics.Debug.WriteLine("");
						System.Diagnostics.Debug.WriteLine("");
						System.Diagnostics.Debug.WriteLine("");

						br = maps.GetItems();

						foreach (Classes.Budgeting.DepartmentName dp in br)
						{
							System.Diagnostics.Debug.WriteLine(dp);
						}*/



		}
	}
}
