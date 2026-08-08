using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using VisualSnowScreen.Models;
using Application = System.Windows.Application;

namespace VisualSnowScreen.Services;

/// <summary>
/// Sistem tepsisi ikonu + context menü. Overlay click-through olduğu için
/// tray menüsü ana etkileşim noktasıdır.
/// </summary>
public sealed class TrayIconController : IDisposable
{
  private readonly NotifyIcon _icon;
  private readonly SettingsService _settings;
  private readonly FilterController _filter;

  public event Action? SettingsRequested;
  public event Action? ReliefRequested;

  public TrayIconController(SettingsService settings, FilterController filter)
  {
    _settings = settings;
    _filter = filter;

    _icon = new NotifyIcon
    {
      Icon = CreateFl41Icon(),
      Text = "Visual Snow FL-41",
      Visible = true
    };
    _icon.DoubleClick += (_, _) => SettingsRequested?.Invoke();
    _icon.ContextMenuStrip = BuildMenu();
  }

  private ContextMenuStrip BuildMenu()
  {
    var L = LocalizationService.S;
    var menu = new ContextMenuStrip();

    var miToggle = new ToolStripMenuItem(L("FilterOn")) { CheckOnClick = true };
    miToggle.Click += (_, _) =>
    {
      _settings.Current.Enabled = miToggle.Checked;
      _settings.Save();
      _filter.Apply();
      UpdateToggleLabel(miToggle);
    };
    menu.Items.Add(miToggle);

    var miMode = new ToolStripMenuItem(L("Mode"));
    var miOverlay = new ToolStripMenuItem(L("Overlay")) { CheckOnClick = true };
    var miGamma = new ToolStripMenuItem(L("Gamma")) { CheckOnClick = true };
    miOverlay.Click += (_, _) => SetMode(RenderMode.Overlay, miOverlay, miGamma);
    miGamma.Click += (_, _) => SetMode(RenderMode.Gamma, miGamma, miOverlay);
    miMode.DropDownItems.AddRange(new ToolStripItem[] { miOverlay, miGamma });
    menu.Items.Add(miMode);

    menu.Items.Add(new ToolStripSeparator());

    var miSettings = new ToolStripMenuItem(L("Settings"));
    miSettings.Click += (_, _) => SettingsRequested?.Invoke();
    menu.Items.Add(miSettings);

    var miRelief = new ToolStripMenuItem(L("Relief"));
    miRelief.Click += (_, _) => ReliefRequested?.Invoke();
    menu.Items.Add(miRelief);

    menu.Items.Add(new ToolStripSeparator());

    var miExit = new ToolStripMenuItem(L("Exit"));
    miExit.Click += (_, _) =>
    {
      _settings.Current.Enabled = false;
      _filter.Apply(); // gamma'yı geri yükle
      _icon.Visible = false;
      Application.Current.Shutdown();
    };
    menu.Items.Add(miExit);

    // İlk durum senkronize et
    miToggle.Checked = _settings.Current.Enabled;
    UpdateToggleLabel(miToggle);
    if (_settings.Current.Mode == RenderMode.Overlay) { miOverlay.Checked = true; }
    else { miGamma.Checked = true; }

    return menu;
  }

  private void SetMode(RenderMode mode, ToolStripMenuItem active, ToolStripMenuItem other)
  {
    active.Checked = true;
    other.Checked = false;
    _settings.Current.Mode = mode;
    _settings.Save();
    _filter.Apply();
  }

  private static void UpdateToggleLabel(ToolStripMenuItem mi)
      => mi.Text = mi.Checked ? "Filtre: AÇIK" : "Filtre: KAPALI";

  /// <summary>FL-41 rose-amber renkli programmatik ikon (harici .ico gerektirmez).</summary>
  private static Icon CreateFl41Icon()
  {
    using var bmp = new Bitmap(16, 16);
    using var g = Graphics.FromImage(bmp);
    // FL-41 rose-amber dolgu
    using var brush = new SolidBrush(Color.FromArgb(224, 169, 175));
    g.FillRectangle(brush, 0, 0, 16, 16);
    // Kar tanesi benzeri "visual snow" işareti — beyaz noktalar
    using var snow = new SolidBrush(Color.FromArgb(180, 255, 255, 255));
    var rnd = new Random(42);
    for (int i = 0; i < 14; i++)
    {
      int x = rnd.Next(2, 14);
      int y = rnd.Next(2, 14);
      g.FillRectangle(snow, x, y, 1, 1);
    }
    var handle = bmp.GetHicon();
    return Icon.FromHandle(handle);
  }

  public void Dispose()
  {
    _icon.Visible = false;
    _icon.Dispose();
  }
}