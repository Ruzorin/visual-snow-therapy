using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

  // NORT Noise Variables
  private WriteableBitmap? _noiseBitmap;
  private int[]? _noisePixels;
  private int _noiseWidth = 640;
  private int _noiseHeight = 360;
  private bool _isNoiseEnabled = false;
  private int _renderWaitFrames = 0;
  private static readonly Random _rnd = new Random();

  public OverlayWindow()
  {
    InitializeComponent();
    RenderOptions.SetBitmapScalingMode(NoiseImage, BitmapScalingMode.NearestNeighbor);
    CompositionTarget.Rendering += OnCompositionRendering;
  }

  /// <summary>FL-41 rengini ve opaklığı uygular. Tek seferlik güncelleme (animasyon yok).</summary>
  public void ApplyTint(Color color, byte alpha)
  {
    var tinted = Color.FromArgb(alpha, color.R, color.G, color.B);
    TintBorder.Background = new SolidColorBrush(tinted);
    // Tek renk dolgu → GPU maliyeti ~sıfır, yeniden composite dışında render yok.
  }

  /// <summary>Smart Screen Filter gürültü katmanını açar/kapatır.</summary>
  public void SetNoiseState(bool enabled, byte opacity = 7)
  {
    _isNoiseEnabled = enabled;
    if (enabled)
    {
      NoiseImage.Visibility = Visibility.Visible;
      NoiseImage.Opacity = opacity / 255.0; // %2-3 (max 255/100*x)
      if (_noiseBitmap == null)
      {
        _noiseBitmap = new WriteableBitmap(_noiseWidth, _noiseHeight, 96, 96, PixelFormats.Bgra32, null);
        _noisePixels = new int[_noiseWidth * _noiseHeight];
        NoiseImage.Source = _noiseBitmap;
      }
    }
    else
    {
      NoiseImage.Visibility = Visibility.Collapsed;
    }
  }

  private void OnCompositionRendering(object? sender, EventArgs e)
  {
    if (!_isNoiseEnabled || _noiseBitmap == null || _noisePixels == null) return;

    // Throttle FPS to ~30 (e.g. skip every other frame in a 60hz monitor)
    if (++_renderWaitFrames % 2 != 0) return;

    // Fill noise parallel
    Parallel.For(0, _noiseHeight, y =>
    {
      int offset = y * _noiseWidth;
      for (int x = 0; x < _noiseWidth; x++)
      {
        // Black & White Noise (A=255, R=rnd, G=rnd, B=rnd)
        byte grain = (byte)Random.Shared.Next(0, 256);
        _noisePixels[offset + x] = (255 << 24) | (grain << 16) | (grain << 8) | grain;
      }
    });

    _noiseBitmap.WritePixels(new Int32Rect(0, 0, _noiseWidth, _noiseHeight), _noisePixels, _noiseWidth * 4, 0);
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
    MakeOwnerTopmost();
    ReinforceTopmost();
    // WM_ACTIVATE hook: foreground değişiminde hemen topmost'u pekiştir.
    var src = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
    src?.AddHook(WndProc);
  }

  private void MakeOwnerTopmost()
  {
    // WPF ShowInTaskbar=False gizli owner penceresi oluşturur; onu da topmost yap.
    var hwnd = new WindowInteropHelper(this).Handle;
    if (hwnd == IntPtr.Zero) return;
    var owner = NativeMethods.GetWindow(hwnd, NativeMethods.GW_OWNER);
    if (owner != IntPtr.Zero)
    {
      NativeMethods.SetWindowPos(owner, NativeMethods.HWND_TOPMOST,
          0, 0, 0, 0,
          NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
    }
  }

  private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    // WM_ACTIVATE: foreground değişince topmost'u hemen pekiştir.
    if (msg == 0x0006) // WM_ACTIVATE
    {
      ReinforceTopmost();
    }
    // WM_WINDOWPOSCHANGING: başka pencere z-order'ı değiştirmeye çalışırsa.
    if (msg == 0x0046) // WM_WINDOWPOSCHANGING
    {
      ReinforceTopmost();
    }
    return IntPtr.Zero;
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

  protected override void OnClosed(EventArgs e)
  {
    CompositionTarget.Rendering -= OnCompositionRendering;
    base.OnClosed(e);
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