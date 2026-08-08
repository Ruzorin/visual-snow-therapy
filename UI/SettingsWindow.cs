using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VisualSnowScreen.Models;
using VisualSnowScreen.Services;

namespace VisualSnowScreen.UI;

public partial class SettingsWindow : Window
{
  private readonly SettingsViewModel _vm;

  // Relief timers
  private DispatcherTimer? _breathTimer;
  private DispatcherTimer? _breakTimer;
  private int _breathPhase;
  private int _breathCount;
  private int _breathCycles;
  private int _breakSeconds;

  public SettingsWindow(SettingsViewModel vm)
  {
    InitializeComponent();
    _vm = vm;
    DataContext = _vm;
    if (_vm.Mode == RenderMode.Overlay) RbOverlay.IsChecked = true;
    else RbGamma.IsChecked = true;
  }

  private void Window_Loaded(object sender, RoutedEventArgs e)
  {
    var cur = LocalizationService.Current.ToString();
    for (int i = 0; i < CmbLanguage.Items.Count; i++)
    {
      if (CmbLanguage.Items[i] is System.Windows.Controls.ComboBoxItem item && item.Tag?.ToString() == cur)
      {
        CmbLanguage.SelectedIndex = i;
        break;
      }
    }
    ApplyLocalization();
  }

  private void ApplyLocalization()
  {
    var L = LocalizationService.S;
    Title = L("SettingsTitle");
    LblSubtitle.Text = L("Subtitle");

    ChkEnabled.Content = L("FilterActive");
    LblRenderMode.Text = L("RenderMode");
    RbOverlay.Content = L("OverlayDesc");
    RbGamma.Content = L("GammaDesc");
    LblPreset.Text = L("ColorPreset");
    LblOpacity.Text = L("Opacity");
    LblGammaIntensity.Text = L("GammaIntensity");
    LblNightLightNote.Text = L("NightLightNote");
    ChkSmartNoise.Content = L("SmartNoiseLabel");
    LblSmartNoiseWarning.Text = L("SmartNoiseWarning");
    LblSmartNoiseMedicalWarning.Text = L("SmartNoiseMedicalWarning");
    ChkAutoStart.Content = L("AutoStart");
    ChkForcedEyeBreak.Content = L("ForcedEyeBreak");
    BtnReliefText.Text = L("OpenRelief");
    LblLanguage.Text = L("Language");
    LblHotkeys.Text = L("Hotkeys");
    LblHkToggle.Text = L("HkToggle");
    LblHkSettings.Text = L("HkSettings");
    LblHkMode.Text = L("HkMode");
    LblHkRelief.Text = L("HkRelief");
    BtnClose.Content = L("Close");

    // Relief panel localization
    TxtReliefHeaderTitle.Text = L("reliefHeaderTitle") ?? "Rahatlama ve Farkındalık";
    TxtReliefHeaderSubtitle.Text = L("reliefHeaderSubtitle") ?? "Visual Snow Syndrome için kanıt destekli rahatlama teknikleri";
    BtnReliefBack.Content = L("reliefBack") ?? "← Geri";
    BtnBreathing.Content = L("reliefTabBreathing") ?? "4-7-8 Nefes";
    BtnEyeBreak.Content = L("reliefTabEyeBreak") ?? "20-20-20 Göz Molası";
    BtnNort.Content = L("reliefTabNort") ?? "NORT Egzersizi";
    BtnHabituation.Content = L("reliefTabHabituation") ?? "Alışma (Deneysel)";
    BtnInfo.Content = L("reliefTabInfo") ?? "Bilgi";

    TxtBreathTitle.Text = L("reliefBreathTitle") ?? "4-7-8 Nefes Tekniği";
    TxtBreathDesc.Text = L("reliefBreathDesc") ?? "Parasympatik sinir sistemini aktive eder, stresi ve fotofobiyi azaltır.";
    BtnBreathStart.Content = L("reliefBreathStart") ?? "Başlat (4 döngü)";
    TxtBreathHint.Text = L("reliefBreathHint") ?? "4 sn nefes al → 7 sn tut → 8 sn ver";
    BreathPhase.Text = L("reliefBreathReady") ?? "Hazır";

    TxtBreakTitle.Text = L("reliefBreakTitle") ?? "20-20-20 Kuralı";
    TxtBreakDesc.Text = L("reliefBreakDesc") ?? "Her 20 dakikada 20 sn boyunca 20 ft (6m) uzağa bak.";
    BtnBreakStart.Content = L("reliefBreakStart") ?? "Şimdi Göz Molası Ver";
    TxtBreakHint.Text = L("reliefBreakHint") ?? "Uzağa bakarken gözlerini kırpma — sakin, yumuşak bakış.";
    TxtBreakSeconds.Text = L("reliefBreakSeconds") ?? "saniye";

    TxtNortTitle.Text = L("reliefNortTitle") ?? "NORT: Göz Takibi ve Sıçrama Egzersizleri";
    TxtNortDesc1.Text = L("reliefNortDesc1") ?? "";
    TxtNortDesc2.Text = L("reliefNortDesc2") ?? "";
    BtnNortStart.Content = L("reliefNortStart") ?? "NORT Egzersizini Başlat";

    TxtHabitTitle.Text = L("reliefHabitTitle") ?? "Alışma (Habituation) — Deneysel";
    TxtHabitDesc.Text = L("reliefHabitDesc") ?? "";
    TxtHabitDuration.Text = L("reliefHabitDuration") ?? "Süre seç:";
    TxtHabitWarning.Text = L("reliefHabitWarning") ?? "Uyarı: Bu deneysel bir tekniktir. Rahatsızlık artarsa hemen durdurun.";
    BtnHabitStart.Content = L("reliefHabitStart") ?? "Statik Ekranı Göster";

    TxtInfoTitle.Text = L("reliefInfoTitle") ?? "Visual Snow Syndrome Hakkında";
    TxtInfoDesc1.Text = L("reliefInfoDesc1") ?? "";
    TxtInfoDesc2.Text = L("reliefInfoDesc2") ?? "";
    TxtInfoFl41Title.Text = L("reliefInfoFl41Title") ?? "Fotofobi için FL-41";
    TxtInfoFl41Desc.Text = L("reliefInfoFl41Desc") ?? "";
    TxtInfoTipsTitle.Text = L("reliefInfoTipsTitle") ?? "Günlük Yönetim İpuçları";
    TxtInfoTips.Text = L("reliefInfoTips") ?? "";

    TxtSourceTitle.Text = L("reliefSourcesTitle") ?? "Kaynaklar (resmi araştırma):";
    TxtMedicalDisclaimer.Text = L("reliefMedicalDisclaimer") ?? "Tıbbi tavsiye yerine geçmez. Semptomlar için nöro-oftalmoloğa başvurun.";
  }

