using Microsoft.Win32;

namespace VisualSnowScreen.Services;

/// <summary>
/// Windows açılışında otomatik başlatma — CurrentVersion\Run kayıt defteri anahtarı.
/// </summary>
public static class AutoStartService
{
  private const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
  private const string AppName = "VisualSnowScreen";

  public static bool IsEnabled()
  {
    using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath);
    return key?.GetValue(AppName) != null;
  }

  public static void SetEnabled(bool enabled)
  {
    using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, true);
    if (key == null) return;

    if (enabled)
    {
      var exePath = Environment.ProcessPath;
      if (exePath != null)
        key.SetValue(AppName, $"\"{exePath}\"");
    }
    else
    {
      if (key.GetValue(AppName) != null)
        key.DeleteValue(AppName, false);
    }
  }
}