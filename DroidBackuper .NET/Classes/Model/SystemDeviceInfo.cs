using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DroidBackuper.NET.Classes.Helpers;

namespace DroidBackuper.NET.Classes.Model
{
    /// <summary>
    /// Класс получения системной информации по устройству
    /// </summary>
    public class SystemDeviceInfo
    {
        /// <summary>
        /// Конвертер массива байт из указателя в массив структур
        /// </summary>
        /// <typeparam name="T">Тип структуры</typeparam>
        /// <param name="memoryPointer">Указатель на память</param>
        /// <param name="structArray">массив структур, которые нужно заполнить</param>
        private void BytesToStructArray<T>(IntPtr memoryPointer, T[] structArray) where T : struct
        {
            long LongPtr = memoryPointer.ToInt64(); // Must work both on x86 and x64

            for (int i = 0; i < structArray.Length; i++)
            {
                IntPtr RectPtr = new IntPtr(LongPtr);
                structArray[i] = Marshal.PtrToStructure<T>(RectPtr); // You do not need to erase struct in this case
                LongPtr += Marshal.SizeOf(typeof(T));
            }
        }

        /// <summary>
        /// Возвращает описание класса по GUID
        /// </summary>
        /// <param name="classGuid">идентификатор класса</param>
        /// <returns>Строка с наименованием</returns>
        public string GetClassDescription(Guid classGuid)
        {
            StringBuilder sb = new StringBuilder(0);
            int descrSize = 0;
            int requiredSize = 0;
            bool res = Win32.SetupDiGetClassDescription(ref classGuid, sb, descrSize, ref requiredSize);

            sb = new StringBuilder(requiredSize);
            descrSize = requiredSize;
            res = Win32.SetupDiGetClassDescription(ref classGuid, sb, descrSize, ref requiredSize);

            return sb.ToString();
        }

        /// <summary>
        /// Подсчитывает количество устройств в классе
        /// </summary>
        /// <param name="classGuid">идентификатор класса</param>
        /// <returns>количество устройств</returns>
        public int CountDeviceInClass(Guid classGuid)
        {
            IntPtr classHandle = Win32.SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, Win32.DIGCF_PRESENT);

            uint devCount = 0;
            bool success;

            if (classHandle != (IntPtr)Win32.INVALID_HANDLE_VALUE)
            {
                do
                {
                    Win32.SP_DEVINFO_DATA devDataInfo = new Win32.SP_DEVINFO_DATA();
                    devDataInfo.cbSize = (uint)Marshal.SizeOf(devDataInfo);
                    success = Win32.SetupDiEnumDeviceInfo(classHandle, devCount, ref devDataInfo);
                    if (success)
                    {
                        devCount++;
                    }
                } while(success);
            }

            return (int)devCount;
        }

        /// <summary>
        /// Возвращает структуры с описанием устройств в классе
        /// </summary>
        /// <param name="classGuid">идентификатор класса</param>
        /// <returns>перечистелние структур классов</returns>
        public IEnumerable<Win32.SP_DEVINFO_DATA> EnumerateDeviceInClass(Guid classGuid)
        {
            IntPtr classHandle = Win32.SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, Win32.DIGCF_PRESENT);

            uint devCount = 0;
            bool success;

            if (classHandle != (IntPtr)Win32.INVALID_HANDLE_VALUE)
            {
                do
                {
                    Win32.SP_DEVINFO_DATA devDataInfo = new Win32.SP_DEVINFO_DATA();
                    devDataInfo.cbSize = (uint)Marshal.SizeOf(devDataInfo);
                    success = Win32.SetupDiEnumDeviceInfo(classHandle, devCount, ref devDataInfo);
                    if (success)
                    {
                        devCount++;
                        //Не знаю почему, но в тесте этот вызов проходит нормально, 
                        //в боевых условиях возвращает инстанс отличный от реального (например, 10 вместо 1)
                        devDataInfo.devInst = devCount;
                        yield return devDataInfo;
                    }
                } while (success);
            }

