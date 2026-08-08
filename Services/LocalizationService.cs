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

      ("IndoorName", AppCulture.Turkish) => "Indoor FL-41",
      ("IndoorName", AppCulture.French) => "FL-41 Intérieur",
      ("IndoorName", AppCulture.German) => "FL-41 Innenraum",
      ("IndoorName", AppCulture.Spanish) => "FL-41 Interiores",
      ("IndoorName", _) => "Indoor FL-41",

      ("IndoorDesc", AppCulture.Turkish) => "Hafif gül kurusu — ofis ve ekran için",
      ("IndoorDesc", AppCulture.French) => "Rose léger — pour bureau et écran",
      ("IndoorDesc", AppCulture.German) => "Leichtes Rosé — für Büro und Bildschirm",
      ("IndoorDesc", AppCulture.Spanish) => "Rosa claro — para oficina y pantalla",
      ("IndoorDesc", _) => "Light rose tint — for office and screen use",

      ("WarmName", AppCulture.Turkish) => "Warm FL-41",
      ("WarmName", AppCulture.French) => "FL-41 Chaud",
      ("WarmName", AppCulture.German) => "FL-41 Warm",
      ("WarmName", AppCulture.Spanish) => "FL-41 Cálido",
      ("WarmName", _) => "Warm FL-41",

      ("WarmDesc", AppCulture.Turkish) => "Sıcak rose-amber — gerçek FL-41 tonu",
      ("WarmDesc", AppCulture.French) => "Rose-ambre chaud — véritable teinte FL-41",
      ("WarmDesc", AppCulture.German) => "Warmes Rosé-Bernstein — echte FL-41 Nuance",
      ("WarmDesc", AppCulture.Spanish) => "Rosa-ámbar cálido — tono FL-41 auténtico",
      ("WarmDesc", _) => "Warm rose-amber — true FL-41 spectrum",

      ("DeepName", AppCulture.Turkish) => "Deep FL-41",
      ("DeepName", AppCulture.French) => "FL-41 Intense",
      ("DeepName", AppCulture.German) => "FL-41 Intensiv",
      ("DeepName", AppCulture.Spanish) => "FL-41 Intenso",
      ("DeepName", _) => "Deep FL-41",

      ("DeepDesc", AppCulture.Turkish) => "Yoğun — koyu ortam / gece",
      ("DeepDesc", AppCulture.French) => "Intense — environnement sombre / nuit",
      ("DeepDesc", AppCulture.German) => "Intensiv — dunkle Umgebung / Nacht",
      ("DeepDesc", AppCulture.Spanish) => "Intenso — ambiente oscuro / noche",
      ("DeepDesc", _) => "High density — dark room / night use",

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

      ("SmartNoiseLabel", AppCulture.Turkish) => "Akıllı Ekran Filtresini Aç (VSS Alışkanlık Katmanı)",
      ("SmartNoiseLabel", AppCulture.French) => "Activer le filtre d'écran intelligent (habituation VSS)",
      ("SmartNoiseLabel", AppCulture.German) => "Smart Screen Filter aktivieren (VSS-Gewöhnung)",
      ("SmartNoiseLabel", AppCulture.Spanish) => "Activar filtro de pantalla inteligente (habituación VSS)",
      ("SmartNoiseLabel", _) => "Enable Smart Screen Filter (VSS Habituation)",

      ("SmartNoiseWarning", AppCulture.Turkish) => "* Yorgunluğu önlemek için 15 dakika sonra güvenle kapatılır.",
      ("SmartNoiseWarning", AppCulture.French) => "* Désactivé après 15 min pour éviter la fatigue.",
      ("SmartNoiseWarning", AppCulture.German) => "* Wird nach 15 Min sicher deaktiviert, um Ermüdung zu vermeiden.",
      ("SmartNoiseWarning", AppCulture.Spanish) => "* Se desactiva tras 15 min para prevenir fatiga.",
      ("SmartNoiseWarning", _) => "* Disabled safely after 15 mins to prevent fatigue.",

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

      // ===== Relief Window =====
      ("reliefTitle", AppCulture.Turkish) => "Visual Snow — Rahatlama ve Farkındalık",
      ("reliefTitle", AppCulture.French) => "Visual Snow — Soulagement et Pleine Conscience",
      ("reliefTitle", AppCulture.German) => "Visual Snow — Linderung und Achtsamkeit",
      ("reliefTitle", AppCulture.Spanish) => "Visual Snow — Alivio y Conciencia",
      ("reliefTitle", _) => "Visual Snow — Relief & Mindfulness",

      ("reliefHeaderTitle", AppCulture.Turkish) => "Rahatlama ve Farkındalık",
      ("reliefHeaderTitle", AppCulture.French) => "Soulagement et Pleine Conscience",
      ("reliefHeaderTitle", AppCulture.German) => "Linderung und Achtsamkeit",
      ("reliefHeaderTitle", AppCulture.Spanish) => "Alivio y Conciencia",
      ("reliefHeaderTitle", _) => "Relief & Mindfulness",

      ("reliefHeaderSubtitle", AppCulture.Turkish) => "Visual Snow Syndrome için kanıt destekli rahatlama teknikleri",
      ("reliefHeaderSubtitle", AppCulture.French) => "Techniques de soulagement basées sur des preuves pour le syndrome Visual Snow",
      ("reliefHeaderSubtitle", AppCulture.German) => "Evidenzbasierte Linderungstechniken für Visual Snow Syndrom",
      ("reliefHeaderSubtitle", AppCulture.Spanish) => "Técnicas de alivio basadas en evidencia para el Síndrome de Visual Snow",
      ("reliefHeaderSubtitle", _) => "Evidence-based relief techniques for Visual Snow Syndrome",

      ("reliefTabBreathing", AppCulture.Turkish) => "4-7-8 Nefes",
      ("reliefTabBreathing", _) => "4-7-8 Breathing",

      ("reliefTabEyeBreak", AppCulture.Turkish) => "20-20-20 Göz Molası",
      ("reliefTabEyeBreak", AppCulture.French) => "Pause oculaire 20-20-20",
      ("reliefTabEyeBreak", AppCulture.German) => "20-20-20 Augenpause",
      ("reliefTabEyeBreak", AppCulture.Spanish) => "Pausa ocular 20-20-20",
      ("reliefTabEyeBreak", _) => "20-20-20 Eye Break",

      ("reliefTabNort", AppCulture.Turkish) => "NORT Egzersizi",
      ("reliefTabNort", _) => "NORT Exercise",

      ("reliefTabHabituation", AppCulture.Turkish) => "Alışma (Deneysel)",
      ("reliefTabHabituation", AppCulture.French) => "Habituation (Expérimental)",
      ("reliefTabHabituation", AppCulture.German) => "Gewöhnung (Experimentell)",
      ("reliefTabHabituation", AppCulture.Spanish) => "Habituación (Experimental)",
      ("reliefTabHabituation", _) => "Habituation (Experimental)",

      ("reliefTabInfo", AppCulture.Turkish) => "Bilgi",
      ("reliefTabInfo", AppCulture.French) => "Infos",
      ("reliefTabInfo", AppCulture.German) => "Info",
      ("reliefTabInfo", AppCulture.Spanish) => "Info",
      ("reliefTabInfo", _) => "Info",

      ("reliefBreathTitle", AppCulture.Turkish) => "4-7-8 Nefes Tekniği",
      ("reliefBreathTitle", _) => "4-7-8 Breathing Technique",

      ("reliefBreathDesc", AppCulture.Turkish) => "Parasympatik sinir sistemini aktive eder, stresi ve fotofobiyi azaltır.",
      ("reliefBreathDesc", _) => "Activates the parasympathetic nervous system, reduces stress and photophobia.",

      ("reliefBreathStart", AppCulture.Turkish) => "Başlat (4 döngü)",
      ("reliefBreathStart", _) => "Start (4 cycles)",

      ("reliefBreathHint", AppCulture.Turkish) => "4 sn nefes al → 7 sn tut → 8 sn ver",
      ("reliefBreathHint", _) => "4s inhale → 7s hold → 8s exhale",

      ("reliefBreathReady", AppCulture.Turkish) => "Hazır",
      ("reliefBreathReady", _) => "Ready",

      ("reliefBreakTitle", AppCulture.Turkish) => "20-20-20 Kuralı",
      ("reliefBreakTitle", _) => "20-20-20 Rule",

      ("reliefBreakDesc", AppCulture.Turkish) => "Her 20 dakikada 20 sn boyunca 20 ft (6m) uzağa bak.",
      ("reliefBreakDesc", _) => "Every 20 minutes, look 20 ft (6m) away for 20 seconds.",

      ("reliefBreakStart", AppCulture.Turkish) => "Şimdi Göz Molası Ver",
      ("reliefBreakStart", _) => "Take an Eye Break Now",

      ("reliefBreakHint", AppCulture.Turkish) => "Uzağa bakarken gözlerini kırpma — sakin, yumuşak bakış.",
      ("reliefBreakHint", _) => "Don't blink while looking away — calm, soft gaze.",

      ("reliefBreakSeconds", AppCulture.Turkish) => "saniye",
      ("reliefBreakSeconds", AppCulture.French) => "secondes",
      ("reliefBreakSeconds", AppCulture.German) => "Sekunden",
      ("reliefBreakSeconds", AppCulture.Spanish) => "segundos",
      ("reliefBreakSeconds", _) => "seconds",

      ("reliefNortTitle", AppCulture.Turkish) => "NORT: Göz Takibi ve Sıçrama Egzersizleri",
      ("reliefNortTitle", _) => "NORT: Eye Tracking & Saccade Exercises",

      ("reliefNortStart", AppCulture.Turkish) => "NORT Egzersizini Başlat",
      ("reliefNortStart", _) => "Start NORT Exercise",

      ("reliefNortDesc1", AppCulture.Turkish) => "Neuro-Optometric Rehabilitation Therapy (NORT), Visual Snow Sendromundaki okülomotor yetersizlikleri hedefler. Göz takibi (Smooth Pursuit) ve sıçrama (Saccade) tekniklerini kullanarak beynin görsel verileri daha rahat işlemesini sağlar.\n\nShidlofsky vd. tarafından gerçekleştirilen bağımsız ve VSI destekli çalışmalar okulomotor disfonksiyon düzeltmelerinin semptomlarda iyileşme sağlayabildiğini belirtmektedir.",
      ("reliefNortDesc1", _) => "Neuro-Optometric Rehabilitation Therapy (NORT) targets oculomotor deficiencies in Visual Snow Syndrome. It uses Smooth Pursuit and Saccade techniques to help the brain process visual data more comfortably.\n\nIndependent and VSI-supported studies by Shidlofsky et al. indicate that oculomotor dysfunction corrections can lead to symptom improvement.",

      ("reliefNortDesc2", AppCulture.Turkish) => "Bu egzersiz toplam 3 dakika sürecek (1.5 dk göz takibi + 1.5 dk sıçrama). Süre bitiminde nörolojik yorgunluk ('rebound') olmaması için güvenli şekilde kapanır.",
      ("reliefNortDesc2", _) => "This exercise lasts 3 minutes total (1.5 min pursuit + 1.5 min saccades). It closes safely at the end to prevent neurological fatigue ('rebound').",

      ("reliefHabitTitle", AppCulture.Turkish) => "Alışma (Habituation) — Deneysel",
      ("reliefHabitTitle", _) => "Habituation — Experimental",

      ("reliefHabitDesc", AppCulture.Turkish) => "Bazı hastalar, kontrollü şekilde statik benzeri uyarana kısa süre maruz kalarak beynin görsel hiperaktivitesinin azaldığını bildiriyor (nöroplastisite prensibi).",
      ("reliefHabitDesc", _) => "Some patients report that brief, controlled exposure to static-like stimuli reduces visual hyperactivity in the brain (neuroplasticity principle).",

      ("reliefHabitDuration", AppCulture.Turkish) => "Süre seç:",
      ("reliefHabitDuration", AppCulture.French) => "Choisir la durée:",
      ("reliefHabitDuration", AppCulture.German) => "Dauer wählen:",
      ("reliefHabitDuration", AppCulture.Spanish) => "Elegir duración:",
      ("reliefHabitDuration", _) => "Select duration:",

      ("reliefHabitWarning", AppCulture.Turkish) => "Uyarı: Bu deneysel bir tekniktir. Rahatsızlık artarsa hemen durdurun.",
      ("reliefHabitWarning", _) => "Warning: This is an experimental technique. Stop immediately if discomfort increases.",

      ("reliefHabitStart", AppCulture.Turkish) => "Statik Ekranı Göster",
      ("reliefHabitStart", _) => "Show Static Screen",

      ("reliefInfoTitle", AppCulture.Turkish) => "Visual Snow Syndrome Hakkında",
      ("reliefInfoTitle", _) => "About Visual Snow Syndrome",

      ("reliefInfoFl41Title", AppCulture.Turkish) => "Fotofobi için FL-41",
      ("reliefInfoFl41Title", _) => "FL-41 for Photophobia",

      ("reliefInfoTipsTitle", AppCulture.Turkish) => "Günlük Yönetim İpuçları",
      ("reliefInfoTipsTitle", _) => "Daily Management Tips",

      ("reliefInfoDesc1", AppCulture.Turkish) => "VSS, tüm görme alanında sürekli 'kar' veya statik görme ile karakterize nörolojik bir durumdur. Gözler açık veya kapalı fark etmez — 7/24 vardır.",
      ("reliefInfoDesc1", _) => "VSS is a neurological condition characterized by continuous 'snow' or static vision across the entire visual field. Eyes open or closed — it's there 24/7.",

      ("reliefInfoDesc2", AppCulture.Turkish) => "Eşlik eden semptomlar: palinopsi (artık görüntüler), fotofobi (ışık hassasiyeti), entoptik fenomenler, noktalı gece körlüğü, tinnitus.",
      ("reliefInfoDesc2", _) => "Accompanying symptoms: palinopsia (after-images), photophobia (light sensitivity), entoptic phenomena, nyctalopia (night blindness), tinnitus.",

      ("reliefInfoFl41Desc", AppCulture.Turkish) => "FL-41 rose-amber camlar 480-520nm mavi-yeşil ışığı bloke eder. Klinik araştırmalar fotofobi yollarındaki (S1, S2, insula, ACC) beyin aktivasyonunu anlamlı ölçüde azalttığını gösteriyor. Bu uygulama aynı etkiyi ekran katmanı olarak taklit eder.",
      ("reliefInfoFl41Desc", _) => "FL-41 rose-amber lenses block 480-520nm blue-green light. Clinical studies show significant reduction in brain activation in photophobia pathways (S1, S2, insula, ACC). This app mimics the same effect as a screen overlay.",

      ("reliefInfoTips", AppCulture.Turkish) => "• Düzenli uyku (melatonin döngüsü)\n• Stres yönetimi: meditasyon, derin nefes\n• Kafein/alkol tüketimini azalt\n• Migren komorbiditesini tedavi et\n• FL-41 camlar + ekran filtresi (bu app)\n• 20-20-20 kuralı ile düzenli göz molası\n• Yüksek kontrast/strobe ışıktan kaçın",
      ("reliefInfoTips", _) => "• Regular sleep (melatonin cycle)\n• Stress management: meditation, deep breathing\n• Reduce caffeine/alcohol\n• Treat migraine comorbidity\n• FL-41 lenses + screen filter (this app)\n• Regular eye breaks with 20-20-20 rule\n• Avoid high contrast/strobe light",

      ("reliefBack", AppCulture.Turkish) => "← Geri",
      ("reliefBack", AppCulture.French) => "← Retour",
      ("reliefBack", AppCulture.German) => "← Zurück",
      ("reliefBack", AppCulture.Spanish) => "← Volver",
      ("reliefBack", _) => "← Back",

      ("reliefBreathInhale", AppCulture.Turkish) => "Nefes Al",
      ("reliefBreathInhale", _) => "Inhale",

      ("reliefBreathHold", AppCulture.Turkish) => "Tut",
      ("reliefBreathHold", AppCulture.French) => "Retenir",
      ("reliefBreathHold", AppCulture.German) => "Halten",
      ("reliefBreathHold", AppCulture.Spanish) => "Mantener",
      ("reliefBreathHold", _) => "Hold",

      ("reliefBreathExhale", AppCulture.Turkish) => "Ver",
      ("reliefBreathExhale", AppCulture.French) => "Expirer",
      ("reliefBreathExhale", AppCulture.German) => "Ausatmen",
      ("reliefBreathExhale", AppCulture.Spanish) => "Exhalar",
      ("reliefBreathExhale", _) => "Exhale",

      ("reliefBreathDone", AppCulture.Turkish) => "Tamamlandı",
      ("reliefBreathDone", AppCulture.French) => "Terminé",
      ("reliefBreathDone", AppCulture.German) => "Fertig",
      ("reliefBreathDone", AppCulture.Spanish) => "Completado",
      ("reliefBreathDone", _) => "Complete",

      ("reliefSourcesTitle", AppCulture.Turkish) => "Kaynaklar (resmi araştırma):",
      ("reliefSourcesTitle", _) => "Sources (official research):",

      ("reliefMedicalDisclaimer", AppCulture.Turkish) => "Tıbbi tavsiye yerine geçmez. Semptomlar için nöro-oftalmoloğa başvurun.",
      ("reliefMedicalDisclaimer", _) => "Does not replace medical advice. Consult a neuro-ophthalmologist for symptoms.",

      // ===== NORT Therapy Window =====
      ("nortTitle", AppCulture.Turkish) => "Nöro-Optometrik Rehabilitasyon Terapisi",
      ("nortTitle", _) => "Neuro-Optometric Rehabilitation Therapy",

      ("nortDesc", AppCulture.Turkish) => "Bu egzersiz, Visual Snow Sendromundaki okülomotor disfonksiyonu hedefleyen bir NORT protokolüdür.",
      ("nortDesc", _) => "This exercise is a NORT protocol targeting oculomotor dysfunction in Visual Snow Syndrome.",

      ("nortTimeLeft", AppCulture.Turkish) => "Kalan Süre:",
      ("nortTimeLeft", _) => "Time Left:",

      ("nortAutoClose", AppCulture.Turkish) => "Egzersiz bittiğinde otomatik kapanır.",
      ("nortAutoClose", _) => "The exercise closes automatically when finished.",

      ("nortEsc", AppCulture.Turkish) => "Çıkmak için ESC'ye basın.",
      ("nortEsc", _) => "Press ESC to exit.",

      ("nortPursuitTitle", AppCulture.Turkish) => "Smooth Pursuit (Göz Takibi)",
      ("nortPursuitTitle", _) => "Smooth Pursuit (Eye Tracking)",

      ("nortSaccadeTitle", AppCulture.Turkish) => "Saccade Training (Sıçrama)",
      ("nortSaccadeTitle", _) => "Saccade Training",

      ("nortSaccadeDesc", AppCulture.Turkish) => "Hedef belirdiği an hızlıca gözlerinizi hedefe kilitleyin.",
      ("nortSaccadeDesc", _) => "Lock your eyes onto the target as soon as it appears.",

      ("nortDoneTitle", AppCulture.Turkish) => "Egzersiz Tamamlandı",
      ("nortDoneTitle", _) => "Exercise Complete",

      ("nortDoneDesc", AppCulture.Turkish) => "Göz ve beyin koordinasyonunuzu başarıyla çalıştırdınız. Nörolojik yorgunluk ve 'rebound' etkisini önlemek için seans güvenli bir şekilde sonlandırıldı.",
      ("nortDoneDesc", _) => "You have successfully trained your eye-brain coordination. The session was safely ended to prevent neurological fatigue and rebound effects.",

      // ===== Smart Noise Medical Warning =====
      ("SmartNoiseMedicalWarning", AppCulture.Turkish) => "⚠ DİKKAT: Bu, projedeki bilimsel araştırmalarla faydası kanıtlanamamış ve olası yan etkileri bilinmeyen TEK özelliktir. Bu bilinçle kullanın.",
      ("SmartNoiseMedicalWarning", AppCulture.French) => "⚠ ATTENTION: Il s'agit de la SEULE fonctionnalité du projet dont les bénéfices ne sont pas prouvés par la recherche scientifique et dont les effets secondaires sont inconnus. À utiliser avec prudence.",
      ("SmartNoiseMedicalWarning", AppCulture.German) => "⚠ ACHTUNG: Dies ist die EINZIGE Funktion im Projekt, deren Nutzen nicht durch wissenschaftliche Forschung belegt ist und deren Nebenwirkungen unbekannt sind. Mit Vorsicht verwenden.",
      ("SmartNoiseMedicalWarning", AppCulture.Spanish) => "⚠ ATENCIÓN: Esta es la ÚNICA función del proyecto cuyos beneficios no están probados por la investigación científica y cuyos efectos secundarios son desconocidos. Úsela con precaución.",
      ("SmartNoiseMedicalWarning", _) => "⚠ WARNING: This is the ONLY feature in the project whose benefits are unproven by scientific research and whose side effects are unknown. Use with caution.",

      _ => key
    };
  }
}