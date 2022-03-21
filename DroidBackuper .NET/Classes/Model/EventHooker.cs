using DroidBackuper.NET.Classes.Helpers;
using System;
using System.Management;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DroidBackuper.NET.Classes.Model
{
	[SupportedOSPlatform("windows")]
	class EventHooker : IDisposable
	{
		/// <summary>
		/// Настройки системы
		/// </summary>
		private AppConfiguration Config { get; set; }

		/// <summary>
		/// Писатель
		/// </summary>
		private ILogger Logger
		{
			get
			{
				return Config.Logger;
			}
		}

		private Object syncObj = new Object();
		/// <summary>
		/// Время ожидания второго события в милисекундах
		/// </summary>
		private int waitNextEventTime;

		/// <summary>
		/// Должно быть два устройства: одно само устройство, второе ADB Interface
		/// </summary>
		private SemaphoreSlim waitingDevices;

		/// <summary>
		/// Наблюдатель за подключением устройств
		/// </summary>
		private readonly ManagementEventWatcher insertWatcher;

		/// <summary>
		/// Наблюдатель отключений устройства
		/// </summary>
		private readonly ManagementEventWatcher removeWatcher;

		/// <summary>
		/// Название устройства
		/// </summary>
		private string DeviceName
		{
			get
			{
				return Config.DeviceName;
			}
		}

		/// <summary>
		/// Производитель
		/// </summary>
		private string DeviceManufacturer
		{
			get
			{
				return Config.DeviceManufacturer;
			}
		}

		/// <summary>
		/// Задача исполнитель
		/// </summary>
		private Task executer;

		/// <summary>
		/// Само действие, которое выполняется при срабатывании правил
		/// </summary>
		private Action Action
		{
			get
			{
				return Config.ExecAction;
			}
		}

		/// <summary>
		/// Время последнего исполнения
		/// </summary>
		private DateTime lastStart;

		/// <summary>
		/// С какой периодичностью ожидать следующего подключения в минутах
		/// </summary>
		private TimeSpan StartInterval
		{
			get
			{
				return Config.StartInterval;
			}
		}

		/// <summary>
		/// Нужно ли записывать расширенную информацию по устройству.
		/// </summary>
		public bool DeviceLog
		{
			get
			{
				return Config.DeviceLog;
			}
		}

		/// <summary>
		/// Обрабочик события подключенного устройства
		/// </summary>
		/// <param name="sender">инициатор</param>
		/// <param name="e">параметры события</param>
		private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
		{
			StringBuilder logText = new StringBuilder();
			ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];

			logText.AppendLine(String.Format("[{0}]-------------- START device insert -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));

			foreach (var property in instance.Properties)
			{
				logText.AppendLine("\t" + property.Name + " = " + property.Value);
			}

			logText.AppendLine(String.Format("[{0}]-------------- END device insert -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));

			Logger.WriteLog(logText.ToString());

			Task.Run(() =>
			{
				this.CheckDevice(instance.Clone() as ManagementBaseObject);
			});
		}

		/// <summary>
		/// Определяем искомое ли это устройство
		/// </summary>
		/// <param name="name">наименование</param>
		/// <param name="manufacturer">производитель</param>
		/// <returns>ИСТИНА - если устройство то, которое ищем</returns>
		private bool IsNeddedDevice(string name, string manufacturer)
		{
			return name.ToLower() == this.DeviceName.ToLower() && this.DeviceManufacturer.ToLower().Contains(manufacturer.ToLower());
		}

		private void CheckDevice(ManagementBaseObject device)
		{
			StringBuilder logText = new StringBuilder();
			logText.AppendLine(String.Format("[{0}]-------------- START CheckDevice -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));

			string name = device.Properties["Name"].Value != null ? device.Properties["Name"].Value.ToString() : "";
			string manufacturer = device.Properties["Manufacturer"].Value != null ? device.Properties["Manufacturer"].Value.ToString() : "";
			string PNPClass = device.Properties["PNPClass"].Value != null ? device.Properties["PNPClass"].Value.ToString() : "";
			bool startFunction = false;

			logText.AppendLine("\t" + name);

			if (DeviceLog && device.Properties["ClassGuid"].Value != null)
			{
				//Запишем для себя информацию о подключенном устройстве. Не важно каком.
				var deviceInstance = device.Properties["PNPDeviceID"].Value.ToString();
				var classGuid = Guid.Parse(device.Properties["ClassGuid"].Value.ToString());

				PrindDeviseInfoWithDelay(classGuid, deviceInstance, 10, Logger).FireAndForgot(Logger);
			}

			switch (PNPClass)
			{
				//Переносные устройства Windows
				case "WPD":
					logText.AppendLine("\t CheckDevice WPD");
					if (IsNeddedDevice(name, manufacturer))
					{
						this.waitingDevices.Release();
						startFunction = true;
					}
					break;
				//Это два класса, говорящие, что есть отладка по USB. Для LG, Samsung, WileyFox Swift 2
				case "USBDevice":
				case "AndroidUsbDeviceClass":
					logText.AppendLine("\t CheckDevice USBDevice");
					var tmpName = name.ToLower();
					if (tmpName.Contains("adb interface") || tmpName.Contains("android"))
					{
						this.waitingDevices.Release();
						startFunction = true;
					}
					break;
			}

			if (startFunction)
			{
				logText.AppendLine("\t CheckDevice StartExecuter");
				this.StartExecuter();
			}
			logText.AppendLine(String.Format("[{0}]-------------- END CheckDevice -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));

			Logger.WriteLog(logText.ToString());
		}

		private async Task PrindDeviseInfoWithDelay(Guid classGuid, string deviceInstance, int delayTimeout, ILogger logger)
		{
			await Task.Delay(delayTimeout);
			var di = new SystemDeviceInfo();
			di.PrintInfo(classGuid, deviceInstance, logger);
		}

		/// <summary>
		/// Асинхронно запускает выполнителя
		/// </summary>
		private void StartExecuter()
		{
			StringBuilder logText = new StringBuilder();
			logText.AppendLine(String.Format("[{0}]-------------- START StartExecuter -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));

			Monitor.Enter(syncObj);
			if (executer.Status == TaskStatus.Created)
			{
				logText.AppendLine("\t StartExecuter executer.Start()");
				logText.AppendLine("\t " + executer.Status);

				executer.Start();
			}
			Monitor.Exit(syncObj);

			logText.AppendLine(String.Format("[{0}]-------------- END StartExecuter -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));
			Logger.WriteLog(logText.ToString());
		}

		/// <summary>
		/// Выполняем запуск
		/// </summary>
		private void Executer()
		{
			StringBuilder logText = new StringBuilder();
			logText.AppendLine(String.Format("[{0}]-------------- START Executer -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));
			//Слишком часто выполняем копирование

			logText.AppendLine("\t Executer Start waiting");

			if (this.waitingDevices.Wait(this.waitNextEventTime) &&
				this.waitingDevices.Wait(this.waitNextEventTime))
			{
				if ((DateTime.Now - this.lastStart) > this.StartInterval)
				{
					logText.AppendLine("\t Executer WORKING");
					this.Action();

					this.lastStart = DateTime.Now;
				}
			}

			this.executer = new Task(this.Executer);

			logText.AppendLine(String.Format("[{0}]-------------- END Executer -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));
			Logger.WriteLog(logText.ToString());
		}


		/// <summary>
		/// Обработчик события отключения устройства
		/// </summary>
		/// <param name="sender">инициатор</param>
		/// <param name="e">параметры события</param>
		private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
		{
			StringBuilder logText = new StringBuilder();
			ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
			logText.AppendLine(String.Format("[{0}]-------------- removed -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));

			foreach (var property in instance.Properties)
			{
				logText.AppendLine("\t" + property.Name + " = " + property.Value);
			}
			logText.AppendLine(String.Format("[{0}]-------------- removed -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));
			Logger.WriteLog(logText.ToString());
		}


		/// <summary>
		/// Контруктор
		/// </summary>
		/// <param name="logger">Писатель логов</param>
		/// <param name="deviceManufacturer">Имя производителя устройства</param>
		/// <param name="deviceName">название устройства</param>
		/// <param name="startInterval">частоста повторного срабатывания</param>
		public EventHooker(AppConfiguration config)
		{//ILogger logger, string deviceManufacturer, string deviceName, int startInterval, Action execAction
		 //Константы
			this.waitNextEventTime = 5000;

			this.Config = config;

			this.waitingDevices = new SemaphoreSlim(0, 2);

			this.executer = new Task(this.Executer);

			//Устройство подключено
			WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");

			insertWatcher = new ManagementEventWatcher(query);
			insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
			insertWatcher.Start();

			//Устройство отключено
			query = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
			removeWatcher = new ManagementEventWatcher(query);
			removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
			removeWatcher.Start();
		}


		/// <summary>
		/// Диспоуз
		/// </summary>
		public void Dispose()
		{
			this.insertWatcher.Dispose();
			this.removeWatcher.Dispose();
		}
	}
}
