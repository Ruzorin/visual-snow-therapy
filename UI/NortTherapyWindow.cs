using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VisualSnowScreen.Services;

namespace VisualSnowScreen.UI;

public partial class NortTherapyWindow : Window
{
  private readonly DispatcherTimer _sessionTimer = new();
  private readonly DispatcherTimer _saccadeTimer = new();
  private readonly Random _rnd = new();

  private int _totalSecondsRemaining = 180; // 3 minutes total limit (safety valve)
  private bool _isSaccadeMode = false;
  private Storyboard? _pursuitStoryboard;

  public NortTherapyWindow()
  {
    InitializeComponent();

    _sessionTimer.Interval = TimeSpan.FromSeconds(1);
    _sessionTimer.Tick += SessionTimer_Tick;

    _saccadeTimer.Interval = TimeSpan.FromSeconds(1.2); // Saccade jump speed
    _saccadeTimer.Tick += SaccadeTimer_Tick;
  }

  private void Window_Loaded(object sender, RoutedEventArgs e)
  {
    ApplyLocalization();

    // The MatrixAnimationUsingPath automatically positions relative to the top-left (0,0) of the canvas if left un-transformed
    Canvas.SetLeft(TargetDot, 0);
    Canvas.SetTop(TargetDot, 0);
    TargetDot.Visibility = Visibility.Visible;

    // Fade out the info overlay softly after 3 seconds
    var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1))
    {
      BeginTime = TimeSpan.FromSeconds(3)
    };
    InfoOverlay.BeginAnimation(UIElement.OpacityProperty, fadeOut);

    StartSmoothPursuit();
    _sessionTimer.Start();
  }

  private void SessionTimer_Tick(object? sender, EventArgs e)
  {
    _totalSecondsRemaining--;
    TimeSpan ts = TimeSpan.FromSeconds(_totalSecondsRemaining);
    TxtTimer.Text = $"{ts.Minutes:D2}:{ts.Seconds:D2}";

    // Phase 1: 3:00 to 1:30 is Smooth Pursuit
    // Phase 2: 1:30 to 0:00 is Saccades
    if (_totalSecondsRemaining == 90 && !_isSaccadeMode)
    {
      _isSaccadeMode = true;
      StopSmoothPursuit();
      StartSaccades();

      TxtTitle.Text = LocalizationService.S("nortSaccadeTitle");
      TxtDesc.Text = LocalizationService.S("nortSaccadeDesc");

      // Show info briefly when switching mode
      var fadeInOut = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
      fadeInOut.AutoReverse = true;
      fadeInOut.Duration = TimeSpan.FromSeconds(3);
      fadeInOut.FillBehavior = FillBehavior.HoldEnd;
      InfoOverlay.BeginAnimation(UIElement.OpacityProperty, fadeInOut);
    }

    if (_totalSecondsRemaining <= 0)
    {
      FinishTherapy();
    }
  }

  private void FinishTherapy()
  {
    _sessionTimer.Stop();
    _saccadeTimer.Stop();
    StopSmoothPursuit();

    TargetDot.Visibility = Visibility.Collapsed;

    TxtTitle.Text = LocalizationService.S("nortDoneTitle");
    TxtDesc.Text = LocalizationService.S("nortDoneDesc");
    TxtTitle.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#A6E3A1")); // Green success

    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
    InfoOverlay.BeginAnimation(UIElement.OpacityProperty, fadeIn);

    // Auto close after 4 seconds
    var closeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
    closeTimer.Tick += (s, ev) =>
    {
      closeTimer.Stop();
      this.Close();
    };
    closeTimer.Start();
  }

  private void StartSmoothPursuit()
  {
    TxtTitle.Text = LocalizationService.S("nortPursuitTitle");

    // Use the actual rendered canvas size (DPI-correct) instead of SystemParameters
    double width = ExerciseCanvas.ActualWidth > 0 ? ExerciseCanvas.ActualWidth : SystemParameters.PrimaryScreenWidth;
    double height = ExerciseCanvas.ActualHeight > 0 ? ExerciseCanvas.ActualHeight : SystemParameters.PrimaryScreenHeight;

    // Build a Figure-8 (infinity) path that stays safely within the visible area.
    // We keep the entire curve inside the central 70% of the screen so the dot never clips.
    double midX = width / 2;
    double midY = height / 2;
    double rx = width * 0.30;   // horizontal half-span of the loop
    double ry = height * 0.22;  // vertical half-span of the loop

    PathGeometry geometry = new PathGeometry();
    PathFigure figure = new PathFigure { StartPoint = new System.Windows.Point(midX, midY) };

    // Right loop of the figure-8: center -> top-right -> bottom-right -> center
    figure.Segments.Add(new BezierSegment(
        new System.Windows.Point(midX + rx, midY - ry),
        new System.Windows.Point(midX + rx, midY + ry),
        new System.Windows.Point(midX, midY),
        true));

    // Left loop of the figure-8: center -> top-left -> bottom-left -> center
    figure.Segments.Add(new BezierSegment(
        new System.Windows.Point(midX - rx, midY - ry),
        new System.Windows.Point(midX - rx, midY + ry),
        new System.Windows.Point(midX, midY),
        true));

    geometry.Figures.Add(figure);

    // MatrixAnimationUsingPath moves the element's top-left to the path point.
    // Offset the dot by -half its size so the CENTER of the dot follows the path.
    Canvas.SetLeft(TargetDot, -TargetDot.Width / 2);
    Canvas.SetTop(TargetDot, -TargetDot.Height / 2);

    MatrixTransform matrixTransform = new MatrixTransform();
    TargetDot.RenderTransform = matrixTransform;

    MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath
    {
      PathGeometry = geometry,
      Duration = TimeSpan.FromSeconds(10), // Slow, smooth pursuit
      RepeatBehavior = RepeatBehavior.Forever,
      DoesRotateWithTangent = false
    };

    _pursuitStoryboard = new Storyboard();
    _pursuitStoryboard.Children.Add(matrixAnimation);
    Storyboard.SetTarget(matrixAnimation, TargetDot);
    Storyboard.SetTargetProperty(matrixAnimation, new PropertyPath("RenderTransform.Matrix"));

    _pursuitStoryboard.Begin();
  }

  private void StopSmoothPursuit()
  {
    _pursuitStoryboard?.Stop();
    TargetDot.RenderTransform = null;
    Canvas.SetLeft(TargetDot, 0);
    Canvas.SetTop(TargetDot, 0);
  }

  private void StartSaccades()
  {
    _saccadeTimer.Start();
    CenterFixation.Visibility = Visibility.Visible;

    double width = ExerciseCanvas.ActualWidth > 0 ? ExerciseCanvas.ActualWidth : SystemParameters.PrimaryScreenWidth;
    double height = ExerciseCanvas.ActualHeight > 0 ? ExerciseCanvas.ActualHeight : SystemParameters.PrimaryScreenHeight;

    Canvas.SetLeft(CenterFixation, (width / 2) - CenterFixation.Width / 2);
    Canvas.SetTop(CenterFixation, (height / 2) - CenterFixation.Height / 2);

    JumpDot(); // initial jump
  }

  private void SaccadeTimer_Tick(object? sender, EventArgs e)
  {
    JumpDot();
  }

  private void JumpDot()
  {
    double width = ExerciseCanvas.ActualWidth > 0 ? ExerciseCanvas.ActualWidth : SystemParameters.PrimaryScreenWidth;
    double height = ExerciseCanvas.ActualHeight > 0 ? ExerciseCanvas.ActualHeight : SystemParameters.PrimaryScreenHeight;

    // Keep the dot fully visible with a comfortable margin from the screen edges
    double margin = 80; // pixels of safe padding on every side
    double dotSize = TargetDot.Width;

    double minX = margin;
    double maxX = width - margin - dotSize;
    double minY = margin;
    double maxY = height - margin - dotSize;

    // Guard against tiny screens
    if (maxX <= minX) maxX = minX + 1;
    if (maxY <= minY) maxY = minY + 1;

    double nextX = _rnd.NextDouble() * (maxX - minX) + minX;
    double nextY = _rnd.NextDouble() * (maxY - minY) + minY;

    Canvas.SetLeft(TargetDot, nextX);
    Canvas.SetTop(TargetDot, nextY);
    TargetDot.RenderTransform = null; // Clear any matrix transform left from pursuit phase
  }

  private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
  {
    if (e.Key == Key.Escape)
    {
      _sessionTimer.Stop();
      _saccadeTimer.Stop();
      this.Close();
    }
  }

  private void ApplyLocalization()
  {
    TxtTitle.Text = LocalizationService.S("nortTitle") ?? "Neuro-Optometric Rehabilitation Therapy";
    TxtDesc.Text = LocalizationService.S("nortDesc") ?? "NORT Session";
    TxtTimeLeft.Text = LocalizationService.S("nortTimeLeft") ?? "Kalan Süre:";
    TxtAutoClose.Text = LocalizationService.S("nortAutoClose") ?? "Egzersiz bittiğinde otomatik kapanır.";
    TxtEsc.Text = LocalizationService.S("nortEsc") ?? "Çıkmak için ESC'ye basın.";
  }
}
