# SharpDelete

SharpDelete is a simple tool written in **C#** designed to delete hidden registry values created by SharpHide. This tool helps remove stealthy registry entries commonly used for persistence by malicious software.

## Features

- Delete hidden registry values in common persistence locations.
- Supports both **HKCU** and **HKLM** hives.
- Option to enter custom registry paths.

## Written in C#

SharpDelete is written in **C#** and interacts directly with the Windows Registry using system calls like RegOpenKeyEx and NtDeleteValueKey. It is designed to detect and remove hidden registry values that may be used for persistence by malware or tools like SharpHide.

## Usage

**To run SharpDelete**, double click the executable, or run the following from the command line:

```plaintext
C:\Tools > SharpDelete.exe
```

Upon execution, you will see:

```plaintext
[+] SharpDelete by Andrew Petrus - Tool to delete hidden registry values created by SharpHide
---------------------------------------------------------------
Select a registry path to remove the hidden value:
1. HKCU\Software\Microsoft\Windows\CurrentVersion\Run
2. HKLM\Software\Microsoft\Windows\CurrentVersion\Run (Administrator privileges required)
3. HKLM\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Run (Administrator privileges required)
4. Enter a custom registry path (Administrator privileges required for HKLM and most HKCR paths)

Enter your choice: 
```

**Example Interaction:**

```
Enter your choice: 1

[+] Deleting hidden registry key in HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
[+] Key successfully deleted.
```

**Notes:**
* Administrator privileges are necessary for options 2, 3, and when entering custom paths under HKLM or most HKCR keys.
* Choose option 4 for paths not listed, ensuring you have the necessary permissions.

## Visual Studio Solution & Project
If you'd like to build SharpDelete from source, you can download the **Visual Studio Solution and Project** from the [Releases](https://github.com/andrew-petrus/SharpDelete/releases) section. This will allow you to customize the tool as you like.

## How to Use
1. Download the latest release from the [Releases](https://github.com/andrew-petrus/SharpDelete/releases) section.
2. Choose between:
    - **Precompiled Binary**: Run the executable directly.
    - **Visual Studio Solution**: Open the solution in Visual Studio, build the project, and run the executable.
3. Simply execute SharpDelete.exe from the GUI or CMD to get started.

## Author
SharpDelete by [Andrew Petrus](https://au.linkedin.com/in/andrew-petrus-b1131918b).

## Credits
SharpDelete is designed to detect and remove hidden registry values, including those created by **SharpHide**. Special thanks to:

- **ewhitehats** for the original whitepaper on the technique, ["Invisible Persistence: Hidden Registry Values"](https://github.com/ewhitehats/InvisiblePersistence/blob/master/InvisibleRegValues_Whitepaper.pdf), which laid the groundwork for tools like SharpHide.

- **SharpHide** - [GitHub Repository](https://github.com/outflanknl/SharpHide)