  private void Preset_Click(object sender, RoutedEventArgs e)
  {
    if (sender is FrameworkElement fe && fe.Tag is string hex)
      _vm.SelectedPresetHex = hex;
  }

  private void Overlay_Checked(object sender, RoutedEventArgs e) => _vm.Mode = RenderMode.Overlay;
  private void Gamma_Checked(object sender, RoutedEventArgs e) => _vm.Mode = RenderMode.Gamma;

  // ===== Relief navigasyon =====
  public void ShowReliefPanel(string tab = "breath")
  {
    SettingsPanel.Visibility = Visibility.Collapsed;
    ReliefPanel.Visibility = Visibility.Visible;
    ShowReliefTab(tab);
  }

  private void OpenRelief_Click(object sender, RoutedEventArgs e)
  {
    ShowReliefPanel("breath");
  }

  private void ReliefBack_Click(object sender, RoutedEventArgs e)
  {
    _breathTimer?.Stop();
    _breakTimer?.Stop();
    ReliefPanel.Visibility = Visibility.Collapsed;
    SettingsPanel.Visibility = Visibility.Visible;
  }

  private void ReliefTab_Click(object sender, RoutedEventArgs e)
  {
    if (sender is FrameworkElement fe && fe.Tag is string tab)
      ShowReliefTab(tab);
  }

  private void ShowReliefTab(string tab)
  {
    PanelBreathing.Visibility = tab == "breath" ? Visibility.Visible : Visibility.Collapsed;
    PanelEyeBreak.Visibility = tab == "break" ? Visibility.Visible : Visibility.Collapsed;
    PanelNort.Visibility = tab == "nort" ? Visibility.Visible : Visibility.Collapsed;
    PanelHabituation.Visibility = tab == "habit" ? Visibility.Visible : Visibility.Collapsed;
    PanelInfo.Visibility = tab == "info" ? Visibility.Visible : Visibility.Collapsed;

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
    BreathPhase.Text = LocalizationService.S("reliefBreathInhale") ?? "Nefes Al";
    BreathCount.Text = "4";

    _breathTimer?.Stop();
    _breathTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
    _breathTimer.Tick += BreathTick;
    _breathTimer.Start();
    AnimateBreathCircle(0.6, 1.0, 4);
  }

  private void BreathTick(object? sender, EventArgs e)
  {
    _breathCount--;
    if (_breathCount > 0)
    {
      BreathCount.Text = _breathCount.ToString();
      return;
    }

    _breathPhase++;
    switch (_breathPhase)
    {
      case 1:
        _breathCount = 7;
        BreathPhase.Text = LocalizationService.S("reliefBreathHold") ?? "Tut";
        BreathCount.Text = "7";
        AnimateBreathCircle(1.0, 1.0, 7);
        break;
      case 2:
        _breathCount = 8;
        BreathPhase.Text = LocalizationService.S("reliefBreathExhale") ?? "Ver";
        BreathCount.Text = "8";
        AnimateBreathCircle(1.0, 0.6, 8);
        break;
      case 3:
        _breathCycles++;
        if (_breathCycles >= 4)
        {
          _breathTimer?.Stop();
          BreathPhase.Text = LocalizationService.S("reliefBreathDone") ?? "Tamamlandı";
          BreathCount.Text = "✓";
          AnimateBreathCircle(BreathScale.ScaleX, 0.6, 1);
          return;
        }
        _breathPhase = 0;
        _breathCount = 4;
        BreathPhase.Text = LocalizationService.S("reliefBreathInhale") ?? "Nefes Al";
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

  private void Language_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  {
    if (CmbLanguage.SelectedItem is System.Windows.Controls.ComboBoxItem item && item.Tag is string tag &&
        Enum.TryParse<LocalizationService.AppCulture>(tag, out var culture))
    {
      LocalizationService.Set(culture);
      ApplyLocalization();
      if (DataContext is SettingsViewModel vm)
      {
        vm.RefreshPresets();
      }
    }
  }

  private void Close_Click(object sender, RoutedEventArgs e) => Close();

  protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
  {
    _breathTimer?.Stop();
    _breakTimer?.Stop();
    e.Cancel = true;
    Hide();
    base.OnClosing(e);
  }
}