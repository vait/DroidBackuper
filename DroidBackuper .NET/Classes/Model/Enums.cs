using System;

namespace DroidBackuper.NET.Classes.Model
{
    /// <summary>
    /// Property data types
    /// </summary>
    /// <see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/dn315030(v=vs.85).aspx"/>
    [Flags]
    public enum DEVPROPTYPE
    {
        /// <summary>
        /// nothing, no property data
        /// </summary>
        DEVPROP_TYPE_EMPTY = 0x00000000, 
        /// <summary>
        /// null property data
        /// </summary>
        DEVPROP_TYPE_NULL = 0x00000001,
        /// <summary>
        /// 8-bit signed int (SBYTE)
        /// </summary>
        DEVPROP_TYPE_SBYTE = 0x00000002,
        /// <summary>
        /// 8-bit unsigned int (BYTE)
        /// </summary>
        DEVPROP_TYPE_BYTE = 0x00000003,
        /// <summary>
        /// 16-bit signed int (SHORT)
        /// </summary>
        DEVPROP_TYPE_INT16 = 0x00000004,
        /// <summary>
        /// 16-bit unsigned int (USHORT)
        /// </summary>
        DEVPROP_TYPE_UINT16 = 0x00000005,
        /// <summary>
        /// 32-bit signed int (LONG)
        /// </summary>
        DEVPROP_TYPE_INT32 = 0x00000006,
        /// <summary>
        /// 32-bit unsigned int (ULONG)
        /// </summary>
        DEVPROP_TYPE_UINT32 = 0x00000007,
        /// <summary>
        /// 64-bit signed int (LONG64)
        /// </summary>
        DEVPROP_TYPE_INT64 = 0x00000008,
        /// <summary>
        /// 64-bit unsigned int (ULONG64)
        /// </summary>
        DEVPROP_TYPE_UINT64 = 0x00000009,
        /// <summary>
        /// 32-bit floating-point (FLOAT)
        /// </summary>
        DEVPROP_TYPE_FLOAT = 0x0000000A,
        /// <summary>
        /// 64-bit floating-point (DOUBLE)
        /// </summary>
        DEVPROP_TYPE_DOUBLE = 0x0000000B,
        /// <summary>
        /// 128-bit data (DECIMAL)
        /// </summary>
        DEVPROP_TYPE_DECIMAL = 0x0000000C,
        /// <summary>
        /// 128-bit unique identifier (GUID)
        /// </summary>
        DEVPROP_TYPE_GUID = 0x0000000D,
        /// <summary>
        /// 64 bit signed int currency value (CURRENCY)
        /// </summary>
        DEVPROP_TYPE_CURRENCY = 0x0000000E,
        /// <summary>
        /// date (DATE)
        /// </summary>
        DEVPROP_TYPE_DATE = 0x0000000F,
        /// <summary>
        /// file time (FILETIME)
        /// </summary>
        DEVPROP_TYPE_FILETIME = 0x00000010,
        /// <summary>
        /// 8-bit boolean (DEVPROP_BOOLEAN)
        /// </summary>
        DEVPROP_TYPE_BOOLEAN = 0x00000011,
        /// <summary>
        /// null-terminated string
        /// </summary>
        DEVPROP_TYPE_STRING = 0x00000012,
        /// <summary>
        /// multi-sz string list
        /// </summary>
        DEVPROP_TYPE_STRING_LIST = (DEVPROP_TYPE_STRING | DEVPROP_TYPEMOD_LIST),
        /// <summary>
        /// self-relative binary SECURITY_DESCRIPTOR
        /// </summary>
        DEVPROP_TYPE_SECURITY_DESCRIPTOR = 0x00000013,
        /// <summary>
        /// security descriptor string (SDDL format)
        /// </summary>
        DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING = 0x00000014,
        /// <summary>
        /// device property key (DEVPROPKEY)
        /// </summary>
        DEVPROP_TYPE_DEVPROPKEY = 0x00000015,
        /// <summary>
        /// device property type (DEVPROPTYPE)
        /// </summary>
        DEVPROP_TYPE_DEVPROPTYPE = 0x00000016,
        /// <summary>
        /// custom binary data
        /// </summary>
        DEVPROP_TYPE_BINARY = (DEVPROP_TYPE_BYTE | DEVPROP_TYPEMOD_ARRAY),  
        /// <summary>
        /// 32-bit Win32 system error code
        /// </summary>
        DEVPROP_TYPE_ERROR = 0x00000017,
        /// <summary>
        /// 32-bit NTSTATUS code
        /// </summary>
        DEVPROP_TYPE_NTSTATUS = 0x00000018,
        /// <summary>
        /// string resource (@[path\]<dllname>,-<strId>)
        /// </summary>
        DEVPROP_TYPE_STRING_INDIRECT = 0x00000019,
        /// <summary>
        /// array of fixed-sized data elements
        /// </summary>
        DEVPROP_TYPEMOD_ARRAY = 0x00001000,
        /// <summary>
        /// list of variable-sized data elements
        /// </summary>
        DEVPROP_TYPEMOD_LIST = 0x00002000,
    }
}