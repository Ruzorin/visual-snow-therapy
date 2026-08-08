using System.Windows.Media;
using VisualSnowScreen.Gamma;
using VisualSnowScreen.Models;
using VisualSnowScreen.Overlay;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace VisualSnowScreen.Services;

/// <summary>
/// Merkezi filtre denetleyicisi: aktif moda göre Overlay veya Gamma ramp yöntemini
/// uygular. Ayar değişimlerinde tek noktadan güncelleme sağlar.
/// </summary>
public sealed class FilterController : IDisposable
{
  private readonly SettingsService _settings;
  private readonly OverlayWindowManager _overlay = new();
  private readonly GammaRampManager _gamma = new();

  public FilterController(SettingsService settings)
  {
    _settings = settings;
  }

  /// <summary>Aktif ayarlara göre filtreyi uygular (açık/kapalı + mod + renk).</summary>
  public void Apply()
  {
    var s = _settings.Current;
    var color = ParseColor(s.ColorHex);

    if (!s.Enabled)
    {
      _overlay.Hide();
      _gamma.Restore();
      return;
    }

    switch (s.Mode)
    {
      case RenderMode.Overlay:
        _gamma.Restore();
        _overlay.ApplyTint(color, s.Opacity);

        // Smart screen filter (noise) is safe-by-design here since it won't be active on Gamma
        _overlay.SetNoiseState(s.SmartNoiseEnabled, (byte)(s.SmartNoiseOpacity * 2.55));

        _overlay.Show();
        break;

      case RenderMode.Gamma:
        _overlay.Hide();
        _gamma.Apply(s.GammaIntensity, color);
        break;
    }
  }

  /// <summary>Akıllı kumlanma durumunu günceller.</summary>
  public void UpdateNoise()
  {
    if (_settings.Current.Mode == RenderMode.Overlay && _settings.Current.Enabled)
    {
      var s = _settings.Current;

      // Calculate opacity for rendering: mapping 1-10 slider to rough byte 5-25 range.
      byte actualOpacity = (byte)(s.SmartNoiseOpacity * 2.55);

      _overlay.SetNoiseState(s.SmartNoiseEnabled, actualOpacity);
    }
  }

  /// <summary>Sadece opaklık/renk değiştiğinde hafif güncelleme (mod değişmeden).</summary>
  public void UpdateTint()
  {
    var s = _settings.Current;
    var color = ParseColor(s.ColorHex);
    if (s.Enabled)
    {
      if (s.Mode == RenderMode.Overlay)
        _overlay.ApplyTint(color, s.Opacity);
      else
        _gamma.Apply(s.GammaIntensity, color);
    }
  }

  /// <summary>Mod değiştir (Overlay ↔ Gamma) ve uygula.</summary>
  public void SwitchMode()
  {
    var s = _settings.Current;
    s.Mode = s.Mode == RenderMode.Overlay ? RenderMode.Gamma : RenderMode.Overlay;
    _settings.Save();
    Apply();
  }

  /// <summary>Monitör yapısı değiştiğinde overlay pencerelerini yeniden yerleştir.</summary>
  public void RefreshLayout()
  {
    if (_settings.Current is { Enabled: true, Mode: RenderMode.Overlay })
      _overlay.RefreshLayout();
  }

  /// <summary>Gamma ramp'i yenile (sürücü reset sonrası periyodik).</summary>
  public void RefreshGamma()
  {
    if (_settings.Current is { Enabled: true, Mode: RenderMode.Gamma })
    {
      var color = ParseColor(_settings.Current.ColorHex);
      _gamma.Refresh(_settings.Current.GammaIntensity, color);
    }
  }

  /// <summary>Overlay topmost z-order'ı pekiştirir (önizleme/alt pencere sorunu).</summary>
  public void ReinforceTopmost()
  {
    if (_settings.Current is { Enabled: true, Mode: RenderMode.Overlay })
      _overlay.ReinforceTopmost();
  }

  private static Color ParseColor(string hex)
  {
    try { return (Color)ColorConverter.ConvertFromString(hex); }
    catch { return (Color)ColorConverter.ConvertFromString(Fl41Presets.Indoor.Hex); }
  }

  public void Dispose()
  {
    _gamma.Dispose();
    _overlay.Dispose();
  }
}