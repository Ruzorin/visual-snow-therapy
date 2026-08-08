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

  private void Window_Loaded(object sender, RoutedEventArgs e)
  {
    // Dil ComboBox'ını mevcut dile göre seç.
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

  /// <summary>Sistem diline göre tüm UI string'lerini uygular.</summary>
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
    ChkAutoStart.Content = L("AutoStart");
    ChkForcedEyeBreak.Content = LocalizationService.S("ForcedEyeBreak");
    BtnReliefText.Text = L("OpenRelief");
    LblLanguage.Text = LocalizationService.S("Language");
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

  private void Language_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  {
    if (CmbLanguage.SelectedItem is System.Windows.Controls.ComboBoxItem item && item.Tag is string tag &&
        Enum.TryParse<LocalizationService.AppCulture>(tag, out var culture))
    {
      LocalizationService.Set(culture);
      ApplyLocalization();
    }
  }

  private void Close_Click(object sender, RoutedEventArgs e) => Close();

  protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
  {
    e.Cancel = true;
    Hide();
    base.OnClosing(e);
  }
}