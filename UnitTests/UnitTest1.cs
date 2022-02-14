using DroidBackuper.NET.Classes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTest
{
	public class SystemDeviceInfoTest
	{
		SystemDeviceInfo siClass;


		public SystemDeviceInfoTest()
		{
			siClass = new SystemDeviceInfo();

			//http://www.pinvoke.net/default.aspx/Constants/PROPERTYKEY.html

		}

		[Fact]
		public void TestHID()
		{
			Guid devClassGuid = Guid.Parse("745a17a0-74d3-11d0-b6fe-00a0c90f57da");//HID

			Assert.Equal("Устройства HID (Human Interface Devices)", siClass.GetClassDescription(devClassGuid));

			var devList = new List<Win32.SP_DEVINFO_DATA>(siClass.EnumerateDeviceInClass(devClassGuid));
			var propList = siClass.GetPropertyKeysInClass(devClassGuid);
			var deviceInstanceProperty = Win32.DeviceProperties.Where(p => p.Value == "PKEY_Device_InstanceId").First().Key;
			var propValue1 = siClass.GetPropertyValuesInClass(devClassGuid, deviceInstanceProperty, devList[0]);

			Assert.True(propList.Count > 0);
			foreach (var prop in propList)
			{
				Assert.NotEqual(0, (int)prop.propertyId);
				Assert.NotEqual(Guid.Empty, prop.formatId);
				var propValue = siClass.GetPropertyValuesInClass(devClassGuid, prop, devList[0]);
				System.Diagnostics.Debug.WriteLine((Win32.DeviceProperties.ContainsKey(prop) ? Win32.DeviceProperties[prop] + "\t" : (prop.formatId.ToString() + "\t" + prop.propertyId) + "\t") + propValue);
			}
		}

		[Fact]
		public void TestHSDD()
		{
			Guid devClassGuid = Guid.Parse("4d36e967-e325-11ce-bfc1-08002be10318");//Disks
			Assert.Equal("Дисковые устройства", siClass.GetClassDescription(devClassGuid));

			var devList = new List<Win32.SP_DEVINFO_DATA>(siClass.EnumerateDeviceInClass(devClassGuid));
			var propList = siClass.GetPropertyKeysInClass(devClassGuid);

			Assert.True(propList.Count > 0);
			foreach (var prop in propList)
			{
				Assert.NotEqual(0, (int)prop.propertyId);
				Assert.NotEqual(Guid.Empty, prop.formatId);
				var propValue = siClass.GetPropertyValuesInClass(devClassGuid, prop, devList[0]);
				System.Diagnostics.Debug.WriteLine((Win32.DeviceProperties.ContainsKey(prop) ? Win32.DeviceProperties[prop] + "\t" : (prop.formatId.ToString() + "\t" + prop.propertyId) + "\t") + propValue);
			}
		}

		[Fact]
		public void TestUSBControllers()
		{
			Guid devClassGuid = Guid.Parse("36fc9e60-c465-11cf-8056-444553540000");//AUDIO
			Assert.Equal("Контроллеры USB", siClass.GetClassDescription(devClassGuid));

			var devList = new List<Win32.SP_DEVINFO_DATA>(siClass.EnumerateDeviceInClass(devClassGuid));
			var propList = siClass.GetPropertyKeysInClass(devClassGuid);

			Assert.True(propList.Count > 0);
			foreach (var prop in propList)
			{
				Assert.NotEqual(0, (int)prop.propertyId);
				Assert.NotEqual(Guid.Empty, prop.formatId);
				var propValue = siClass.GetPropertyValuesInClass(devClassGuid, prop, devList[0]);
				System.Diagnostics.Debug.WriteLine((Win32.DeviceProperties.ContainsKey(prop) ? Win32.DeviceProperties[prop] + "\t" : (prop.formatId.ToString() + "\t" + prop.propertyId) + "\t") + propValue);
			}
		}
	}
}
;