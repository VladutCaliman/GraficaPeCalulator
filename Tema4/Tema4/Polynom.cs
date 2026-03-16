using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class Polynom
{
    private readonly Canvas _canvas;
    private readonly List<Point> _points = new List<Point>();
    private readonly List<Ellipse> _ellipses = new List<Ellipse>();
    private Polyline? _polyline;
    private bool _isDrawn = false;

    // Newton Coefficients (Divided Differences)
    private double[] _coefficients = Array.Empty<double>();

    // Dragging logic
    private Ellipse? _draggingEllipse = null;
    private int _draggingIndex = -1;
    private Point _dragOffset;

    public Polynom(Canvas canvas)
    {
        _canvas = canvas;
        _canvas.MouseLeftButtonDown += Polynom_MouseLeftButtonDown;
        _canvas.MouseRightButtonDown += Polynom_MouseRightButtonDown;
    }

    private void Polynom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Point clickPoint = e.GetPosition(_canvas);

        if (!_isDrawn)
        {
            // Ensure X coordinates are strictly increasing to avoid division by zero
            if (_points.Count > 0 && clickPoint.X <= _points[^1].X)
                return;

            _points.Add(clickPoint);
            Ellipse ellipse = CreateEllipse(clickPoint);
            _ellipses.Add(ellipse);
            _canvas.Children.Add(ellipse);
        }
        else
        {
            // Dragging existing points
            for (int i = 0; i < _ellipses.Count; i++)
            {
                var ellipse = _ellipses[i];
                Point pos = new Point(Canvas.GetLeft(ellipse) + 3, Canvas.GetTop(ellipse) + 3);
                double dx = clickPoint.X - pos.X;
                double dy = clickPoint.Y - pos.Y;

                if (dx * dx + dy * dy <= 36) // 6px radius hit detection
                {
                    _draggingEllipse = ellipse;
                    _draggingIndex = i;
                    _dragOffset = new Point(dx, dy);
                    _canvas.MouseMove += Polynom_MouseMove;
                    _canvas.MouseLeftButtonUp += Polynom_MouseLeftButtonUp;
                    break;
                }
            }
        }
    }

    private void Polynom_MouseMove(object sender, MouseEventArgs e)
    {
        if (_draggingEllipse == null || _draggingIndex == -1) return;

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            StopDragging();
            return;
        }

        Point mousePos = e.GetPosition(_canvas);
        double newX = mousePos.X - _dragOffset.X;
        double newY = mousePos.Y - _dragOffset.Y;

        // Maintain horizontal ordering to prevent Newton table instability
        if (_draggingIndex == 0)
        {
            if (_points.Count > 1)
                newX = Math.Max(0, Math.Min(_points[1].X - 1, newX));
        }
        else if (_draggingIndex == _points.Count - 1)
        {
            newX = Math.Max(_points[^2].X + 1, Math.Min(_canvas.ActualWidth, newX));
        }
        else
        {
            newX = Math.Max(_points[_draggingIndex - 1].X + 1, Math.Min(_points[_draggingIndex + 1].X - 1, newX));
        }

        newY = Math.Max(0, Math.Min(_canvas.ActualHeight, newY));

        _points[_draggingIndex] = new Point(newX, newY);
        Canvas.SetLeft(_draggingEllipse, newX - 3);
        Canvas.SetTop(_draggingEllipse, newY - 3);

        if (_isDrawn)
            DrawNewtonCurve();
    }

    private void Polynom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => StopDragging();

    private void StopDragging()
    {
        _draggingEllipse = null;
        _draggingIndex = -1;
        _canvas.MouseMove -= Polynom_MouseMove;
        _canvas.MouseLeftButtonUp -= Polynom_MouseLeftButtonUp;
    }

    private void Polynom_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_points.Count < 2 || _isDrawn) return;

        _isDrawn = true;
        DrawNewtonCurve();
    }

    private void DrawNewtonCurve()
    {
        if (_polyline != null)
            _canvas.Children.Remove(_polyline);

        _polyline = new Polyline
        {
            Stroke = Brushes.Blue,
            StrokeThickness = 2
        };

        // 1. Pre-calculate the Divided Difference coefficients
        CalculateDividedDifferences();

        // 2. Sample points for the polyline
        double minX = _points[0].X;
        double maxX = _points[^1].X;

        for (double u = minX; u <= maxX; u += 1)
        {
            double y = EvaluateNewton(u);
            _polyline.Points.Add(new Point(u, y));
        }

        _canvas.Children.Add(_polyline);
    }

    private void CalculateDividedDifferences()
    {
        int n = _points.Count;
        _coefficients = new double[n];
        double[,] table = new double[n, n];

        // Initialize first column with Y values
        for (int i = 0; i < n; i++)
            table[i, 0] = _points[i].Y;

        // Build the table column by column
        for (int j = 1; j < n; j++)
        {
            for (int i = 0; i < n - j; i++)
            {
                table[i, j] = (table[i + 1, j - 1] - table[i, j - 1]) / (_points[i + j].X - _points[i].X);
            }
        }

        // Coefficients are the diagonal/top row elements: f[x0], f[x0,x1], f[x0,x1,x2]...
        for (int i = 0; i < n; i++)
            _coefficients[i] = table[0, i];
    }

    private double EvaluateNewton(double x)
    {
        int n = _points.Count;
        if (n == 0) return 0;

        // Evaluate using Horner's Method for Newton Form:
        // P(x) = a0 + (x-x0)(a1 + (x-x1)(a2 + ...))
        double result = _coefficients[n - 1];
        for (int i = n - 2; i >= 0; i--)
        {
            result = _coefficients[i] + (x - _points[i].X) * result;
        }
        return result;
    }

    private Ellipse CreateEllipse(Point point)
    {
        Ellipse ellipse = new Ellipse
        {
            Width = 6,
            Height = 6,
            Fill = Brushes.Red
        };
        Canvas.SetLeft(ellipse, point.X - 3);
        Canvas.SetTop(ellipse, point.Y - 3);
        return ellipse;
    }

    public void Reset()
    {
        _points.Clear();
        _ellipses.Clear();
        _canvas.Children.Clear();
        _polyline = null;
        _isDrawn = false;
        StopDragging();
    }
}