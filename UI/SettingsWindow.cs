using System.Windows;
using VisualSnowScreen.Models;
using VisualSnowScreen.Services;

namespace VisualSnowScreen.UI;

public partial class SettingsWindow : Window
{
  private readonly SettingsViewModel _vm;

  public event Action? ReliefRequested;

  public SettingsWindow(SettingsViewModel vm)
  {
    InitializeComponent();
    _vm = vm;
    DataContext = _vm;
    // Mevcut moda göre RadioButton işaretle.
    if (_vm.Mode == RenderMode.Overlay) RbOverlay.IsChecked = true;
    else RbGamma.IsChecked = true;
  }

  private void Window_Loaded(object sender, RoutedEventArgs e) => ApplyLocalization();

  /// <summary>Sistem diline göre tüm UI string'lerini uygular.</summary>
  private void ApplyLocalization()
  {
    var L = LocalizationService.S;
    Title = L("SettingsTitle");
    LblSubtitle.Text = LocalizationService.Current == LocalizationService.AppCulture.Turkish
        ? "480-520nm mavi-yeşil blokajı · rose-amber"
        : LocalizationService.Current == LocalizationService.AppCulture.French
        ? "Bloc bleu-vert 480-520nm · rose-ambre"
        : LocalizationService.Current == LocalizationService.AppCulture.German
        ? "Blau-Grün-Block 480-520nm · Rose-Bernstein"
        : "480-520nm blue-green block · rose-amber";

    ChkEnabled.Content = L("FilterActive");
    LblRenderMode.Text = L("RenderMode");
    RbOverlay.Content = L("OverlayDesc");
    RbGamma.Content = L("GammaDesc");
    LblPreset.Text = L("ColorPreset");
    LblOpacity.Text = L("Opacity");
    LblGammaIntensity.Text = L("GammaIntensity");
    LblNightLightNote.Text = L("NightLightNote");
    ChkAutoStart.Content = L("AutoStart");
    BtnOpenRelief.Content = L("OpenRelief");
    LblHotkeys.Text = L("Hotkeys");
    LblHkToggle.Text = L("HkToggle");
    LblHkSettings.Text = L("HkSettings");
    LblHkMode.Text = L("HkMode");
    LblHkRelief.Text = L("HkRelief");
    BtnClose.Content = L("Close");
  }

  private void Preset_Click(object sender, RoutedEventArgs e)
  {
    if (sender is FrameworkElement fe && fe.Tag is string hex)
      _vm.SelectedPresetHex = hex;
  }

  private void Overlay_Checked(object sender, RoutedEventArgs e) => _vm.Mode = RenderMode.Overlay;
  private void Gamma_Checked(object sender, RoutedEventArgs e) => _vm.Mode = RenderMode.Gamma;

  private void OpenRelief_Click(object sender, RoutedEventArgs e) => ReliefRequested?.Invoke();

  private void Close_Click(object sender, RoutedEventArgs e) => Close();

  protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
  {
    e.Cancel = true;
    Hide();
    base.OnClosing(e);
  }
}