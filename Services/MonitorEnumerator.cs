using System.Windows;
using VisualSnowScreen.Native;

namespace VisualSnowScreen.Services;

/// <summary>
/// Tek bir monitörün fiziksel koordinat ve boyut bilgisi (virtual screen uzayında).
/// </summary>
public sealed record MonitorInfo(IntPtr Handle, Rect Bounds, bool IsPrimary)
{
  public double Width => Bounds.Width;
  public double Height => Bounds.Height;
}

/// <summary>
/// Sistemdeki tüm monitörleri enumerate eder. Per-monitor overlay pencere yerleşimi için.
/// </summary>
public static class MonitorEnumerator
{
  /// <summary>Tüm monitörleri döndürür (virtual screen koordinatlarında).</summary>
  public static List<MonitorInfo> GetAll()
  {
    var list = new List<MonitorInfo>();
    NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
        (IntPtr hMon, IntPtr hdc, ref NativeMethods.RECT rc, IntPtr data) =>
        {
          var info = new NativeMethods.MONITORINFO { cbSize = System.Runtime.InteropServices.Marshal.SizeOf<NativeMethods.MONITORINFO>() };
          if (NativeMethods.GetMonitorInfo(hMon, ref info))
          {
            var bounds = new Rect(rc.Left, rc.Top, rc.Width, rc.Height);
            list.Add(new MonitorInfo(hMon, bounds, (info.dwFlags & NativeMethods.MONITORINFOF_PRIMARY) != 0));
          }
          return true;
        }, IntPtr.Zero);
    return list;
  }
}