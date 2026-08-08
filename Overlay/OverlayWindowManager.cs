using System.Windows;
using System.Windows.Media;
using VisualSnowScreen.Models;
using VisualSnowScreen.Services;
using Color = System.Windows.Media.Color;

namespace VisualSnowScreen.Overlay;

/// <summary>
/// Her monitör için bir OverlayWindow oluşturur/yönetir.
/// Monitör yapısı değiştiğinde (çözünürlük, eklenme) pencereleri yeniden konumlandırır.
/// </summary>
public sealed class OverlayWindowManager : IDisposable
{
  private readonly List<OverlayWindow> _windows = new();
  private Color _color;
  private byte _alpha;
  private bool _shown;

  /// <summary>FL-41 rengini ve opaklığı tüm overlay pencerelerine uygular.</summary>
  public void ApplyTint(Color color, byte alpha)
  {
    _color = color;
    _alpha = alpha;
    foreach (var w in _windows)
      w.ApplyTint(color, alpha);
  }

  /// <summary>Overlay pencerelerini tüm monitörlere yerleştirir ve gösterir.</summary>
  public void Show()
  {
    RecreateWindows();
    _shown = true;
  }

  /// <summary>Tüm overlay pencerelerini gizler (kapatmaz — yeniden açmada hızlı).</summary>
  public void Hide()
  {
    foreach (var w in _windows)
      w.Hide();
    _shown = false;
  }

  /// <summary>Monitör yapısı değiştiğinde pencereleri yeniden oluşturur/konumlandırır.</summary>
  public void RefreshLayout()
  {
    if (!_shown) return;
    RecreateWindows();
  }

  private void RecreateWindows()
  {
    // Mevcut pencereleri kapat.
    foreach (var w in _windows)
    {
      w.Close();
    }
    _windows.Clear();

    var monitors = MonitorEnumerator.GetAll();
    foreach (var mon in monitors)
    {
      var win = new OverlayWindow();
      win.PositionOn(mon.Bounds);
      win.ApplyTint(_color, _alpha);
      win.Show();
      _windows.Add(win);
    }
  }

  /// <summary>Topmost z-order'ı pekiştirir (örn. başka topmost pencere açıldığında).</summary>
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
  }
}