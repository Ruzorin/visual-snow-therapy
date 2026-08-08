using System.Runtime.InteropServices;

namespace VisualSnowScreen.Native;

/// <summary>
/// Win32 API P/Invoke katmanı: click-through layered window, gamma ramp, global hotkey.
/// Tüm native çağrılar burada toplanır.
/// </summary>
internal static class NativeMethods
{
  // ===== Extended window styles =====
  public const int GWL_EXSTYLE = -20;
  public const int WS_EX_LAYERED = 0x00080000;
  public const int WS_EX_TRANSPARENT = 0x00000020;
  public const int WS_EX_TOOLWINDOW = 0x00000080;
  public const int WS_EX_NOACTIVATE = 0x08000000;
  public const int WS_EX_TOPMOST = 0x00000008;

  // ===== SetWindowPos flags =====
  public const int SWP_NOMOVE = 0x0002;
  public const int SWP_NOSIZE = 0x0001;
  public const int SWP_NOACTIVATE = 0x0010;
  public const int SWP_NOZORDER = 0x0004;
  public const int SWP_NOOWNERZORDER = 0x0200;
  public const int SWP_SHOWWINDOW = 0x0040;
  public static readonly IntPtr HWND_TOPMOST = new(-1);
  public static readonly IntPtr HWND_NOTOPMOST = new(-2);

  // ===== ShowWindow =====
  public const int SW_SHOWNOACTIVATE = 4;

  // ===== Hotkey modifiers & messages =====
  public const int MOD_ALT = 0x0001;
  public const int MOD_CONTROL = 0x0002;
  public const int MOD_SHIFT = 0x0004;
  public const int MOD_NOREPEAT = 0x4000;
  public const int WM_HOTKEY = 0x0312;

  // ===== Display =====
  public const int ENUM_CURRENT_SETTINGS = -1;
  public const int DISP_CHANGE_SUCCESSFUL = 0;

  [DllImport("user32.dll", SetLastError = true)]
  public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

  public const uint LWA_COLORKEY = 0x00000001;
  public const uint LWA_ALPHA = 0x00000002;

  [DllImport("user32.dll", SetLastError = true)]
  public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

  [DllImport("user32.dll")]
  public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

  // ===== Gamma Ramp (donanımsal ekran LUT) =====
  // RAMPDWORD = 256 * 3 WORD girişi; her kanal 256 WORD (0-65535).
  [DllImport("gdi32.dll", SetLastError = true)]
  public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

  [DllImport("gdi32.dll", SetLastError = true)]
  public static extern bool GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

  [DllImport("user32.dll")]
  public static extern IntPtr GetDC(IntPtr hWnd);

  [DllImport("user32.dll")]
  public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

  public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

  [DllImport("user32.dll")]
  public static extern IntPtr MonitorFromRect(ref RECT lprc, int dwFlags);

  [StructLayout(LayoutKind.Sequential)]
  public struct RAMP
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public ushort[] Red;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public ushort[] Green;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public ushort[] Blue;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public int Width => Right - Left;
    public int Height => Bottom - Top;
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public struct MONITORINFO
  {
    public int cbSize;
    public RECT rcMonitor;
    public RECT rcWork;
    public uint dwFlags;
  }

  public const int MONITORINFOF_PRIMARY = 0x00000001;
  public const int MONITOR_DEFAULTTONEAREST = 0x00000002;

  // GetWindow constants
  public const int GW_OWNER = 4;

  [DllImport("user32.dll", SetLastError = true)]
  public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
}