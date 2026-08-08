using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VisualSnowScreen.Native;
using VisualSnowScreen.Services;

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
    ApplyLocalization();
    ShowTab("breath");
    SourceInitialized += (_, _) => MakeWin32Topmost();
  }

  private void ApplyLocalization()
  {
    Title = LocalizationService.S("reliefTitle") ?? "Visual Snow — Rahatlama ve Farkındalık";
    TxtHeaderTitle.Text = LocalizationService.S("reliefHeaderTitle") ?? "Rahatlama ve Farkındalık";
    TxtHeaderSubtitle.Text = LocalizationService.S("reliefHeaderSubtitle") ?? "Visual Snow Syndrome için kanıt destekli rahatlama teknikleri";

    BtnBreathing.Content = LocalizationService.S("reliefTabBreathing") ?? "4-7-8 Nefes";
    BtnEyeBreak.Content = LocalizationService.S("reliefTabEyeBreak") ?? "20-20-20 Göz Molası";
    BtnNort.Content = LocalizationService.S("reliefTabNort") ?? "NORT Egzersizi";
    BtnHabituation.Content = LocalizationService.S("reliefTabHabituation") ?? "Alışma (Deneysel)";
    BtnInfo.Content = LocalizationService.S("reliefTabInfo") ?? "Bilgi";

    // Breathing
    TxtBreathTitle.Text = LocalizationService.S("reliefBreathTitle") ?? "4-7-8 Nefes Tekniği";
    TxtBreathDesc.Text = LocalizationService.S("reliefBreathDesc") ?? "Parasympatik sinir sistemini aktive eder, stresi ve fotofobiyi azaltır.";
    BtnBreathStart.Content = LocalizationService.S("reliefBreathStart") ?? "Başlat (4 döngü)";
    TxtBreathHint.Text = LocalizationService.S("reliefBreathHint") ?? "4 sn nefes al → 7 sn tut → 8 sn ver";
    BreathPhase.Text = LocalizationService.S("reliefBreathReady") ?? "Hazır";

    // Eye break
    TxtBreakTitle.Text = LocalizationService.S("reliefBreakTitle") ?? "20-20-20 Kuralı";
    TxtBreakDesc.Text = LocalizationService.S("reliefBreakDesc") ?? "Her 20 dakikada 20 sn boyunca 20 ft (6m) uzağa bak.";
    BtnBreakStart.Content = LocalizationService.S("reliefBreakStart") ?? "Şimdi Göz Molası Ver";
    TxtBreakHint.Text = LocalizationService.S("reliefBreakHint") ?? "Uzağa bakarken gözlerini kırpma — sakin, yumuşak bakış.";
    TxtBreakSeconds.Text = LocalizationService.S("reliefBreakSeconds") ?? "saniye";

    // NORT
    TxtNortTitle.Text = LocalizationService.S("reliefNortTitle") ?? "NORT: Göz Takibi ve Sıçrama Egzersizleri";
    BtnNortStart.Content = LocalizationService.S("reliefNortStart") ?? "NORT Egzersizini Başlat";

    // Habituation
    TxtHabitTitle.Text = LocalizationService.S("reliefHabitTitle") ?? "Alışma (Habituation) — Deneysel";
    TxtHabitDuration.Text = LocalizationService.S("reliefHabitDuration") ?? "Süre seç:";
    TxtHabitWarning.Text = LocalizationService.S("reliefHabitWarning") ?? "Uyarı: Bu deneysel bir tekniktir. Rahatsızlık artarsa hemen durdurun.";
    BtnHabitStart.Content = LocalizationService.S("reliefHabitStart") ?? "Statik Ekranı Göster";

    // Info
    TxtInfoTitle.Text = LocalizationService.S("reliefInfoTitle") ?? "Visual Snow Syndrome Hakkında";
    TxtInfoFl41Title.Text = LocalizationService.S("reliefInfoFl41Title") ?? "Fotofobi için FL-41";
    TxtInfoTipsTitle.Text = LocalizationService.S("reliefInfoTipsTitle") ?? "Günlük Yönetim İpuçları";

    // Sources
    TxtSourceTitle.Text = LocalizationService.S("reliefSourcesTitle") ?? "Kaynaklar (resmi araştırma):";
    TxtMedicalDisclaimer.Text = LocalizationService.S("reliefMedicalDisclaimer") ?? "Tıbbi tavsiye yerine geçmez. Semptomlar için nöro-oftalmoloğa başvurun.";
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
    PanelNort.Visibility = tab == "nort" ? Visibility.Visible : Visibility.Collapsed;
    PanelHabituation.Visibility = tab == "habit" ? Visibility.Visible : Visibility.Collapsed;
    PanelInfo.Visibility = tab == "info" ? Visibility.Visible : Visibility.Collapsed;

    // Aktif sekme vurgusu
    var active = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xCD, 0xD6, 0xF4));
    var inactive = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xA6, 0xAD, 0xC8));
    BtnBreathing.Foreground = tab == "breath" ? active : inactive;
    BtnEyeBreak.Foreground = tab == "break" ? active : inactive;
    BtnNort.Foreground = tab == "nort" ? active : inactive;
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

  // ===== NORT Therapy =====
  private void Nort_Start(object sender, RoutedEventArgs e)
  {
    var nort = new NortTherapyWindow
    {
      Owner = this,
      WindowStartupLocation = WindowStartupLocation.CenterOwner
    };
    nort.Show();
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