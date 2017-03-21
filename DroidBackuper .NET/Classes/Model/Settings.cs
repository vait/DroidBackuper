using System;

namespace DroidBackuper.NET.Classes.Model
{
    struct Settings
    {
        private string deviceName;

        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; }
        }

        private string deviceManufacturer;

        public string DeviceManufacturer
        {
            get { return deviceManufacturer; }
            set { deviceManufacturer = value; }
        }

    }
}
