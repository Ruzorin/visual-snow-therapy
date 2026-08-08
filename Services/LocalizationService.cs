using System.Globalization;

namespace VisualSnowScreen.Services;

/// <summary>
/// Çoklu dil desteği (i18n). Sistem diline göre otomatik seçim: EN, FR, DE, TR, ES.
/// Varsayılan: EN (desteklenmeyen diller için fallback).
/// </summary>
public static class LocalizationService
{
  public enum AppCulture { English, French, German, Turkish, Spanish }

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
      "ES" => AppCulture.Spanish,
      _ => AppCulture.English
    };
    return Current;
  }

  /// <summary>Manuel dil seçimi (ayarlardan).</summary>
  public static void Set(AppCulture culture) => Current = culture;

  // ===== String tablosu =====
  public static string S(string key) => Get(key, Current);

  private static string Get(string key, AppCulture c)
  {
    // TR, FR, DE, ES, EN sırasıyla
    return (key, c) switch
    {
      // App / tray
      ("AppName", AppCulture.Turkish) => "Visual Snow FL-41 Filtresi",
      ("AppName", AppCulture.French) => "Filtre FL-41 Visual Snow",
      ("AppName", AppCulture.German) => "Visual Snow FL-41 Filter",
      ("AppName", AppCulture.Spanish) => "Filtro FL-41 Visual Snow",
      ("AppName", _) => "Visual Snow FL-41 Filter",

      ("FilterOn", AppCulture.Turkish) => "Filtre: AÇIK",
      ("FilterOn", AppCulture.French) => "Filtre: ACTIF",
      ("FilterOn", AppCulture.German) => "Filter: AN",
      ("FilterOn", AppCulture.Spanish) => "Filtro: ACTIVO",
      ("FilterOn", _) => "Filter: ON",

      ("FilterOff", AppCulture.Turkish) => "Filtre: KAPALI",
      ("FilterOff", AppCulture.French) => "Filtre: INACTIF",
      ("FilterOff", AppCulture.German) => "Filter: AUS",
      ("FilterOff", AppCulture.Spanish) => "Filtro: INACTIVO",
      ("FilterOff", _) => "Filter: OFF",

      ("Mode", AppCulture.Turkish) => "Mod",
      ("Mode", AppCulture.French) => "Mode",
      ("Mode", AppCulture.German) => "Modus",
      ("Mode", AppCulture.Spanish) => "Modo",
      ("Mode", _) => "Mode",

      ("Overlay", AppCulture.Turkish) => "Overlay (FL-41 pencere)",
      ("Overlay", AppCulture.French) => "Overlay (fenêtre FL-41)",
      ("Overlay", AppCulture.German) => "Overlay (FL-41 Fenster)",
      ("Overlay", AppCulture.Spanish) => "Overlay (ventana FL-41)",
      ("Overlay", _) => "Overlay (FL-41 window)",

      ("Gamma", AppCulture.Turkish) => "Gamma (oyun dostu)",
      ("Gamma", AppCulture.French) => "Gamma (mode jeu)",
      ("Gamma", AppCulture.German) => "Gamma (spielmodus)",
      ("Gamma", AppCulture.Spanish) => "Gamma (modo juego)",
      ("Gamma", _) => "Gamma (game-friendly)",

      ("Settings", AppCulture.Turkish) => "Ayarlar...",
      ("Settings", AppCulture.French) => "Paramètres...",
      ("Settings", AppCulture.German) => "Einstellungen...",
      ("Settings", AppCulture.Spanish) => "Ajustes...",
      ("Settings", _) => "Settings...",

      ("Relief", AppCulture.Turkish) => "Rahatlama (4-7-8 / 20-20-20)...",
      ("Relief", AppCulture.French) => "Soulagement (4-7-8 / 20-20-20)...",
      ("Relief", AppCulture.German) => "Linderung (4-7-8 / 20-20-20)...",
      ("Relief", AppCulture.Spanish) => "Alivio (4-7-8 / 20-20-20)...",
      ("Relief", _) => "Relief (4-7-8 / 20-20-20)...",

      ("Exit", AppCulture.Turkish) => "Çıkış",
      ("Exit", AppCulture.French) => "Quitter",
      ("Exit", AppCulture.German) => "Beenden",
      ("Exit", AppCulture.Spanish) => "Salir",
      ("Exit", _) => "Exit",

      // Settings window
      ("SettingsTitle", AppCulture.Turkish) => "Visual Snow FL-41 — Ayarlar",
      ("SettingsTitle", AppCulture.French) => "Visual Snow FL-41 — Paramètres",
      ("SettingsTitle", AppCulture.German) => "Visual Snow FL-41 — Einstellungen",
      ("SettingsTitle", AppCulture.Spanish) => "Visual Snow FL-41 — Ajustes",
      ("SettingsTitle", _) => "Visual Snow FL-41 — Settings",

      ("Subtitle", AppCulture.Turkish) => "480-520nm mavi-yeşil blokajı · rose-amber",
      ("Subtitle", AppCulture.French) => "Bloc bleu-vert 480-520nm · rose-ambre",
      ("Subtitle", AppCulture.German) => "Blau-Grün-Block 480-520nm · Rose-Bernstein",
      ("Subtitle", AppCulture.Spanish) => "Bloque azul-verde 480-520nm · rosa-ámbar",
      ("Subtitle", _) => "480-520nm blue-green block · rose-amber",

      ("FilterActive", AppCulture.Turkish) => "Filtre AKTİF",
      ("FilterActive", AppCulture.French) => "Filtre ACTIF",
      ("FilterActive", AppCulture.German) => "Filter AKTIV",
      ("FilterActive", AppCulture.Spanish) => "Filtro ACTIVO",
      ("FilterActive", _) => "Filter ACTIVE",

      ("RenderMode", AppCulture.Turkish) => "Render Modu",
      ("RenderMode", AppCulture.French) => "Mode de rendu",
      ("RenderMode", AppCulture.German) => "Render-Modus",
      ("RenderMode", AppCulture.Spanish) => "Modo de renderizado",
      ("RenderMode", _) => "Render Mode",

      ("OverlayDesc", AppCulture.Turkish) => "Overlay — hassas FL-41 renk + opaklık (iş/masaüstü)",
      ("OverlayDesc", AppCulture.French) => "Overlay — couleur FL-41 précise + opacité (bureau)",
      ("OverlayDesc", AppCulture.German) => "Overlay — präzise FL-41 Farbe + Deckkraft (Desktop)",
      ("OverlayDesc", AppCulture.Spanish) => "Overlay — color FL-41 preciso + opacidad (escritorio)",
      ("OverlayDesc", _) => "Overlay — precise FL-41 color + opacity (work/desktop)",

      ("GammaDesc", AppCulture.Turkish) => "Gamma — donanımsal LUT, fullscreen oyun dostu",
      ("GammaDesc", AppCulture.French) => "Gamma — LUT matériel, compatible fullscreen",
      ("GammaDesc", AppCulture.German) => "Gamma — Hardware-LUT, fullscreen-kompatibel",
      ("GammaDesc", AppCulture.Spanish) => "Gamma — LUT hardware, compatible con pantalla completa",
      ("GammaDesc", _) => "Gamma — hardware LUT, fullscreen game-friendly",

      ("ColorPreset", AppCulture.Turkish) => "FL-41 Renk Preset",
      ("ColorPreset", AppCulture.French) => "Préréglage couleur FL-41",
      ("ColorPreset", AppCulture.German) => "FL-41 Farbpreset",
      ("ColorPreset", AppCulture.Spanish) => "Preset de color FL-41",
      ("ColorPreset", _) => "FL-41 Color Preset",

      ("Opacity", AppCulture.Turkish) => "Opaklık",
      ("Opacity", AppCulture.French) => "Opacité",
      ("Opacity", AppCulture.German) => "Deckkraft",
      ("Opacity", AppCulture.Spanish) => "Opacidad",
      ("Opacity", _) => "Opacity",

      ("GammaIntensity", AppCulture.Turkish) => "Gamma Yoğunluğu",
      ("GammaIntensity", AppCulture.French) => "Intensité gamma",
      ("GammaIntensity", AppCulture.German) => "Gamma-Intensität",
      ("GammaIntensity", AppCulture.Spanish) => "Intensidad gamma",
      ("GammaIntensity", _) => "Gamma Intensity",

      ("NightLightNote", AppCulture.Turkish) => "Not: Windows Night Light kapalı olmalı.",
      ("NightLightNote", AppCulture.French) => "Note: Windows Night Light doit être désactivé.",
      ("NightLightNote", AppCulture.German) => "Hinweis: Windows Night Light muss deaktiviert sein.",
      ("NightLightNote", AppCulture.Spanish) => "Nota: Windows Night Light debe estar desactivado.",
      ("NightLightNote", _) => "Note: Windows Night Light must be off.",

      ("AutoStart", AppCulture.Turkish) => "Windows açılışında otomatik başlat",
      ("AutoStart", AppCulture.French) => "Démarrer automatiquement avec Windows",
      ("AutoStart", AppCulture.German) => "Beim Windows-Start automatisch starten",
      ("AutoStart", AppCulture.Spanish) => "Iniciar automáticamente con Windows",
      ("AutoStart", _) => "Start automatically with Windows",

      ("ForcedEyeBreak", AppCulture.Turkish) => "20-20-20 zorunlu göz molası (engelleme uyarısı)",
      ("ForcedEyeBreak", AppCulture.French) => "Pause oculaire 20-20-20 obligatoire (rappel bloquant)",
      ("ForcedEyeBreak", AppCulture.German) => "20-20-20 Augenpause erzwingen (blockierende Erinnerung)",
      ("ForcedEyeBreak", AppCulture.Spanish) => "Forzar pausa ocular 20-20-20 (recordatorio bloqueante)",
      ("ForcedEyeBreak", _) => "Force 20-20-20 eye break (blocking reminder)",

      ("Hotkeys", AppCulture.Turkish) => "Kısayollar",
      ("Hotkeys", AppCulture.French) => "Raccourcis",
      ("Hotkeys", AppCulture.German) => "Tastenkürzel",
      ("Hotkeys", AppCulture.Spanish) => "Atajos",
      ("Hotkeys", _) => "Hotkeys",

      ("Close", AppCulture.Turkish) => "Kapat",
      ("Close", AppCulture.French) => "Fermer",
      ("Close", AppCulture.German) => "Schließen",
      ("Close", AppCulture.Spanish) => "Cerrar",
      ("Close", _) => "Close",

      ("OpenRelief", AppCulture.Turkish) => "RAHATLAMA ARAÇLARINI AÇ",
      ("OpenRelief", AppCulture.French) => "OUVRIR LES OUTILS DE SOULAGEMENT",
      ("OpenRelief", AppCulture.German) => "LINDERUNGSTOOLS ÖFFNEN",
      ("OpenRelief", AppCulture.Spanish) => "ABRIR HERRAMIENTAS DE ALIVIO",
      ("OpenRelief", _) => "OPEN RELIEF TOOLS",

      ("Language", AppCulture.Turkish) => "Dil:",
      ("Language", AppCulture.French) => "Langue :",
      ("Language", AppCulture.German) => "Sprache:",
      ("Language", AppCulture.Spanish) => "Idioma:",
      ("Language", _) => "Language:",

      // Presets
      ("IndoorName", AppCulture.Turkish) => "Indoor FL-41",
      ("IndoorName", AppCulture.French) => "FL-41 Intérieur",
      ("IndoorName", AppCulture.German) => "FL-41 Innen",
      ("IndoorName", AppCulture.Spanish) => "FL-41 Interior",
      ("IndoorName", _) => "Indoor FL-41",

      ("IndoorDesc", AppCulture.Turkish) => "Hafif gül kurusu — ofis ve ekran için",
      ("IndoorDesc", AppCulture.French) => "Rose léger — bureau et écran",
      ("IndoorDesc", AppCulture.German) => "Helles Rose — Büro und Bildschirm",
      ("IndoorDesc", AppCulture.Spanish) => "Rosa claro — oficina y pantalla",
      ("IndoorDesc", _) => "Light rose — office and screen",

      ("WarmName", AppCulture.Turkish) => "Warm FL-41",
      ("WarmName", AppCulture.French) => "FL-41 Chaud",
      ("WarmName", AppCulture.German) => "FL-41 Warm",
      ("WarmName", AppCulture.Spanish) => "FL-41 Cálido",
      ("WarmName", _) => "Warm FL-41",

      ("WarmDesc", AppCulture.Turkish) => "Sıcak rose-amber — gerçek FL-41 tonu",
      ("WarmDesc", AppCulture.French) => "Rose-ambre chaud — vrai ton FL-41",
      ("WarmDesc", AppCulture.German) => "Warmes Rose-Bernstein — echter FL-41 Ton",
      ("WarmDesc", AppCulture.Spanish) => "Rosa-ámbar cálido — tono FL-41 real",
      ("WarmDesc", _) => "Warm rose-amber — true FL-41 tone",

      ("DeepName", AppCulture.Turkish) => "Deep FL-41",
      ("DeepName", AppCulture.French) => "FL-41 Profond",
      ("DeepName", AppCulture.German) => "FL-41 Tief",
      ("DeepName", AppCulture.Spanish) => "FL-41 Profundo",
      ("DeepName", _) => "Deep FL-41",

      ("DeepDesc", AppCulture.Turkish) => "Yoğun — koyu ortam / gece",
      ("DeepDesc", AppCulture.French) => "Intense — environnement sombre / nuit",
      ("DeepDesc", AppCulture.German) => "Intensiv — dunkle Umgebung / Nacht",
      ("DeepDesc", AppCulture.Spanish) => "Intenso — entorno oscuro / noche",
      ("DeepDesc", _) => "Intense — dark environment / night",

      // Hotkey labels
      ("HkToggle", AppCulture.Turkish) => "Ctrl+Alt+F  —  Filtre aç/kapa",
      ("HkToggle", AppCulture.French) => "Ctrl+Alt+F  —  Activer/désactiver le filtre",
      ("HkToggle", AppCulture.German) => "Ctrl+Alt+F  —  Filter an/aus",
      ("HkToggle", AppCulture.Spanish) => "Ctrl+Alt+F  —  Activar/desactivar filtro",
      ("HkToggle", _) => "Ctrl+Alt+F  —  Toggle filter",

      ("HkSettings", AppCulture.Turkish) => "Ctrl+Alt+O  —  Ayarlar paneli",
      ("HkSettings", AppCulture.French) => "Ctrl+Alt+O  —  Panneau de paramètres",
      ("HkSettings", AppCulture.German) => "Ctrl+Alt+O  —  Einstellungen",
      ("HkSettings", AppCulture.Spanish) => "Ctrl+Alt+O  —  Panel de ajustes",
      ("HkSettings", _) => "Ctrl+Alt+O  —  Settings panel",

      ("HkMode", AppCulture.Turkish) => "Ctrl+Alt+M  —  Mod değiştir",
      ("HkMode", AppCulture.French) => "Ctrl+Alt+M  —  Changer de mode",
      ("HkMode", AppCulture.German) => "Ctrl+Alt+M  —  Modus wechseln",
      ("HkMode", AppCulture.Spanish) => "Ctrl+Alt+M  —  Cambiar modo",
      ("HkMode", _) => "Ctrl+Alt+M  —  Switch mode",

      ("HkRelief", AppCulture.Turkish) => "Ctrl+Alt+R  —  Rahatlama araçları",
      ("HkRelief", AppCulture.French) => "Ctrl+Alt+R  —  Outils de soulagement",
      ("HkRelief", AppCulture.German) => "Ctrl+Alt+R  —  Linderungstools",
      ("HkRelief", AppCulture.Spanish) => "Ctrl+Alt+R  —  Herramientas de alivio",
      ("HkRelief", _) => "Ctrl+Alt+R  —  Relief tools",

      // Eye break reminder
      ("EyeBreakTitle", AppCulture.Turkish) => "20-20-20 Göz Molası",
      ("EyeBreakTitle", AppCulture.French) => "Pause oculaire 20-20-20",
      ("EyeBreakTitle", AppCulture.German) => "20-20-20 Augenpause",
      ("EyeBreakTitle", AppCulture.Spanish) => "Pausa ocular 20-20-20",
      ("EyeBreakTitle", _) => "20-20-20 Eye Break",

      ("EyeBreakMsg", AppCulture.Turkish) => "20 dakika geçti!\n\n20 saniye boyunca ekrandan uzaklaşıp\n20 feet (6m) uzağa bak — gözlerini dinlendir.",
      ("EyeBreakMsg", AppCulture.French) => "20 minutes passées!\n\nPendant 20 secondes, regardez à 20 pieds (6m) — reposez vos yeux.",
      ("EyeBreakMsg", AppCulture.German) => "20 Minuten vorbei!\n\nFür 20 Sekunden auf 20 Fuß (6m) schauen — Augen ausruhen.",
      ("EyeBreakMsg", AppCulture.Spanish) => "¡20 minutos pasados!\n\nDurante 20 segundos, mire a 20 pies (6m) — descanse sus ojos.",
      ("EyeBreakMsg", _) => "20 minutes passed!\n\nFor 20 seconds, look 20 ft (6m) away — rest your eyes.",

      _ => key
    };
  }
}