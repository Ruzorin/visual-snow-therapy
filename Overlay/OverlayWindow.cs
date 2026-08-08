using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using VisualSnowScreen.Native;
using Color = System.Windows.Media.Color;

namespace VisualSnowScreen.Overlay;

/// <summary>
/// Tek bir monitörü kaplayan, tıklamaları geçiren (click-through), FL-41 renkli
/// yarı saydam üst katman penceresi.
///
/// Mekanizma:
///  - AllowsTransparency=True → WPF otomatik WS_EX_LAYERED ekler (per-pixel alpha).
///  - SourceInitialized'da HWND'ye WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE eklenir
///    → tıklamalar alt pencereye geçer, Alt-Tab/taskbar'da görünmez, odak çalmaz.
///  - Arka plan FL-41 renkli brush (alpha>0) → "hollow window" tuzağına düşmez.
/// </summary>
public partial class OverlayWindow : Window
{
  private bool _clickThroughApplied;

  public OverlayWindow()
  {
    InitializeComponent();
  }

  /// <summary>FL-41 rengini ve opaklığı uygular. Tek seferlik güncelleme (animasyon yok).</summary>
  public void ApplyTint(Color color, byte alpha)
  {
    var tinted = Color.FromArgb(alpha, color.R, color.G, color.B);
    TintBorder.Background = new SolidColorBrush(tinted);
    // Tek renk dolgu → GPU maliyeti ~sıfır, yeniden composite dışında render yok.
  }

  /// <summary>Pencereyi belirli monitör koordinatlarına yerleştirir (virtual screen).</summary>
  public void PositionOn(Rect bounds)
  {
    Left = bounds.Left;
    Top = bounds.Top;
    Width = bounds.Width;
    Height = bounds.Height;
  }

  protected override void OnSourceInitialized(EventArgs e)
  {
    base.OnSourceInitialized(e);
    ApplyClickThrough();
    ReinforceTopmost();
  }

  /// <summary>HWND'ye click-through + toolwindow + noactivate extended stillerini ekler.</summary>
  private void ApplyClickThrough()
  {
    if (_clickThroughApplied) return;
    var helper = new WindowInteropHelper(this);
    var hwnd = helper.Handle;

    var exStyle = NativeMethods.GetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE);
    exStyle = new IntPtr(exStyle.ToInt64()
        | NativeMethods.WS_EX_TRANSPARENT
        | NativeMethods.WS_EX_LAYERED
        | NativeMethods.WS_EX_TOOLWINDOW
        | NativeMethods.WS_EX_NOACTIVATE);
    NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE, exStyle);
    _clickThroughApplied = true;
  }

  /// <summary>Topmost'u pekiştirir (bazı senaryolarda z-order kaybı olabiliyor).</summary>
  public void ReinforceTopmost()
  {
    var helper = new WindowInteropHelper(this);
    if (helper.Handle == IntPtr.Zero) return;
    NativeMethods.SetWindowPos(helper.Handle, NativeMethods.HWND_TOPMOST,
        0, 0, 0, 0,
        NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
  }
}