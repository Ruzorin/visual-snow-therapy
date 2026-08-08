using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VisualSnowScreen.Models;

namespace VisualSnowScreen.Services;

/// <summary>
/// Ayar paneli ViewModel — opaklık kaydırıcı, FL-41 preset seçimi, mod toggle,
/// autostart ve gamma yoğunluğu için INotifyPropertyChanged binding kaynağı.
/// </summary>
public sealed class SettingsViewModel : INotifyPropertyChanged
{
  private readonly SettingsService _settings;
  private readonly FilterController _filter;

  public SettingsViewModel(SettingsService settings, FilterController filter)
  {
    _settings = settings;
    _filter = filter;
    var s = settings.Current;
    _selectedPresetHex = s.ColorHex;
    _opacity = s.Opacity;
    _mode = s.Mode;
    _autoStart = s.AutoStart;
    _forcedEyeBreak = s.ForcedEyeBreak;
    _gammaIntensity = s.GammaIntensity;
    _enabled = s.Enabled;
  }

  // ===== FL-41 Preset'leri =====
  public Fl41Presets.Preset[] Presets => Fl41Presets.All;

  private string _selectedPresetHex;
  public string SelectedPresetHex
  {
    get => _selectedPresetHex;
    set
    {
      if (_selectedPresetHex == value) return;
      _selectedPresetHex = value;
      _settings.Current.ColorHex = value;
      // Preset seçilirse opaklığı preset default'una ayarla (ilk seçim).
      var preset = Array.Find(Fl41Presets.All, p => p.Hex == value);
      if (preset != null)
      {
        _settings.Current.Opacity = preset.DefaultAlpha;
        Opacity = preset.DefaultAlpha;
      }
      _settings.Save();
      _filter.UpdateTint();
      OnPropertyChanged();
      OnPropertyChanged(nameof(SelectedPresetName));
    }
  }

  public string SelectedPresetName =>
      Array.Find(Fl41Presets.All, p => p.Hex == SelectedPresetHex)?.Name ?? "Custom";

  // ===== Opaklık (0-255) =====
  private byte _opacity;
  public byte Opacity
  {
    get => _opacity;
    set
    {
      if (_opacity == value) return;
      _opacity = value;
      _settings.Current.Opacity = value;
      _settings.Save();
      _filter.UpdateTint();
      OnPropertyChanged();
      OnPropertyChanged(nameof(OpacityPercent));
    }
  }

  public int OpacityPercent => (int)Math.Round(_opacity / 255.0 * 100);

  // ===== Render Modu =====
  private RenderMode _mode;
  public RenderMode Mode
  {
    get => _mode;
    set
    {
      if (_mode == value) return;
      _mode = value;
      _settings.Current.Mode = value;
      _settings.Save();
      _filter.Apply();
      OnPropertyChanged();
      OnPropertyChanged(nameof(IsOverlayMode));
      OnPropertyChanged(nameof(IsGammaMode));
    }
  }

  public bool IsOverlayMode => _mode == RenderMode.Overlay;
  public bool IsGammaMode => _mode == RenderMode.Gamma;

  // ===== Gamma Yoğunluğu (0.0-1.0) =====
  private double _gammaIntensity;
  public double GammaIntensity
  {
    get => _gammaIntensity;
    set
    {
      if (Math.Abs(_gammaIntensity - value) < 0.001) return;
      _gammaIntensity = value;
      _settings.Current.GammaIntensity = value;
      _settings.Save();
      _filter.UpdateTint();
      OnPropertyChanged();
      OnPropertyChanged(nameof(GammaPercent));
    }
  }

  public int GammaPercent => (int)Math.Round(_gammaIntensity * 100);

  // ===== Filtre aktif =====
  private bool _enabled;
  public bool Enabled
  {
    get => _enabled;
    set
    {
      if (_enabled == value) return;
      _enabled = value;
      _settings.Current.Enabled = value;
      _settings.Save();
      _filter.Apply();
      OnPropertyChanged();
    }
  }

  // ===== AutoStart =====
  private bool _autoStart;
  public bool AutoStart
  {
    get => _autoStart;
    set
    {
      if (_autoStart == value) return;
      _autoStart = value;
      _settings.Current.AutoStart = value;
      AutoStartService.SetEnabled(value);
      _settings.Save();
      OnPropertyChanged();
    }
  }

  // ===== 20-20-20 Zorunlu mod =====
  private bool _forcedEyeBreak;
  public bool ForcedEyeBreak
  {
    get => _forcedEyeBreak;
    set
    {
      if (_forcedEyeBreak == value) return;
      _forcedEyeBreak = value;
      _settings.Current.ForcedEyeBreak = value;
      _settings.Save();
      OnPropertyChanged();
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;
  private void OnPropertyChanged([CallerMemberName] string? name = null)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}