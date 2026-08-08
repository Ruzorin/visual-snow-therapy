using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace VisualSnowScreen.Models;

/// <summary>
/// Render modu: Overlay (WPF click-through pencere) veya Gamma (donanımsal gamma LUT).
/// </summary>
public enum RenderMode
{
    /// <summary>Hassas FL-41 renk + opaklık; exclusive fullscreen dışında çalışır.</summary>
    Overlay,

    /// <summary>Donanımsal gamma LUT; fullscreen oyun dahil her yerde, sıfır overlay yükü.</summary>
    Gamma
}

/// <summary>
/// FL-41 renk preset'leri. 480-520nm mavi-yeşil blokajını taklit eden rose-amber tonlar.
/// </summary>
public static class Fl41Presets
{
    public sealed record Preset(string Name, string Hex, byte DefaultAlpha, string Description)
    {
        public Color Color => (Color)ColorConverter.ConvertFromString(Hex);
    }

    /// <summary>Indoor FL-41 — hafif rose, ofis/ekran için (~%25 blokaj hissi).</summary>
    public static readonly Preset Indoor = new("Indoor FL-41", "#E0A9AF", 90, "Hafif gül kurusu — ofis ve ekran için");

    /// <summary>Warm FL-41 — daha sıcak, turuncu-kahve alt tonlu (gerçek FL-41'e yakın).</summary>
    public static readonly Preset Warm = new("Warm FL-41", "#D98C8C", 110, "Sıcak rose-amber — gerçek FL-41 tonu");

    /// <summary>Deep FL-41 — yoğun, koyu ortam / gece kullanımı.</summary>
    public static readonly Preset Deep = new("Deep FL-41", "#C97F7F", 140, "Yoğun — koyu ortam / gece");

    /// <summary>Tüm preset listesi (UI seçimi için).</summary>
    public static readonly Preset[] All = { Indoor, Warm, Deep };
}

/// <summary>
/// Uygulama ayarları — JSON'a persist edilir.
/// </summary>
public sealed class AppSettings
{
    /// <summary>Aktif render modu.</summary>
    public RenderMode Mode { get; set; } = RenderMode.Overlay;

    /// <summary>FL-41 renk hex kodu (preset veya custom).</summary>
    public string ColorHex { get; set; } = Fl41Presets.Indoor.Hex;

    /// <summary>Opaklık 0-255 (alpha). Varsayılan ~%35.</summary>
    public byte Opacity { get; set; } = Fl41Presets.Indoor.DefaultAlpha;

    /// <summary>Filtre aktif mi?</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Windows açılışında otomatik başlat.</summary>
    public bool AutoStart { get; set; } = false;

    /// <summary>20-20-20 göz molası zorunlu (engelleme uyarısı, isteğe bağlı).</summary>
    public bool ForcedEyeBreak { get; set; } = false;

    /// <summary>Gamma modu yoğunluğu 0.0-1.0 (gamma LUT kaydırma miktarı).</summary>
    public double GammaIntensity { get; set; } = 0.45;

    /// <summary>Donanım render yerine yazılım render (Intel iGPU sorunlarında fallback).</summary>
    public bool SoftwareRendering { get; set; } = false;

    /// <summary>Ayar penceresi X koordinatı (persist).</summary>
    public double SettingsLeft { get; set; } = double.NaN;

    /// <summary>Ayar penceresi Y koordinatı (persist).</summary>
    public double SettingsTop { get; set; } = double.NaN;
}