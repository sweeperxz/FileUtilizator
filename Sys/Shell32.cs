using System;
using System.Runtime.InteropServices;

namespace FileUtilizator;

public static class Shell32
{
    [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
    public static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);
}