using System.Windows;
using System.Windows.Media;
using VisualSnowScreen.Models;
using VisualSnowScreen.Services;
using Color = System.Windows.Media.Color;

namespace VisualSnowScreen.Overlay;

/// <summary>
/// Her monitör için bir OverlayWindow oluşturur/yönetir.
///
/// Flicker'sız güncelleme: monitör yapısı değişmediyse pencerelere dokunulmaz.
/// Sadece monitör sayısı/koordinat değiştiğinde fark güncellenir (eski kapatılır,
/// yeni açılır — ama yalnızca gerçek değişimde, periyodik olarak değil).
/// </summary>
public sealed class OverlayWindowManager : IDisposable
{
  private readonly List<OverlayWindow> _windows = new();
  private readonly Dictionary<IntPtr, Rect> _monitorBounds = new();
  private Color _color;
  private byte _alpha;
  private bool _shown;

  public void ApplyTint(Color color, byte alpha)
  {
    _color = color;
    _alpha = alpha;
    foreach (var w in _windows)
      w.ApplyTint(color, alpha);
  }

  public void Show()
  {
    EnsureWindows();
    foreach (var w in _windows)
    {
      try { w.Show(); } catch { }
    }
    _shown = true;
  }

  public void Hide()
  {
    foreach (var w in _windows)
      w.Hide();
    _shown = false;
  }

  /// <summary>
  /// Monitör yapısı değiştiyse pencereleri günceller. Değişmediyse HİÇBİR ŞEY yapmaz
  /// (flicker yok). Sadece gerçek değişimde Close/Show olur.
  /// </summary>
  public void RefreshLayout()
  {
    if (!_shown) return;
    EnsureWindows();
  }

  /// <summary>
  /// Monitör yapısını kontrol eder; değişiklik varsa pencereleri diff günceller.
  /// Değişiklik yoksa erken dönüş — flicker yok.
  /// </summary>
  private void EnsureWindows()
  {
    var monitors = MonitorEnumerator.GetAll();
    var currentBounds = monitors.ToDictionary(m => m.Handle, m => m.Bounds);

    // Hızlı yol: monitör sayısı ve koordinatlar aynıysa hiçbir şey yapma.
    if (currentBounds.Count == _monitorBounds.Count &&
        currentBounds.All(kv => _monitorBounds.TryGetValue(kv.Key, out var b) && b == kv.Value))
    {
      return; // değişiklik yok → flicker yok
    }

    // Değişiklik var: eski pencereleri kapat, yenileri oluştur.
    foreach (var w in _windows)
    {
      try { w.Close(); } catch { }
    }
    _windows.Clear();
    _monitorBounds.Clear();

    foreach (var mon in monitors)
    {
      var win = new OverlayWindow();
      win.PositionOn(mon.Bounds);
      win.ApplyTint(_color, _alpha);
      win.Show();
      _windows.Add(win);
      _monitorBounds[mon.Handle] = mon.Bounds;
    }
  }

  /// <summary>Topmost z-order'ı pekiştirir (önizleme/alt pencere sorunu için).</summary>
  public void ReinforceTopmost()
  {
    foreach (var w in _windows)
      w.ReinforceTopmost();
  }

  public void Dispose()
  {
    foreach (var w in _windows)
    {
      try { w.Close(); } catch { }
    }
    _windows.Clear();
    _monitorBounds.Clear();
  }
}