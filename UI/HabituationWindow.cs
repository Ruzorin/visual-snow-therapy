using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace VisualSnowScreen.UI;

/// <summary>
/// Deneysel habituation penceresi: kontrollü statik benzeri uyarana kısa süre maruziyet.
/// Nöroplastisite prensibiyle görsel hiperaktivite azaltma hipotezi.
///
/// Kaynak: Visual Snow Initiative — habituation tekniği açıklaması.
/// DENEYSEL — rahatsızlık artarsa ESC ile kapat.
/// </summary>
public partial class HabituationWindow : Window
{
  private readonly DispatcherTimer _timer;
  private int _remaining;

  public HabituationWindow(int seconds)
  {
    InitializeComponent();
    _remaining = seconds;
    RemainingText.Text = seconds.ToString();
    _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
    _timer.Tick += Tick;
  }

  private void Tick(object? sender, EventArgs e)
  {
    _remaining--;
    if (_remaining <= 0)
    {
      _timer.Stop();
      RemainingText.Text = "✓";
      PhaseText.Text = "Tamamlandı";
      Close();
      return;
    }
    RemainingText.Text = _remaining.ToString();
  }

  protected override void OnContentRendered(EventArgs e)
  {
    base.OnContentRendered(e);
    _timer.Start();
  }

  private void Close_Click(object sender, RoutedEventArgs e) => Close();

  private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
  {
    if (e.Key == System.Windows.Input.Key.Escape) Close();
  }
}