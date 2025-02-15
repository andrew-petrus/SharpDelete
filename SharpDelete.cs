using System;
using System.Runtime.InteropServices;

namespace SharpDelete
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING : IDisposable
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr buffer;

            public UNICODE_STRING(string s)
            {
                Length = (ushort)(s.Length * 2);
                MaximumLength = (ushort)(Length + 2);
                buffer = Marshal.StringToHGlobalUni(s);
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(buffer);
                buffer = IntPtr.Zero;
            }
        }
        // Registry hives handles
        public static UIntPtr HKEY_CURRENT_USER = (UIntPtr)0x80000001;
        public static UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
        public static UIntPtr HKEY_CLASSES_ROOT = (UIntPtr)0x80000000;
        public static UIntPtr HKEY_USERS = (UIntPtr)0x80000003;
        public static UIntPtr HKEY_CURRENT_CONFIG = (UIntPtr)0x80000005;
        public static int KEY_SET_VALUE = 0x0002; // Access rights for deletion

        // Import APIs
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern uint RegOpenKeyEx(
            UIntPtr hKey,
            string subKey,
            int ulOptions,
            int samDesired,
            out UIntPtr KeyHandle
        );

        [DllImport("ntdll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern uint NtDeleteValueKey(
            UIntPtr KeyHandle,
            IntPtr ValueName
        );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(UIntPtr KeyHandle);

        static IntPtr StructureToPtr(object obj)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }

        // Function to delete hidden registry value 
        static void DeleteHiddenRegistryValue(UIntPtr hive, string regPath, string hiveString)
        {
            UIntPtr regKeyHandle = UIntPtr.Zero;
            string hiddenValueName = "\0\0" + regPath; // Prepend null wchar characters to registry path

            Console.WriteLine($"\n[+] Deleting hidden registry key in {hiveString}\\{regPath}");

            uint status = RegOpenKeyEx(hive, regPath, 0, KEY_SET_VALUE, out regKeyHandle);
            if (status != 0)
            {
                Console.WriteLine("[!] Failed to open registry key. Status: " + status);
                return;
            }

            UNICODE_STRING valueName = new UNICODE_STRING(hiddenValueName) // Unicode string required to hold the nulls in path otherwise API breaks
            {
                Length = (ushort)(2 * 11),
                MaximumLength = 0
            };
            try
            {
                IntPtr valueNamePtr = StructureToPtr(valueName);
                status = NtDeleteValueKey(regKeyHandle, valueNamePtr); // Delete the hidden value
            }
            finally
            {
                valueName.Dispose();
            }
            if (status == 0)
            {
                Console.WriteLine("[+] Key successfully deleted.");
            }
            else
            {
                Console.WriteLine("[!] Failed to delete registry key. Status code: " + status);
            }

            RegCloseKey(regKeyHandle);
        }

        static void Main()
        {
            Console.WriteLine("\r\n[+] SharpDelete by Andrew Petrus - Tool to delete hidden registry values created by SharpHide");
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Select a registry path to remove the hidden value:");
            Console.WriteLine("1. HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run");
            Console.WriteLine("2. HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Run (Administrator privileges required)");
            Console.WriteLine("3. HKLM\\Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run (Administrator privileges required)");
            Console.WriteLine("4. Enter a custom registry path (Administrator privileges required for HKLM and most HKCR paths)");
            Console.Write("\r\nEnter your choice: ");

            string choice = Console.ReadLine();

            string hiveString = "";
            UIntPtr specifiedHive;

            switch (choice)
            {
                case "1":
                    hiveString = "HKCU";
                    specifiedHive = HKEY_CURRENT_USER;
                    DeleteHiddenRegistryValue(specifiedHive, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", hiveString);
                    break;
                case "2":
                    hiveString = "HKLM";
                    specifiedHive = HKEY_LOCAL_MACHINE;
                    DeleteHiddenRegistryValue(specifiedHive, "Software\\Microsoft\\Windows\\CurrentVersion\\Run", hiveString);
                    break;
                case "3":
                    hiveString = "HKLM";
                    specifiedHive = HKEY_LOCAL_MACHINE;
                    DeleteHiddenRegistryValue(specifiedHive, "Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", hiveString);
                    break;
                case "4":
                    Console.Write("Enter the full registry path (e.g., HKCU\\Software\\Microsoft): ");
                    string customRegPath = Console.ReadLine().Trim();
                    string[] regPathSections = customRegPath.Split(new char[] { '\\' }, 2);

                    if (regPathSections.Length < 2)
                    {
                        Console.WriteLine("Invalid path format. Use HKCU\\ or HKLM\\ as the prefix.");
                        return;
                    }

                    hiveString = regPathSections[0].ToUpper();
                    string subKeyPath = regPathSections[1]; 

                    switch (hiveString)
                    {
                        case "HKCU":
                            specifiedHive = HKEY_CURRENT_USER;
                            break;
                        case "HKLM":
                            specifiedHive = HKEY_LOCAL_MACHINE;
                            break;
                        case "HKCR":
                            specifiedHive = HKEY_CLASSES_ROOT;
                            break;
                        case "HKU":
                            specifiedHive = HKEY_USERS;
                            break;
                        case "HKCC":
                            specifiedHive = HKEY_CURRENT_CONFIG;
                            break;
                        default:
                            Console.WriteLine("Unsupported or invalid hive. Use HKCU, HKLM, HKCR, HKU, or HKCC.");
                            return;
                    }

                    DeleteHiddenRegistryValue(specifiedHive, subKeyPath, hiveString);
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}
