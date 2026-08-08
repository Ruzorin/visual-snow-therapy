using System.Globalization;

namespace VisualSnowScreen.Services;

/// <summary>
/// Çoklu dil desteği (i18n). Sistem diline göre otomatik seçim: EN, FR, DE, TR.
/// Varsayılan: EN (desteklenmeyen diller için fallback).
/// </summary>
public static class LocalizationService
{
  public enum AppCulture { English, French, German, Turkish }

  public static AppCulture Current { get; private set; } = AppCulture.English;

  /// <summary>Sistem kültürüne göre dili seçer.</summary>
  public static AppCulture Detect()
  {
    var ci = CultureInfo.CurrentUICulture;
    var twoLetter = ci.TwoLetterISOLanguageName.ToUpperInvariant();
    Current = twoLetter switch
    {
      "FR" => AppCulture.French,
      "DE" => AppCulture.German,
      "TR" => AppCulture.Turkish,
      _ => AppCulture.English
    };
    return Current;
  }

  /// <summary>Manuel dil seçimi (ayarlardan).</summary>
  public static void Set(AppCulture culture) => Current = culture;

  // ===== String tablosu =====
  public static string S(string key) => Get(key, Current);

  private static string Get(string key, AppCulture c) =>
      (c, key) switch
      {
        // App / tray
        (_, "AppName") => c == AppCulture.Turkish ? "Visual Snow FL-41 Filtresi"
                        : c == AppCulture.French ? "Filtre FL-41 Visual Snow"
                        : c == AppCulture.German ? "Visual Snow FL-41 Filter"
                        : "Visual Snow FL-41 Filter",
        (_, "FilterOn") => c == AppCulture.Turkish ? "Filtre: AÇIK"
                        : c == AppCulture.French ? "Filtre: ACTIF"
                        : c == AppCulture.German ? "Filter: AN"
                        : "Filter: ON",
        (_, "FilterOff") => c == AppCulture.Turkish ? "Filtre: KAPALI"
                        : c == AppCulture.French ? "Filtre: INACTIF"
                        : c == AppCulture.German ? "Filter: AUS"
                        : "Filter: OFF",
        (_, "Mode") => c == AppCulture.Turkish ? "Mod"
                        : c == AppCulture.French ? "Mode"
                        : c == AppCulture.German ? "Modus"
                        : "Mode",
        (_, "Overlay") => c == AppCulture.Turkish ? "Overlay (FL-41 pencere)"
                        : c == AppCulture.French ? "Overlay (fenêtre FL-41)"
                        : c == AppCulture.German ? "Overlay (FL-41 Fenster)"
                        : "Overlay (FL-41 window)",
        (_, "Gamma") => c == AppCulture.Turkish ? "Gamma (oyun dostu)"
                        : c == AppCulture.French ? "Gamma (mode jeu)"
                        : c == AppCulture.German ? "Gamma (spielmodus)"
                        : "Gamma (game-friendly)",
        (_, "Settings") => c == AppCulture.Turkish ? "Ayarlar..."
                        : c == AppCulture.French ? "Paramètres..."
                        : c == AppCulture.German ? "Einstellungen..."
                        : "Settings...",
        (_, "Relief") => c == AppCulture.Turkish ? "Rahatlama (4-7-8 / 20-20-20)..."
                        : c == AppCulture.French ? "Soulagement (4-7-8 / 20-20-20)..."
                        : c == AppCulture.German ? "Linderung (4-7-8 / 20-20-20)..."
                        : "Relief (4-7-8 / 20-20-20)...",
        (_, "Exit") => c == AppCulture.Turkish ? "Çıkış"
                        : c == AppCulture.French ? "Quitter"
                        : c == AppCulture.German ? "Beenden"
                        : "Exit",

        // Settings window
        (_, "SettingsTitle") => c == AppCulture.Turkish ? "Visual Snow FL-41 — Ayarlar"
                        : c == AppCulture.French ? "Visual Snow FL-41 — Paramètres"
                        : c == AppCulture.German ? "Visual Snow FL-41 — Einstellungen"
                        : "Visual Snow FL-41 — Settings",
        (_, "FilterActive") => c == AppCulture.Turkish ? "Filtre AKTİF"
                        : c == AppCulture.French ? "Filtre ACTIF"
                        : c == AppCulture.German ? "Filter AKTIV"
                        : "Filter ACTIVE",
        (_, "RenderMode") => c == AppCulture.Turkish ? "Render Modu"
                        : c == AppCulture.French ? "Mode de rendu"
                        : c == AppCulture.German ? "Render-Modus"
                        : "Render Mode",
        (_, "OverlayDesc") => c == AppCulture.Turkish ? "Overlay — hassas FL-41 renk + opaklık (iş/masaüstü)"
                        : c == AppCulture.French ? "Overlay — couleur FL-41 précise + opacité (bureau)"
                        : c == AppCulture.German ? "Overlay — präzise FL-41 Farbe + Deckkraft (Desktop)"
                        : "Overlay — precise FL-41 color + opacity (work/desktop)",
        (_, "GammaDesc") => c == AppCulture.Turkish ? "Gamma — donanımsal LUT, fullscreen oyun dostu"
                        : c == AppCulture.French ? "Gamma — LUT matériel, compatible fullscreen"
                        : c == AppCulture.German ? "Gamma — Hardware-LUT, fullscreen-kompatibel"
                        : "Gamma — hardware LUT, fullscreen game-friendly",
        (_, "ColorPreset") => c == AppCulture.Turkish ? "FL-41 Renk Preset"
                        : c == AppCulture.French ? "Préréglage couleur FL-41"
                        : c == AppCulture.German ? "FL-41 Farbpreset"
                        : "FL-41 Color Preset",
        (_, "Opacity") => c == AppCulture.Turkish ? "Opaklık"
                        : c == AppCulture.French ? "Opacité"
                        : c == AppCulture.German ? "Deckkraft"
                        : "Opacity",
        (_, "GammaIntensity") => c == AppCulture.Turkish ? "Gamma Yoğunluğu"
                        : c == AppCulture.French ? "Intensité gamma"
                        : c == AppCulture.German ? "Gamma-Intensität"
                        : "Gamma Intensity",
        (_, "NightLightNote") => c == AppCulture.Turkish ? "Not: Windows Night Light kapalı olmalı."
                        : c == AppCulture.French ? "Note: Windows Night Light doit être désactivé."
                        : c == AppCulture.German ? "Hinweis: Windows Night Light muss deaktiviert sein."
                        : "Note: Windows Night Light must be off.",
        (_, "AutoStart") => c == AppCulture.Turkish ? "Windows açılışında otomatik başlat"
                        : c == AppCulture.French ? "Démarrer automatiquement avec Windows"
                        : c == AppCulture.German ? "Beim Windows-Start automatisch starten"
                        : "Start automatically with Windows",
        (_, "Hotkeys") => c == AppCulture.Turkish ? "Kısayollar"
                        : c == AppCulture.French ? "Raccourcis"
                        : c == AppCulture.German ? "Tastenkürzel"
                        : "Hotkeys",
        (_, "Close") => c == AppCulture.Turkish ? "Kapat"
                        : c == AppCulture.French ? "Fermer"
                        : c == AppCulture.German ? "Schließen"
                        : "Close",
        (_, "OpenRelief") => c == AppCulture.Turkish ? "Rahatlama araçlarını aç"
                        : c == AppCulture.French ? "Ouvrir les outils de soulagement"
                        : c == AppCulture.German ? "Linderungstools öffnen"
                        : "Open relief tools",

        // Presets
        (_, "IndoorName") => c == AppCulture.Turkish ? "Indoor FL-41"
                        : c == AppCulture.French ? "FL-41 Intérieur"
                        : c == AppCulture.German ? "FL-41 Innen"
                        : "Indoor FL-41",
        (_, "IndoorDesc") => c == AppCulture.Turkish ? "Hafif gül kurusu — ofis ve ekran için"
                        : c == AppCulture.French ? "Rose léger — bureau et écran"
                        : c == AppCulture.German ? "Helles Rose — Büro und Bildschirm"
                        : "Light rose — office and screen",
        (_, "WarmName") => c == AppCulture.Turkish ? "Warm FL-41"
                        : c == AppCulture.French ? "FL-41 Chaud"
                        : c == AppCulture.German ? "FL-41 Warm"
                        : "Warm FL-41",
        (_, "WarmDesc") => c == AppCulture.Turkish ? "Sıcak rose-amber — gerçek FL-41 tonu"
                        : c == AppCulture.French ? "Rose-ambre chaud — vrai ton FL-41"
                        : c == AppCulture.German ? "Warmes Rose-Bernstein — echter FL-41 Ton"
                        : "Warm rose-amber — true FL-41 tone",
        (_, "DeepName") => c == AppCulture.Turkish ? "Deep FL-41"
                        : c == AppCulture.French ? "FL-41 Profond"
                        : c == AppCulture.German ? "FL-41 Tief"
                        : "Deep FL-41",
        (_, "DeepDesc") => c == AppCulture.Turkish ? "Yoğun — koyu ortam / gece"
                        : c == AppCulture.French ? "Intense — environnement sombre / nuit"
                        : c == AppCulture.German ? "Intensiv — dunkle Umgebung / Nacht"
                        : "Intense — dark environment / night",

        // Hotkey labels
        (_, "HkToggle") => c == AppCulture.Turkish ? "Ctrl+Alt+F  —  Filtre aç/kapa"
                        : c == AppCulture.French ? "Ctrl+Alt+F  —  Activer/désactiver le filtre"
                        : c == AppCulture.German ? "Ctrl+Alt+F  —  Filter an/aus"
                        : "Ctrl+Alt+F  —  Toggle filter",
        (_, "HkSettings") => c == AppCulture.Turkish ? "Ctrl+Alt+O  —  Ayarlar paneli"
                        : c == AppCulture.French ? "Ctrl+Alt+O  —  Panneau de paramètres"
                        : c == AppCulture.German ? "Ctrl+Alt+O  —  Einstellungen"
                        : "Ctrl+Alt+O  —  Settings panel",
        (_, "HkMode") => c == AppCulture.Turkish ? "Ctrl+Alt+M  —  Mod değiştir"
                        : c == AppCulture.French ? "Ctrl+Alt+M  —  Changer de mode"
                        : c == AppCulture.German ? "Ctrl+Alt+M  —  Modus wechseln"
                        : "Ctrl+Alt+M  —  Switch mode",
        (_, "HkRelief") => c == AppCulture.Turkish ? "Ctrl+Alt+R  —  Rahatlama araçları"
                        : c == AppCulture.French ? "Ctrl+Alt+R  —  Outils de soulagement"
                        : c == AppCulture.German ? "Ctrl+Alt+R  —  Linderungstools"
                        : "Ctrl+Alt+R  —  Relief tools",

        // Eye break reminder
        (_, "EyeBreakTitle") => c == AppCulture.Turkish ? "20-20-20 Göz Molası"
                        : c == AppCulture.French ? "Pause oculaire 20-20-20"
                        : c == AppCulture.German ? "20-20-20 Augenpause"
                        : "20-20-20 Eye Break",
        (_, "EyeBreakMsg") => c == AppCulture.Turkish ? "20 dakika geçti!\n\n20 saniye boyunca ekrandan uzaklaşıp\n20 feet (6m) uzağa bak — gözlerini dinlendir."
                        : c == AppCulture.French ? "20 minutes passées!\n\nPendant 20 secondes, regardez à 20 pieds (6m) — reposez vos yeux."
                        : c == AppCulture.German ? "20 Minuten vorbei!\n\nFür 20 Sekunden auf 20 Fuß (6m) schauen — Augen ausruhen."
                        : "20 minutes passed!\n\nFor 20 seconds, look 20 ft (6m) away — rest your eyes.",

        _ => key
      };
}