            yield break;
        }

        /// <summary>
        /// Возвращает структуры свойств, доступных для характеристик данного класса
        /// </summary>
        /// <param name="classGuid">Идентификатор класса</param>
        /// <returns>Перечисление характеристик</returns>
        public IList<Win32.DEVPROPKEY> GetPropertyKeysInClass(Guid classGuid)
        {
            IntPtr classHandle = Win32.SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, Win32.DIGCF_PRESENT);
            List<Win32.DEVPROPKEY> keys = new List<Win32.DEVPROPKEY>();
            int requiredCount = 0;

            bool success = true;

            Win32.SP_DEVINFO_DATA devDataInfo = new Win32.SP_DEVINFO_DATA();
            devDataInfo.cbSize = (uint)Marshal.SizeOf(devDataInfo);

            if (Win32.SetupDiEnumDeviceInfo(classHandle, 0, ref devDataInfo)) {
                IntPtr buffer = IntPtr.Zero;
                Win32.SetupDiGetDevicePropertyKeys(classHandle, ref devDataInfo, buffer, 0, ref requiredCount, 0);

                Win32.DEVPROPKEY[] propkeys = new Win32.DEVPROPKEY[requiredCount];
                int iStructSize = Marshal.SizeOf(propkeys[0]);

                buffer = Marshal.AllocHGlobal(iStructSize * requiredCount);

                success = Win32.SetupDiGetDevicePropertyKeys(classHandle, ref devDataInfo, buffer, requiredCount, ref requiredCount, 0);

                if (!success) {
                    Marshal.FreeHGlobal(buffer);
                    return keys.ToArray();
                }

                BytesToStructArray<Win32.DEVPROPKEY>(buffer, propkeys);

                Marshal.FreeHGlobal(buffer);

                keys.AddRange(propkeys);
            }

            return keys.ToArray();
        }

        /// <summary>
        /// Печатает в логгер информацию по всем характеристикам для указзаного устройства в классе
        /// </summary>
        /// <param name="classGuid">идентификатор класса</param>
        /// <param name="deviceInstance">идентификатор устройства</param>
        /// <param name="logger">писатель</param>
        internal void PrintInfo(Guid classGuid, string deviceInstance, ILogger logger)
        {
            StringBuilder logText = new StringBuilder();

            logText.AppendLine(String.Format("[{0}]-------------- START Device information -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));

            var className = GetClassDescription(classGuid);
            logText.AppendLine(String.Format("\tClass \"{0}\" ID [{1}]", classGuid.ToString(), className));

            var deviceCount = CountDeviceInClass(classGuid);
            logText.AppendLine(String.Format("\tThe class has {0} device(s)", deviceCount));

            //Ищем устройство среди представленных в классе 
            var devList = EnumerateDeviceInClass(classGuid);
            var deviceInstanceProperty = Win32.DeviceProperties.Where(p => p.Value == "PKEY_Device_InstanceId").First().Key;
            bool found = false;
            Win32.SP_DEVINFO_DATA device = new Win32.SP_DEVINFO_DATA();

            foreach (var dev in devList)
            {
                //Проверяем свойства
                var propValue = GetPropertyValuesInClass(classGuid, deviceInstanceProperty, dev);
                if (propValue != null && propValue.Equals(deviceInstance))
                {
                    found = true;
                    device = dev;
                    break;
                }
            }

            if (!found)
            {
                if (deviceCount > 0)
                    device = devList.First();
                logText.AppendLine(String.Format("\tDevice with instance {0} not found.", deviceInstance));
            }
            else
            {
                //Выводим ВСЕ свойства устройства
                var propList = GetPropertyKeysInClass(classGuid);

                foreach (var prop in propList)
                {
                    var propValue = GetPropertyValuesInClass(classGuid, prop, device);
                    if (propValue == null)
                        continue;
                    Type valueType = propValue.GetType();
                    if (valueType.IsArray)
                    {
                        string[] arr = propValue as string[];

                        logText.AppendLine(String.Format("\t{0}: [{1}]\t{2}",
                            (Win32.DeviceProperties.ContainsKey(prop) ? Win32.DeviceProperties[prop] : prop.formatId.ToString()),
                           prop.propertyId, arr.Length > 0 ? arr[0] : "no value"));

                        if (arr.Length > 1)
                        {
                            for (int i = 1; i < arr.Length; ++i)
                            {
                                logText.AppendLine(String.Format("\t\t{0}", arr[i]));
                            }
                        }
                    }
                    else
                    {
                        logText.AppendLine(String.Format("\t{0}: [{1}]\t{2}", 
                            (Win32.DeviceProperties.ContainsKey(prop) ? Win32.DeviceProperties[prop] : prop.formatId.ToString()),
                           prop.propertyId, propValue));
                    }
                }

            }

            logText.AppendLine(String.Format("[{0}]-------------- END Device information -----------", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")));
            logger.WriteLog(logText.ToString());
        }

        /// <summary>
        /// Получает значение характеристики устройства в указанном класса. Так как для каждого запроса 
        /// устройства выделяется новый указатель в памяти на описание его, то работаем с номером устройства
        /// </summary>
        /// <param name="classGuid">идентификатор класса</param>
        /// <param name="property">искомая характеристика</param>
        /// <param name="deviceNumber">порядковый номер устройства в классе</param>
        /// <returns>значение свойства</returns>
        public object GetPropertyValuesInClass(Guid classGuid, Win32.DEVPROPKEY property, Win32.SP_DEVINFO_DATA deviceNumber)
        {
            object value = null;
            IntPtr classHandle = Win32.SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, Win32.DIGCF_PRESENT);

            bool success = true;
            uint proptype = 0;
            IntPtr buffer = IntPtr.Zero;
            uint buffSize = 0;
            uint reqBuffSize = 0;

            Win32.SP_DEVINFO_DATA devDataInfo = new Win32.SP_DEVINFO_DATA();
            devDataInfo.cbSize = (uint)Marshal.SizeOf(devDataInfo);
            success = Win32.SetupDiEnumDeviceInfo(classHandle, deviceNumber.devInst - 1, ref devDataInfo);

            if (!success)
                return value;

            try
            {
                //Return ERROR_INSUFFICIENT_BUFFER
                //122(0x7A)
                //The data area passed to a system call is too small.
                Win32.SetupDiGetDeviceProperty(classHandle, ref devDataInfo, ref property, out proptype, buffer, buffSize, out reqBuffSize, 0);

                buffSize = reqBuffSize;
                buffer = Marshal.AllocHGlobal((int)buffSize);
                byte[] lbuffer = new byte[reqBuffSize];

                Win32.SetupDiGetDeviceProperty(classHandle, ref devDataInfo, ref property, out proptype, buffer, buffSize, out reqBuffSize, 0);

                Marshal.Copy(buffer, lbuffer, 0, (int)reqBuffSize);

                switch((DEVPROPTYPE)proptype)
                {
                    case DEVPROPTYPE.DEVPROP_TYPE_BOOLEAN:
                        value = Convert.ToBoolean(lbuffer[0]);
                        break;
                    case DEVPROPTYPE.DEVPROP_TYPE_STRING:
                        value = Encoding.Unicode.GetString(lbuffer).Replace("\0", "");
                        break;
                    case DEVPROPTYPE.DEVPROP_TYPE_GUID:
                        value = new Guid(lbuffer);
                        break;
                    case DEVPROPTYPE.DEVPROP_TYPE_UINT32:
                        value = BitConverter.ToUInt32(lbuffer, 0);
                        break;
                    case DEVPROPTYPE.DEVPROP_TYPE_BINARY:
                        value = BitConverter.ToString(lbuffer).Replace("-", string.Empty);
                        break;
                    case DEVPROPTYPE.DEVPROP_TYPE_UINT64:
                        value = BitConverter.ToUInt64(lbuffer, 0);
                        break;
                    case DEVPROPTYPE.DEVPROP_TYPE_STRING_LIST:
                        value = Encoding.Unicode.GetString(lbuffer).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case DEVPROPTYPE.DEVPROP_TYPE_FILETIME:
                        value = DateTime.FromFileTime(BitConverter.ToInt64(lbuffer, 0));
                        break;
                    default:
                        value = ((DEVPROPTYPE)proptype).ToString();
                        break;
                }
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
            }

           return value;
        }
    }
}
