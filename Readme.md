# DroidBackuper
DroidBackuper is a simple application for copy some user data from your device with Android.
## Main idea
The main idea of this project is automatically copy user data from my phone when I connect it to computer.
First of all, I need to catch the device plug in event in Windows. Several search in Google requests led me to post on stackoverflow.com (How to identify what device was plugged into the USB slot?)[http://stackoverflow.com/questions/16620509/how-to-identify-what-device-was-plugged-into-the-usb-slot]. It would seem that a solution is found. But I think that solution give very small information about device. I want know more...
## Research Windows device information
I found needed information on:
* [Работа с устройствами в Windows](http://pblog.ru/?p=105)
* [C# USB driver from C++: SetupDiGetDeviceInterfaceDetail (stackoverflow.com)](http://stackoverflow.com/questions/30981181/c-sharp-usb-driver-from-c-setupdigetdeviceinterfacedetail)
* [Win32Api USB SetupDiGetDeviceInterfaceDetail fail (stackoverflow.com)](http://stackoverflow.com/questions/9245595/win32api-usb-setupdigetdeviceinterfacedetail-fail)
* [pinvoke.net](http://www.pinvoke.net/default.aspx)
But I have one problem: when I call SetupDiEnumDeviceInfo for my WPD-phone for counting device in this class, I get only one device. **But**! in struct SP_DEVINFO_DATA devInst is not 1. It may be 10 or 7. Why do it happen? I don't know. That's why I did this in `SystemDeviceInfo.cs`:
```cs
    do
    {
        Win32.SP_DEVINFO_DATA devDataInfo = new Win32.SP_DEVINFO_DATA();
        devDataInfo.cbSize = (uint)Marshal.SizeOf(devDataInfo);
        success = Win32.SetupDiEnumDeviceInfo(classHandle, devCount, ref devDataInfo);
        if (success)
        {
            devCount++;
            devDataInfo.devInst = devCount;
            yield return devDataInfo;
        }
    } while (success);
```
## What I got in the end
This application listening Windows `Win32_PnPEntity` event, when device plugged in it compare manufacture device and model. If they are all equals application executes commands, and, if need, write full information of plugged device to log file.
## Usage
Start application
Find in tray icon with clock
Open settings
Enjoy
## PS
Sory for my English ;)