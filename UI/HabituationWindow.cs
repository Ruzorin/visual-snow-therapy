using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

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
    Loaded += (_, _) => DrawStaticNoise();
  }

  /// <summary>Canvas'a statik benzeri rastgele beyaz/gri noktalar çizer.</summary>
  private void DrawStaticNoise()
  {
    var rnd = new Random(42);
    double w = StaticCanvas.ActualWidth > 0 ? StaticCanvas.ActualWidth : 560;
    double h = StaticCanvas.ActualHeight > 0 ? StaticCanvas.ActualHeight : 360;

    for (int i = 0; i < 1200; i++)
    {
      var x = rnd.NextDouble() * w;
      var y = rnd.NextDouble() * h;
      var gray = rnd.Next(120, 255);
      var size = rnd.Next(1, 3);
      var dot = new Rectangle
      {
        Width = size,
        Height = size,
        Fill = new SolidColorBrush(Color.FromRgb((byte)gray, (byte)gray, (byte)gray))
      };
      Canvas.SetLeft(dot, x);
      Canvas.SetTop(dot, y);
      StaticCanvas.Children.Add(dot);
    }
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