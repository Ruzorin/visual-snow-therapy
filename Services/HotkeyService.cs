using System.Windows.Interop;
using VisualSnowScreen.Native;

namespace VisualSnowScreen.Services;

/// <summary>
/// Global klavye kısayolları (RegisterHotKey). Overlay click-through olduğu için
/// fare ile etkileşim mümkün değil — kısayollar ana etkileşim yolu.
///
/// Kısayollar:
///   Ctrl+Alt+F → Filtre aç/kapa
///   Ctrl+Alt+O → Ayarlar paneli
///   Ctrl+Alt+M → Mod değiştir (Overlay ↔ Gamma)
/// </summary>
public sealed class HotkeyService : IDisposable
{
  private const int ID_TOGGLE = 9001;
  private const int ID_SETTINGS = 9002;
  private const int ID_MODE = 9003;
  private const int ID_RELIEF = 9004;

  private IntPtr _hwnd;
  private HwndSource? _source;
  private bool _registered;

  public event Action? ToggleRequested;
  public event Action? SettingsRequested;
  public event Action? ModeSwitchRequested;
  public event Action? ReliefRequested;

  /// <summary>Gizli bir mesaj-only pencere oluşturup kısayolları kaydeder.</summary>
  public void Register()
  {
    if (_registered) return;

    // Mesaj-only pencere (görünmez) — WM_HOTKEY almak için.
    var parameters = new HwndSourceParameters("VisualSnowHotkeySink")
    {
      WindowStyle = 0,
      ExtendedWindowStyle = 0,
      PositionX = 0,
      PositionY = 0,
      Width = 0,
      Height = 0
    };
    _source = new HwndSource(parameters);
    _hwnd = _source.Handle;
    _source.AddHook(WndProc);

    int mods = NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT | NativeMethods.MOD_NOREPEAT;
    NativeMethods.RegisterHotKey(_hwnd, ID_TOGGLE, mods, 0x46);    // F
    NativeMethods.RegisterHotKey(_hwnd, ID_SETTINGS, mods, 0x4F);  // O
    NativeMethods.RegisterHotKey(_hwnd, ID_MODE, mods, 0x4D);      // M
    NativeMethods.RegisterHotKey(_hwnd, ID_RELIEF, mods, 0x52);    // R
    _registered = true;
  }

  private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    if (msg == NativeMethods.WM_HOTKEY)
    {
      int id = wParam.ToInt32();
      switch (id)
      {
        case ID_TOGGLE: ToggleRequested?.Invoke(); break;
        case ID_SETTINGS: SettingsRequested?.Invoke(); break;
        case ID_MODE: ModeSwitchRequested?.Invoke(); break;
        case ID_RELIEF: ReliefRequested?.Invoke(); break;
      }
      handled = true;
    }
    return IntPtr.Zero;
  }

  public void Dispose()
  {
    if (!_registered) return;
    NativeMethods.UnregisterHotKey(_hwnd, ID_TOGGLE);
    NativeMethods.UnregisterHotKey(_hwnd, ID_SETTINGS);
    NativeMethods.UnregisterHotKey(_hwnd, ID_MODE);
    NativeMethods.UnregisterHotKey(_hwnd, ID_RELIEF);
    _source?.Dispose();
    _registered = false;
  }
}