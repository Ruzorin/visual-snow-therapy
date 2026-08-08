using System.Windows.Media;
using VisualSnowScreen.Native;
using Color = System.Windows.Media.Color;

namespace VisualSnowScreen.Gamma;

/// <summary>
/// Donanımsal ekran gamma LUT'unu (SetDeviceGammaRamp) değiştirerek FL-41
/// white-point yaklaşımı uygular. f.lux / Windows Night Light yöntemi.
///
/// Avantajları:
///  - Exclusive fullscreen oyunlar DAHİL her yerde çalışır (overlay pencere yok).
///  - Sıfır composite/GPU yükü — pil ve oyun dostu.
///  - RTX 4050 oyun performansını etkilemez.
///
/// Mekanizma: Her kanalın (R,G,B) 256 girişli LUT'unu doğrusal identity yerine
/// ölçeklenmiş bir eğriyle değiştirir. FL-41 için:
///  - Mavi (480-520nm) en çok azaltılır.
///  - Yeşil orta derecede azaltılır.
///  - Kırmızı en az azaltılır → rose-amber white point.
///
/// Not: Windows Night Light ile çakışır; kullanıcı Night Light'ı kapatmalı.
/// Bazı sürücüler LUT'u resetler; bu yüzden periyodik yenileme önerilir.
/// </summary>
public sealed class GammaRampManager : IDisposable
{
  // Orijinal (identity) ramp — geri yüklemek için saklanır.
  private NativeMethods.RAMP? _originalRamp;
  private bool _applied;

  /// <summary>FL-41 white-point gamma ramp uygular.</summary>
  /// <param name="intensity">0.0 (etkisiz) - 1.0 (maksimum rose-amber kaydırma).</param>
  /// <param name="color">FL-41 renk (kanal oranları için; alpha yok sayılır).</param>
  public void Apply(double intensity, Color color)
  {
    intensity = Math.Clamp(intensity, 0.0, 1.0);

    // İlk uygulamada orijinal ramp'i sakla (geri yükleme için).
    if (_originalRamp == null)
    {
      var id = CreateIdentityRamp();
      _originalRamp = id;
    }

    // FL-41 kanal ölçekleri: rengin normalize RGB'si + intensity.
    // color zaten rose-amber; onu white-point hedefi olarak kullan.
    // Düşük intensity → identity'e yakın; yüksek → renge yakınsar.
    var rScale = Lerp(1.0, color.R / 255.0, intensity);
    var gScale = Lerp(1.0, color.G / 255.0, intensity);
    var bScale = Lerp(1.0, color.B / 255.0, intensity);

    var ramp = new NativeMethods.RAMP
    {
      Red = new ushort[256],
      Green = new ushort[256],
      Blue = new ushort[256]
    };

    for (int i = 0; i < 256; i++)
    {
      // Doğrusal identity * kanal ölçeği. Gamma eğrisi basit lineer tutulur
      // (karmaşık eğri VSS'de flicker algısını artırabilir).
      ushort baseVal = (ushort)(i * 257); // 0..65535 lineer
      ramp.Red[i] = ScaleClamp(baseVal, rScale);
      ramp.Green[i] = ScaleClamp(baseVal, gScale);
      ramp.Blue[i] = ScaleClamp(baseVal, bScale);
    }

    var dc = NativeMethods.GetDC(IntPtr.Zero);
    try
    {
      _applied = NativeMethods.SetDeviceGammaRamp(dc, ref ramp);
    }
    finally
    {
      NativeMethods.ReleaseDC(IntPtr.Zero, dc);
    }
  }

  /// <summary>Gamma ramp'i orijinal identity değerine geri yükler.</summary>
  public void Restore()
  {
    if (!_applied || _originalRamp == null) return;
    var ramp = _originalRamp.Value;
    var dc = NativeMethods.GetDC(IntPtr.Zero);
    try
    {
      NativeMethods.SetDeviceGammaRamp(dc, ref ramp);
    }
    finally
    {
      NativeMethods.ReleaseDC(IntPtr.Zero, dc);
    }
    _applied = false;
  }

  /// <summary>Gamma ramp'i yeniden uygular (sürücü reset sonrası).</summary>
  public void Refresh(double intensity, Color color)
  {
    if (_applied) Apply(intensity, color);
  }

  private static NativeMethods.RAMP CreateIdentityRamp()
  {
    var ramp = new NativeMethods.RAMP
    {
      Red = new ushort[256],
      Green = new ushort[256],
      Blue = new ushort[256]
    };
    for (int i = 0; i < 256; i++)
    {
      ramp.Red[i] = (ushort)(i * 257);
      ramp.Green[i] = (ushort)(i * 257);
      ramp.Blue[i] = (ushort)(i * 257);
    }
    return ramp;
  }

  private static ushort ScaleClamp(ushort value, double scale)
  {
    var v = (int)(value * scale);
    return (ushort)Math.Clamp(v, 0, 65535);
  }

  private static double Lerp(double a, double b, double t) => a + (b - a) * t;

  public void Dispose() => Restore();
}