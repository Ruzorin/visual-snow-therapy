using System.IO;
using System.Text.Json;
using VisualSnowScreen.Models;

namespace VisualSnowScreen.Services;

/// <summary>
/// Ayarları %AppData%\VisualSnowScreen\settings.json'a persist eder.
/// Thread-safe, atomik yazma.
/// </summary>
public sealed class SettingsService
{
  private static readonly string SettingsDir =
      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VisualSnowScreen");

  private static readonly string SettingsPath = Path.Combine(SettingsDir, "settings.json");

  private static readonly JsonSerializerOptions JsonOpts = new()
  {
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public AppSettings Current { get; private set; } = new();

  public event EventHandler? SettingsChanged;

  /// <summary>Ayarları diskten yükler; yoksa varsayılan döner.</summary>
  public AppSettings Load()
  {
    try
    {
      if (File.Exists(SettingsPath))
      {
        var json = File.ReadAllText(SettingsPath);
        Current = JsonSerializer.Deserialize<AppSettings>(json, JsonOpts) ?? new AppSettings();
      }
    }
    catch
    {
      // Bozuk dosya → varsayılana dön, kilitlenme yok.
      Current = new AppSettings();
    }
    return Current;
  }

  /// <summary>Ayarları atomik olarak diske yazar.</summary>
  public void Save(AppSettings? settings = null)
  {
    Current = settings ?? Current;
    try
    {
      Directory.CreateDirectory(SettingsDir);
      var json = JsonSerializer.Serialize(Current, JsonOpts);
      // Atomik yazma: geçici dosya + replace.
      var tmp = SettingsPath + ".tmp";
      File.WriteAllText(tmp, json);
      if (File.Exists(SettingsPath))
        File.Replace(tmp, SettingsPath, null);
      else
        File.Move(tmp, SettingsPath);
      SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
    catch
    {
      // Yazma hatası sessiz geç — uygulama çalışmaya devam etmeli.
    }
  }
}