using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DroidBackuper.NET.Classes.Model;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class SystemDeviceInfoTest
    {
        SystemDeviceInfo siClass;
        

        [TestInitialize]
        public void Initialize()
        {
            siClass = new SystemDeviceInfo();

            //http://www.pinvoke.net/default.aspx/Constants/PROPERTYKEY.html
            
        }

        [TestMethod]
        public void TestWPD()
        {
            Guid devClassGuid = Guid.Parse("eec5ad98-8080-425f-922a-dabf3de3f69a");//WPD

            Assert.AreEqual("Переносные устройства", siClass.GetClassDescription(devClassGuid));

            Assert.AreEqual(1, siClass.CountDeviceInClass(devClassGuid));

            var devList = new List<Win32.SP_DEVINFO_DATA>( siClass.EnumerateDeviceInClass(devClassGuid));

            var propList = siClass.GetPropertyKeysInClass(devClassGuid);

            var deviceInstanceProperty = Win32.DeviceProperties.Where(p => p.Value == "PKEY_Device_InstanceId").First().Key;
            var propValue1 = siClass.GetPropertyValuesInClass(devClassGuid, deviceInstanceProperty, devList[0]);


            Assert.IsTrue(propList.Count > 0);
            foreach(var prop in propList)
            {
                Assert.AreNotEqual(0, prop.propertyId);
                Assert.AreNotEqual(Guid.Empty, prop.formatId);
                var propValue = siClass.GetPropertyValuesInClass(devClassGuid, prop, devList[0]);
                System.Diagnostics.Debug.WriteLine((Win32.DeviceProperties.ContainsKey(prop) ? Win32.DeviceProperties[prop] + "\t" : (prop.formatId.ToString() + "\t" + prop.propertyId) + "\t")  + propValue);
            }
        }

        [TestMethod]
        public void TestHDD()
        {
            Guid devClassGuid =Guid.Parse("88bae032-5a81-49f0-bc3d-a4ff138216d6");//HDD
            Assert.AreEqual("Устройства USB", siClass.GetClassDescription(devClassGuid));

            //Assert.AreEqual(1, siClass.CountDeviceInClass(devClassGuid));

            var devList = new List<Win32.SP_DEVINFO_DATA>(siClass.EnumerateDeviceInClass(devClassGuid));

            var propList = siClass.GetPropertyKeysInClass(devClassGuid);

            Assert.IsTrue(propList.Count > 0);
            foreach (var prop in propList)
            {
                Assert.AreNotEqual(0, prop.propertyId);
                Assert.AreNotEqual(Guid.Empty, prop.formatId);
                var propValue = siClass.GetPropertyValuesInClass(devClassGuid, prop, devList[0]);
                System.Diagnostics.Debug.WriteLine((Win32.DeviceProperties.ContainsKey(prop) ? Win32.DeviceProperties[prop] + "\t" : (prop.formatId.ToString() + "\t" + prop.propertyId) + "\t") + propValue);
            }
        }

        [TestMethod]
        public void TestAUDIO()
        {
            Guid devClassGuid = Guid.Parse("36fc9e60-c465-11cf-8056-444553540000");//AUDIO
            Assert.AreEqual("Контроллеры USB", siClass.GetClassDescription(devClassGuid));

            //Assert.AreEqual(2, siClass.CountDeviceInClass(devClassGuid));

            var devList = new List<Win32.SP_DEVINFO_DATA>(siClass.EnumerateDeviceInClass(devClassGuid));

            var propList = siClass.GetPropertyKeysInClass(devClassGuid);

            Assert.IsTrue(propList.Count > 0);
            foreach (var prop in propList)
            {
                Assert.AreNotEqual(0, prop.propertyId);
                Assert.AreNotEqual(Guid.Empty, prop.formatId);
                var propValue = siClass.GetPropertyValuesInClass(devClassGuid, prop, devList[0]);
                System.Diagnostics.Debug.WriteLine((Win32.DeviceProperties.ContainsKey(prop) ? Win32.DeviceProperties[prop] + "\t" : (prop.formatId.ToString() + "\t" + prop.propertyId) + "\t") + propValue);
            }
        }
    }
}
;