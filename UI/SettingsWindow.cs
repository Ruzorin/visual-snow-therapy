using System.Windows;
using VisualSnowScreen.Models;
using VisualSnowScreen.Services;

namespace VisualSnowScreen.UI;

public partial class SettingsWindow : Window
{
  private readonly SettingsViewModel _vm;

  public SettingsWindow(SettingsViewModel vm)
  {
    InitializeComponent();
    _vm = vm;
    DataContext = _vm;
    // Mevcut moda göre RadioButton işaretle.
    if (_vm.Mode == Models.RenderMode.Overlay) RbOverlay.IsChecked = true;
    else RbGamma.IsChecked = true;
  }

  private void Preset_Click(object sender, RoutedEventArgs e)
  {
    if (sender is FrameworkElement fe && fe.Tag is string hex)
      _vm.SelectedPresetHex = hex;
  }

  private void Overlay_Checked(object sender, RoutedEventArgs e) => _vm.Mode = Models.RenderMode.Overlay;
  private void Gamma_Checked(object sender, RoutedEventArgs e) => _vm.Mode = Models.RenderMode.Gamma;

  private void Close_Click(object sender, RoutedEventArgs e)
  {
    // Pencere kapanırken konumu persist et.
    _vm.AutoStart = _vm.AutoStart; // no-op; settings already saved on change
    Close();
  }

  protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
  {
    // Kapatma yerine gizle — yeniden açmada hızlı (tray'den).
    e.Cancel = true;
    Hide();
    base.OnClosing(e);
  }
}