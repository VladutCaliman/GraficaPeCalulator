using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Tema1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DrawingHandler _drawingHandler;

        private enum InteractionMode
        {
            None,
            Scale,
            Rotate,
            Translate,
            Symmetry
        }

        private InteractionMode _currentMode = InteractionMode.None;
        private bool _isDragging;
        private Point _lastMousePosition;

        private const double ScaleSpeed = 0.01;
        private const double RotationSpeed = 0.5;

        private bool _isDefiningSymmetryLine;
        private bool _hasSymmetryLineStart;
        private Point _symmetryLineStart;
        private Line _symmetryPreviewLine;

        public MainWindow()
        {
            InitializeComponent();

            _drawingHandler = new DrawingHandler(DrawingCanvas);

            DrawingCanvas.MouseLeftButtonDown += DrawingCanvas_MouseLeftButtonDown;
            DrawingCanvas.MouseRightButtonDown += DrawingCanvas_MouseRightButtonDown;
            DrawingCanvas.MouseMove += DrawingCanvas_MouseMove;
            DrawingCanvas.MouseLeftButtonUp += DrawingCanvas_MouseLeftButtonUp;
            DrawingCanvas.MouseLeave += DrawingCanvas_MouseLeave;

            ResetButton.Click += ResetButton_Click;
            RotateButton.Click += RotateButton_Click;
            ScaleButton.Click += ScaleButton_Click;
            TranslateButton.Click += TranslateButton_Click;
            SymmetryOriginButton.Click += SymmetryOriginButton_Click;
            SymmetryLineButton.Click += SymmetryLineButton_Click;
        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(DrawingCanvas);

            if (_isDefiningSymmetryLine)
            {
                HandleSymmetryLineClick(position);
                return;
            }

            if (_currentMode == InteractionMode.None)
            {
                _drawingHandler.AddPoint(position);
            }
            else
            {
                _isDragging = true;
                _lastMousePosition = position;
                DrawingCanvas.CaptureMouse();
            }
        }
        private void DrawingCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _drawingHandler.ClosePolygon();
        }


        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _drawingHandler.Reset();
            _currentMode = InteractionMode.None;
            UpdateStatusLabel();
            _isDragging = false;
            DrawingCanvas.ReleaseMouseCapture();

            _isDefiningSymmetryLine = false;
            _hasSymmetryLineStart = false;
            RemoveSymmetryPreviewLine();
        }
        private bool UseOrigin()
        {
            return OriginRadio.IsChecked == true;
        }
        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMode(InteractionMode.Rotate);
        }
        private void ScaleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMode(InteractionMode.Scale);
        }
        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMode(InteractionMode.Translate);
        }
        private void SymmetryOriginButton_Click(object sender, RoutedEventArgs e)
        {
            _currentMode = InteractionMode.Symmetry;
            _drawingHandler.SymmetryOrigin(UseOrigin());
            UpdateStatusLabel();
        }
        private void SymmetryLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isDefiningSymmetryLine)
            {
                _isDefiningSymmetryLine = false;
                _hasSymmetryLineStart = false;
                RemoveSymmetryPreviewLine();
            }
            else
            {
                _currentMode = InteractionMode.None;
                _isDragging = false;
                DrawingCanvas.ReleaseMouseCapture();

                _isDefiningSymmetryLine = true;
                _hasSymmetryLineStart = false;
                RemoveSymmetryPreviewLine();
            }
            UpdateStatusLabel();
        }
        private void ToggleMode(InteractionMode mode)
        {
            if (_currentMode == mode)
            {
                _currentMode = InteractionMode.None;
            }
            else
            {
                _currentMode = mode;
            }

            UpdateStatusLabel();

            _isDragging = false;
            DrawingCanvas.ReleaseMouseCapture();
        }
        private void UpdateStatusLabel()
        {
            if (_isDefiningSymmetryLine)
            {
                StatusLabel.Text = _hasSymmetryLineStart
                    ? "Symmetry: Click to set END point"
                    : "Symmetry: Click to set START point";
                StatusLabel.Foreground = System.Windows.Media.Brushes.DeepPink;
                return;
            }

            switch (_currentMode)
            {
                case InteractionMode.None:
                    StatusLabel.Text = "None (Drawing)";
                    StatusLabel.Foreground = System.Windows.Media.Brushes.Blue;
                    break;
                case InteractionMode.Rotate:
                    StatusLabel.Text = "Rotating (Oy)";
                    StatusLabel.Foreground = System.Windows.Media.Brushes.Red;
                    break;
                case InteractionMode.Scale:
                    StatusLabel.Text = "Scaling (Ox)";
                    StatusLabel.Foreground = System.Windows.Media.Brushes.DarkGreen;
                    break;
                case InteractionMode.Translate:
                    StatusLabel.Text = "Translating";
                    StatusLabel.Foreground = System.Windows.Media.Brushes.DarkOrange;
                    break;
                case InteractionMode.Symmetry:
                    StatusLabel.Text = "Symmetry (Origin)";
                    StatusLabel.Foreground = System.Windows.Media.Brushes.DeepPink;
                    break;
            }
        }
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var current = e.GetPosition(DrawingCanvas);

            if (_isDefiningSymmetryLine && _hasSymmetryLineStart)
            {
                UpdateSymmetryPreviewLine(_symmetryLineStart, current);
                return;
            }

            if (!_isDragging)
            {
                return;
            }

            switch (_currentMode)
            {
                case InteractionMode.Scale:
                    {
                        var deltaX = current.X - _lastMousePosition.X;
                        var factor = 1.0 + deltaX * ScaleSpeed;
                        if (factor > 0)
                        {
                            _drawingHandler.Scale(factor, factor, UseOrigin());
                        }
                        break;
                    }
                case InteractionMode.Rotate:
                    {
                        var deltaY = _lastMousePosition.Y - current.Y;
                        var angle = deltaY * RotationSpeed;
                        _drawingHandler.Rotate(angle, UseOrigin());
                        break;
                    }
                case InteractionMode.Translate:
                    {
                        _drawingHandler.TranslateTo(current);
                        break;
                    }
            }

            _lastMousePosition = current;
        }
        private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            DrawingCanvas.ReleaseMouseCapture();
        }
        private void DrawingCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _isDragging = false;
            DrawingCanvas.ReleaseMouseCapture();
        }
        private void HandleSymmetryLineClick(Point position)
        {
            if (!_hasSymmetryLineStart)
            {
                _symmetryLineStart = position;
                _hasSymmetryLineStart = true;
                UpdateSymmetryPreviewLine(_symmetryLineStart, _symmetryLineStart);
                UpdateStatusLabel();
            }
            else
            {
                var end = position;
                UpdateSymmetryPreviewLine(_symmetryLineStart, end);
                _drawingHandler.SymmetryLine(_symmetryLineStart, end);

                _isDefiningSymmetryLine = false;
                _hasSymmetryLineStart = false;
                RemoveSymmetryPreviewLine();
                UpdateStatusLabel();
            }
        }
        private void UpdateSymmetryPreviewLine(Point start, Point end)
        {
            if (_symmetryPreviewLine == null)
            {
                _symmetryPreviewLine = new Line
                {
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 1,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection { 4, 4 }
                };
                DrawingCanvas.Children.Add(_symmetryPreviewLine);
            }

            _symmetryPreviewLine.X1 = start.X;
            _symmetryPreviewLine.Y1 = start.Y;
            _symmetryPreviewLine.X2 = end.X;
            _symmetryPreviewLine.Y2 = end.Y;
        }
        private void RemoveSymmetryPreviewLine()
        {
            if (_symmetryPreviewLine != null)
            {
                DrawingCanvas.Children.Remove(_symmetryPreviewLine);
                _symmetryPreviewLine = null;
            }
        }
    }
}