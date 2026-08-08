# Visual Snow Therapy 🧊👁️

A lightweight Windows desktop screen-layer application that transforms your screen into the full **FL-41 (rose-amber)** color spectrum for **Visual Snow Syndrome (VSS)** and **photophobia** relief. Built with C# and WPF (.NET 8).

It also includes evidence-based relaxation tools: **4-7-8 breathing**, **20-20-20 eye breaks**, an experimental **habituation** screen, and VSS awareness info with official research source links.

---

## 📖 Table of Contents

- [What is FL-41?](#what-is-fl-41)
- [Features](#features)
- [Two Render Modes (Hybrid)](#two-render-modes-hybrid)
- [Hotkeys](#hotkeys)
- [FL-41 Color Presets](#fl-41-color-presets)
- [Relaxation & Awareness Tools](#relaxation--awareness-tools)
- [VSS Safety](#vss-safety)
- [GPU / Battery Optimization](#gpu--battery-optimization)
- [Run / Build](#run--build)
- [Architecture](#architecture)
- [Research Sources](#research-sources)
- [Disclaimer](#disclaimer)
- [Türkçe Açıklama](#türkçe-açıklama)

---

## What is FL-41?

**FL-41** ("Fluorescent 41") is a specialized rose-amber optical tint developed in the 1980s. Unlike sunglasses that reduce all light equally, FL-41 **selectively blocks the 480–520 nm blue-green spectrum** — the wavelengths most strongly linked to photophobia, migraine, and visual discomfort.

Clinical research (PMC10939838) shows FL-41 lenses **significantly reduce BOLD activation** in photophobia neural pathways (S1, S2, insula, ACC). This app replicates that effect as a screen overlay.

## Features

- ✅ **Topmost** fullscreen overlay (`Topmost = true`)
- ✅ **Click-through** — clicks pass to windows below (`WS_EX_TRANSPARENT` + `WS_EX_LAYERED`)
- ✅ **Opacity slider** for precise FL-41 intensity
- ✅ **Hybrid render**: Overlay (precise color) **and** Gamma Ramp (game-friendly)
- ✅ **Multi-monitor** support with Per-Monitor V2 DPI awareness
- ✅ **System tray** icon + context menu
- ✅ **Global hotkeys**
- ✅ **Windows autostart** option
- ✅ **Relaxation tools**: 4-7-8 breathing, 20-20-20 eye break, habituation, awareness
- ✅ **Settings persistence** (`%AppData%\VisualSnowScreen\settings.json`)

## Two Render Modes (Hybrid)

| Mode | Method | When to use |
|---|---|---|
| **Overlay** | WPF click-through translucent window (`WS_EX_TRANSPARENT` + `WS_EX_LAYERED`) | Work, desktop, browser — precise FL-41 color + opacity control |
| **Gamma** | Hardware gamma LUT (`SetDeviceGammaRamp`) | Gaming (RTX 4050), battery (Ryzen iGPU) — fullscreen-compatible, zero GPU load |

Overlay mode does **not** work in exclusive-fullscreen games (the game bypasses topmost). Switch to **Gamma** mode when gaming (tray menu or `Ctrl+Alt+M`).

> **Gamma mode note:** Windows Night Light must be off (it uses the same gamma LUT).

## Hotkeys

- `Ctrl+Alt+F` — Toggle filter on/off
- `Ctrl+Alt+O` — Open settings panel
- `Ctrl+Alt+M` — Switch mode (Overlay ↔ Gamma)
- `Ctrl+Alt+R` — Open Relaxation & Awareness window

## FL-41 Color Presets

Rose-amber tones mimicking 480–520 nm blue-green blockage:

- **Indoor FL-41** `#E0A9AF` — office/screen (~35% opacity)
- **Warm FL-41** `#D98C8C` — true FL-41 tone (warm)
- **Deep FL-41** `#C97F7F` — dark environment / night

Adjust intensity with the opacity slider. Start low and increase gradually.

## Relaxation & Awareness Tools

The **Relief** window (`Ctrl+Alt+R` or tray menu) provides:

1. **4-7-8 Breathing** — animated breathing circle. Inhale 4s → hold 7s → exhale 8s, 4 cycles. Activates the parasympathetic nervous system, reducing stress and photophobia.
2. **20-20-20 Eye Break** — 20-second countdown. Every 20 min, look 20 ft (6m) away for 20 s. An automatic reminder pops up every 20 minutes.
3. **Habituation (Experimental)** — controlled short exposure to static-like stimulus. Based on neuroplasticity; some patients report reduced visual hypersensitivity. **Stop immediately if discomfort increases.**
4. **Awareness Info** — what VSS is, how FL-41 helps, daily management tips, with links to official research sources.

## VSS Safety

- **No flicker/animation** in the filter itself — static tint only (flicker is a VSS trigger).
- Transitions are instant (no fade).
- Default opacity starts low.

## GPU / Battery Optimization

- **Overlay**: single-color fill → GPU cost ~zero, `IsHitTestVisible=False`.
- **Gamma**: no WPF rendering at all → lowest load, doesn't affect game performance.
- Intel iGPU high-memory issue? Enable **Software Rendering** in settings (`RenderOptions.ProcessRenderMode = SoftwareOnly`).
- Gamma mode auto-refreshes every 30 s (driver reset protection).
- Overlay windows reposition every 5 s on monitor layout changes.

## Run / Build

```powershell
# Run from source
dotnet run -c Release

# Build
dotnet build -c Release

# Self-contained publish (single exe, no .NET runtime needed on target)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

The published exe will be in `publish/VisualSnowScreen.exe`.

## Architecture

```
App.xaml(.cs)                → startup, service wiring, render mode, 20-20-20 reminder
Native/NativeMethods.cs      → Win32 P/Invoke (click-through, gamma, hotkey)
Overlay/OverlayWindow        → per-monitor FL-41 click-through window
Overlay/OverlayWindowManager → multi-monitor window management
Gamma/GammaRampManager.cs    → SetDeviceGammaRamp FL-41 white-point
Services/FilterController    → mode coordination (overlay ↔ gamma)
Services/SettingsService     → JSON persistence
Services/SettingsViewModel   → opacity/preset/mode binding
Services/TrayIconController  → tray menu + programmatic FL-41 icon
Services/HotkeyService       → global hotkeys
Services/AutoStartService    → Windows autostart (registry)
UI/SettingsWindow            → settings panel (opacity slider, presets, mode)
UI/ReliefWindow              → 4-7-8 breathing, 20-20-20, habituation, awareness
UI/HabituationWindow         → experimental habituation screen
```

**Target:** .NET 8 (LTS), WPF + WinForms (for NotifyIcon), Per-Monitor V2 DPI, Windows 10/11.

## Research Sources

- [Visual Snow Initiative — Tips for Managing VSS](https://www.visualsnowinitiative.org/vss-tips)
- [Visual Snow Initiative — Chromatic Filters](https://www.visualsnowinitiative.org/chromatic-filters)
- [FL-41 Tint Reduces Activation of Neural Pathways of Photophobia (PMC10939838)](https://pmc.ncbi.nlm.nih.gov/articles/PMC10939838)
- [Diagnostic and Management Strategies of Visual Snow (PMC11930237)](https://pmc.ncbi.nlm.nih.gov/articles/PMC11930237)
- [EyeWiki — Visual Snow](https://eyewiki.org/Visual_Snow)
- [TheraSpecs — FL-41 Glasses](https://www.theraspecs.com/fl-41-glasses)

## Disclaimer

This application is **not a medical device** and does **not** replace professional medical advice. Visual Snow Syndrome is a neurological condition — consult a neuro-ophthalmologist for diagnosis and treatment. The habituation technique is experimental. Stop any exercise if discomfort increases.

---

# Türkçe Açıklama

**Visual Snow Therapy**, Visual Snow Syndrome (VSS) ve fotofobi için ekranı tam **FL-41 (gül kurusu / rose-amber)** renk spektrumuna dönüştüren çok hafif bir Windows masaüstü ekran katmanı uygulamasıdır. C# ve WPF (.NET 8) ile yazılmıştır.

Ayrıca kanıt destekli rahatlama araçları içerir: **4-7-8 nefes**, **20-20-20 göz molası**, deneysel **alışma (habituation)** ekranı ve resmi araştırma kaynaklı VSS farkındalık bilgisi.

## FL-41 Nedir?

**FL-41** ("Fluorescent 41"), 1980'lerde geliştirilmiş özel bir gül kurusu optik tintidir. Tüm ışığı eşit azaltan güneş gözlüklerinden farklı olarak FL-41 **seçici olarak 480–520 nm mavi-yeşil spektrumu bloke eder** — fotofobi, migren ve görsel rahatsızlıkla en güçlü ilişkili dalga boyları.

Klinik araştırmalar (PMC10939838), FL-41 camların fotofobi nöral yollarında (S1, S2, insula, ACC) **BOLD aktivasyonunu anlamlı ölçüde azalttığını** gösteriyor. Bu uygulama aynı etkiyi ekran katmanı olarak taklit eder.

## Özellikler

- ✅ En üst katmanda tam ekran overlay (`Topmost = true`)
- ✅ Tıklamaları alt pencerelere geçiren click-through (`WS_EX_TRANSPARENT` + `WS_EX_LAYERED`)
- ✅ Opaklık kaydırıcı ile hassas FL-41 yoğunluğu
- ✅ Hibrit render: Overlay (hassas renk) **ve** Gamma Ramp (oyun dostu)
- ✅ Çoklu monitör + Per-Monitor V2 DPI farkındalığı
- ✅ Sistem tepsisi ikonu + menü
- ✅ Global kısayollar
- ✅ Windows açılışında otomatik başlatma
- ✅ Rahatlama araçları: 4-7-8 nefes, 20-20-20, alışma, farkındalık
- ✅ Ayar kalıcılığı (`%AppData%\VisualSnowScreen\settings.json`)

## İki Render Modu (Hibrit)

| Mod | Yöntem | Ne zaman |
|---|---|---|
| **Overlay** | WPF click-through yarı saydam pencere | İş, masaüstü, tarayıcı — hassas FL-41 renk + opaklık |
| **Gamma** | Donanımsal gamma LUT (`SetDeviceGammaRamp`) | Oyun (RTX 4050), pil (Ryzen iGPU) — fullscreen uyumlu, sıfır GPU yükü |

Overlay modu exclusive-fullscreen oyunlarda çalışmaz. Oyun oynarken **Gamma** moduna geçin (tray menüsü veya `Ctrl+Alt+M`).

> **Gamma modu notu:** Windows Night Light kapalı olmalı.

## Kısayollar

- `Ctrl+Alt+F` — Filtre aç/kapa
- `Ctrl+Alt+O` — Ayarlar paneli
- `Ctrl+Alt+M` — Mod değiştir (Overlay ↔ Gamma)
- `Ctrl+Alt+R` — Rahatlama & Farkındalık penceresi

## FL-41 Renk Preset'leri

- **Indoor FL-41** `#E0A9AF` — ofis/ekran (~%35 opaklık)
- **Warm FL-41** `#D98C8C` — gerçek FL-41 tonu (sıcak)
- **Deep FL-41** `#C97F7F` — koyu ortam / gece

## Rahatlama Araçları

1. **4-7-8 Nefes** — animasyonlu nefes dairesi. 4 sn al → 7 sn tut → 8 sn ver, 4 döngü. Parasympatik sinir sistemini aktive eder.
2. **20-20-20 Göz Molası** — 20 sn geri sayım. Her 20 dk'da 20 ft (6m) uzağa 20 sn bak. Otomatik hatırlatma her 20 dk'da bir.
3. **Alışma (Deneysel)** — kontrollü statik benzeri uyarana kısa maruziyet. Nöroplastisite prensibi. **Rahatsızlık artarsa hemen durdur.**
4. **Farkındalık** — VSS nedir, FL-41 nasıl yardımcı olur, günlük yönetim ipuçları, resmi kaynak linkleriyle.

## Çalıştırma

```powershell
dotnet run -c Release

# Self-contained tek exe
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

## Araştırma Kaynakları

- [Visual Snow Initiative — VSS Yönetim İpuçları](https://www.visualsnowinitiative.org/vss-tips)
- [Visual Snow Initiative — Kromatik Filtreler](https://www.visualsnowinitiative.org/chromatic-filters)
- [FL-41 Fotofobi Nöral Yol Azaltması (PMC10939838)](https://pmc.ncbi.nlm.nih.gov/articles/PMC10939838)
- [Visual Snow Tanı ve Yönetim (PMC11930237)](https://pmc.ncbi.nlm.nih.gov/articles/PMC11930237)
- [EyeWiki — Visual Snow](https://eyewiki.org/Visual_Snow)

## Yasal Uyarı

Bu uygulama bir **tıbbi cihaz değildir** ve profesyonel tıbbi tavsiyenin yerine geçmez. Visual Snow Syndrome nörolojik bir durumdur — tanı ve tedavi için bir nöro-oftalmoloğa başvurun. Alışma tekniği deneyseldir. Rahatsızlık artarsa egzersizi hemen bırakın.