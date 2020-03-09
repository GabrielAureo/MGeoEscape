
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;

public class StandaloneWindowBehaviour : MonoBehaviour{
#if UNITY_STANDALONE && !UNITY_EDITOR

    #region DLL Imports
    private const string UnityWindowClassName = "UnityWndClass";

    [DllImport("kernel32.dll")]
    static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    static extern bool SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);

    #endregion
    
    #region Private fields
    private static IntPtr windowHandle = IntPtr.Zero;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const UInt32 SWP_NOSIZE = 0x0001;
    private const UInt32 SWP_NOMOVE = 0x0002;
    private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
    #endregion
    void Start(){
        uint threadId = GetCurrentThreadId();
        EnumThreadWindows(threadId, (hWnd, lParam) =>
        {
            var classText = new StringBuilder(UnityWindowClassName.Length + 1);
            GetClassName(hWnd, classText, classText.Capacity);
            if (classText.ToString() == UnityWindowClassName)
            {
                windowHandle = hWnd;
                return false;
            }
            return true;
        }, IntPtr.Zero);

        Debug.Log(string.Format("Window Handle: {0}", windowHandle));
        var args = System.Environment.GetCommandLineArgs();

        SetWindowText(windowHandle, string.Format("Player {0} GUID: {1}", args[1], args[2]));
        int p = 1920/3;
        SetWindowPos(windowHandle,  HWND_TOPMOST, (int.Parse(args[1]) - 1) * p, 200, 0, 0, SWP_NOSIZE);
    }
    
#endif
}

