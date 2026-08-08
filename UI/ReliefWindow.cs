using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VisualSnowScreen.Native;

namespace VisualSnowScreen.UI;

/// <summary>
/// VSS rahatlama penceresi: 4-7-8 nefes, 20-20-20 göz molası, deneysel habituation,
/// ve farkındalık bilgisi (resmi kaynak linkleriyle).
///
/// Kaynaklar:
///  - Visual Snow Initiative (vss-tips, chromatic-filters)
///  - PMC10939838 — FL-41 fotofobi nöral yol azaltması
///  - PMC11930237 — Filtreli camlar VSS semptom azaltması
///  - EyeWiki — Visual Snow
/// </summary>
public partial class ReliefWindow : Window
{
  private DispatcherTimer? _breathTimer;
  private DispatcherTimer? _breakTimer;
  private int _breathPhase; // 0=inhale, 1=hold, 2=exhale
  private int _breathCount;
  private int _breathCycles;
  private int _breakSeconds;

  public ReliefWindow()
  {
    InitializeComponent();
    ShowTab("breath");
    SourceInitialized += (_, _) => MakeWin32Topmost();
  }

  /// <summary>Win32 seviyesinde topmost yapar — overlay'in (WS_EX_TOPMOST) üstüne geçer.</summary>
  private void MakeWin32Topmost()
  {
    var helper = new WindowInteropHelper(this);
    if (helper.Handle == IntPtr.Zero) return;
    NativeMethods.SetWindowPos(helper.Handle, NativeMethods.HWND_TOPMOST,
        0, 0, 0, 0,
        NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
  }

  private void Tab_Click(object sender, RoutedEventArgs e)
  {
    if (sender is FrameworkElement fe && fe.Tag is string tab)
      ShowTab(tab);
  }

  private void ShowTab(string tab)
  {
    PanelBreathing.Visibility = tab == "breath" ? Visibility.Visible : Visibility.Collapsed;
    PanelEyeBreak.Visibility = tab == "break" ? Visibility.Visible : Visibility.Collapsed;
    PanelHabituation.Visibility = tab == "habit" ? Visibility.Visible : Visibility.Collapsed;
    PanelInfo.Visibility = tab == "info" ? Visibility.Visible : Visibility.Collapsed;

    // Aktif sekme vurgusu
    var active = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xCD, 0xD6, 0xF4));
    var inactive = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xA6, 0xAD, 0xC8));
    BtnBreathing.Foreground = tab == "breath" ? active : inactive;
    BtnEyeBreak.Foreground = tab == "break" ? active : inactive;
    BtnHabituation.Foreground = tab == "habit" ? active : inactive;
    BtnInfo.Foreground = tab == "info" ? active : inactive;
  }

  // ===== 4-7-8 Nefes =====
  private void Breathing_Start(object sender, RoutedEventArgs e)
  {
    _breathCycles = 0;
    _breathPhase = 0;
    _breathCount = 4;
    BreathPhase.Text = "Nefes Al";
    BreathCount.Text = "4";

    _breathTimer?.Stop();
    _breathTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
    _breathTimer.Tick += BreathTick;
    _breathTimer.Start();
    AnimateBreathCircle(0.6, 1.0, 4); // inhale: büyü
  }

  private void BreathTick(object? sender, EventArgs e)
  {
    _breathCount--;
    if (_breathCount > 0)
    {
      BreathCount.Text = _breathCount.ToString();
      return;
    }

    // Faz değiştir
    _breathPhase++;
    switch (_breathPhase)
    {
      case 1: // Hold 7
        _breathCount = 7;
        BreathPhase.Text = "Tut";
        BreathCount.Text = "7";
        AnimateBreathCircle(1.0, 1.0, 7); // sabit
        break;
      case 2: // Exhale 8
        _breathCount = 8;
        BreathPhase.Text = "Ver";
        BreathCount.Text = "8";
        AnimateBreathCircle(1.0, 0.6, 8); // küçül
        break;
      case 3: // döngü tamam
        _breathCycles++;
        if (_breathCycles >= 4)
        {
          _breathTimer?.Stop();
          BreathPhase.Text = "Tamamlandı";
          BreathCount.Text = "✓";
          AnimateBreathCircle(BreathScale.ScaleX, 0.6, 1);
          return;
        }
        _breathPhase = 0;
        _breathCount = 4;
        BreathPhase.Text = "Nefes Al";
        BreathCount.Text = "4";
        AnimateBreathCircle(0.6, 1.0, 4);
        break;
    }
  }

  private void AnimateBreathCircle(double from, double to, int seconds)
  {
    var anim = new DoubleAnimation(from, to, TimeSpan.FromSeconds(seconds))
    {
      EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
    };
    BreathScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, anim);
    BreathScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, anim);
  }

  // ===== 20-20-20 Göz Molası =====
  private void EyeBreak_Start(object sender, RoutedEventArgs e)
  {
    _breakSeconds = 20;
    BreakCountdown.Text = "20";

    _breakTimer?.Stop();
    _breakTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
    _breakTimer.Tick += BreakTick;
    _breakTimer.Start();
  }

  private void BreakTick(object? sender, EventArgs e)
  {
    _breakSeconds--;
    if (_breakSeconds > 0)
    {
      BreakCountdown.Text = _breakSeconds.ToString();
    }
    else
    {
      _breakTimer?.Stop();
      BreakCountdown.Text = "✓";
    }
  }

  // ===== Habituation (deneysel) =====
  private void Habituation_Start(object sender, RoutedEventArgs e)
  {
    int seconds = Habit30.IsChecked == true ? 30 : Habit60.IsChecked == true ? 60 : 120;
    var habit = new HabituationWindow(seconds)
    {
      Owner = this,
      WindowStartupLocation = WindowStartupLocation.CenterOwner
    };
    habit.Show();
  }

  private void Link_Click(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
  {
    try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true }); } catch { }
    e.Handled = true;
  }

  protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
  {
    _breathTimer?.Stop();
    _breakTimer?.Stop();
    e.Cancel = true;
    Hide();
    base.OnClosing(e);
  }
}