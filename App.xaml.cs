using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using VisualSnowScreen.Services;
using VisualSnowScreen.UI;
using Application = System.Windows.Application;

namespace VisualSnowScreen;

/// <summary>
/// Uygulama giriş noktası. Tüm servisleri kurar, ayarları yükler, filtreyi uygular,
/// tray ikonu + global kısayollar + ayar panelini başlatır.
///
/// GPU/pil optimizasyonu:
///  - SoftwareRendering ayarı açıksa RenderOptions.ProcessRenderMode = SoftwareOnly
///    (Intel iGPU yüksek bellek tüketimi sorununda fallback).
///  - Aksi halde donanım render (statik tek-renk overlay için GPU maliyeti ~sıfır).
///  - Gamma ramp modunda hiç WPF render yok — en düşük yük.
/// </summary>
public partial class App : Application
{
  private SettingsService? _settings;
  private FilterController? _filter;
  private TrayIconController? _tray;
  private HotkeyService? _hotkeys;
  private SettingsWindow? _settingsWindow;
  private SettingsViewModel? _vm;
  private ReliefWindow? _reliefWindow;

  // Gamma ramp sürücü reset'lerine karşı periyodik yenileme (her 30 sn).
  private System.Threading.Timer? _gammaRefreshTimer;
  // Monitör yapısı değişimlerini dinleme (overlay yeniden yerleşim).
  private System.Windows.Threading.DispatcherTimer? _monitorCheckTimer;
  // 20-20-20 göz molası otomatik hatırlatma (varsayılan 20 dk).
  private System.Windows.Threading.DispatcherTimer? _eyeBreakReminder;
  private bool _eyeBreakReminderShown;

  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    _settings = new SettingsService();
    _settings.Load();

    // Render modu (donanım vs yazılım) — Intel iGPU fallback.
    if (_settings.Current.SoftwareRendering)
      RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

    _filter = new FilterController(_settings);
    _filter.Apply();

    _vm = new SettingsViewModel(_settings, _filter);

    _tray = new TrayIconController(_settings, _filter);
    _tray.SettingsRequested += ShowSettings;
    _tray.ReliefRequested += ShowRelief;

    _hotkeys = new HotkeyService();
    _hotkeys.ToggleRequested += () =>
    {
      _settings.Current.Enabled = !_settings.Current.Enabled;
      _settings.Save();
      _filter.Apply();
    };
    _hotkeys.SettingsRequested += ShowSettings;
    _hotkeys.ModeSwitchRequested += () => _filter.SwitchMode();
    _hotkeys.ReliefRequested += ShowRelief;
    _hotkeys.Register();

    // 20-20-20 otomatik hatırlatma: her 20 dk'da bir bildirim.
    _eyeBreakReminder = new System.Windows.Threading.DispatcherTimer
    {
      Interval = TimeSpan.FromMinutes(20)
    };
    _eyeBreakReminder.Tick += (_, _) => ShowEyeBreakReminder();
    _eyeBreakReminder.Start();

    // Gamma modu periyodik yenileme (sürücü reset koruması).
    _gammaRefreshTimer = new System.Threading.Timer(
        _ => Current.Dispatcher.Invoke(() => _filter.RefreshGamma()),
        null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

    // Monitör değişim kontrolü (her 5 sn, ucuz).
    _monitorCheckTimer = new System.Windows.Threading.DispatcherTimer
    {
      Interval = TimeSpan.FromSeconds(5)
    };
    _monitorCheckTimer.Tick += (_, _) => _filter.RefreshLayout();
    _monitorCheckTimer.Start();

    // İlk açılışta ayar panelini göster ki kullanıcı opaklığı ayarlasın.
    ShowSettings();
  }

  private void ShowSettings()
  {
    if (_vm == null) return;
    _settingsWindow ??= new SettingsWindow(_vm);
    _settingsWindow.Show();
    _settingsWindow.Activate();
  }

  private void ShowRelief()
  {
    _reliefWindow ??= new ReliefWindow();
    _reliefWindow.Show();
    _reliefWindow.Activate();
  }

  private void ShowEyeBreakReminder()
  {
    if (_eyeBreakReminderShown) return;
    _eyeBreakReminderShown = true;
    var msg = "20 dakika geçti!\n\n20 saniye boyunca ekrandan uzaklaşıp\n20 feet (6m) uzağa bak — gözlerini dinlendir.\n\n(Visual Snow Initiative önerisi)";
    var result = System.Windows.MessageBox.Show(msg, "20-20-20 Göz Molası",
      MessageBoxButton.OKCancel, MessageBoxImage.Information);
    _eyeBreakReminderShown = false;
    if (result == MessageBoxResult.OK)
    {
      // Relief penceresini 20-20-20 sekmesinde aç.
      _reliefWindow ??= new ReliefWindow();
      _reliefWindow.Show();
      _reliefWindow.Activate();
    }
  }

  protected override void OnExit(ExitEventArgs e)
  {
    _gammaRefreshTimer?.Dispose();
    _monitorCheckTimer?.Stop();
    _eyeBreakReminder?.Stop();
    _hotkeys?.Dispose();
    _tray?.Dispose();
    _filter?.Dispose(); // gamma'yı geri yükler
    base.OnExit(e);
  }
}