using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

public class HermiteCurve
{
    private readonly Canvas _canvas;
    private readonly List<Point> _points = new List<Point>();
    private readonly List<Ellipse> _ellipses = new List<Ellipse>();
    private readonly List<Line> _tangentVisuals = new List<Line>();
    private Polyline _curvePolyline;

    private Ellipse _draggingEllipse = null;
    private int _draggingIndex = -1;

    public HermiteCurve(Canvas canvas)
    {
        _canvas = canvas;
        _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        _canvas.MouseMove += Canvas_MouseMove;
        _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

        _curvePolyline = new Polyline { Stroke = Brushes.Red, StrokeThickness = 2 };
        _canvas.Children.Add(_curvePolyline);
    }

    private double F1(double u) => 2 * Math.Pow(u, 3) - 3 * Math.Pow(u, 2) + 1;
    private double F2(double u) => -2 * Math.Pow(u, 3) + 3 * Math.Pow(u, 2);
    private double F3(double u) => Math.Pow(u, 3) - 2 * Math.Pow(u, 2) + u;
    private double F4(double u) => Math.Pow(u, 3) - Math.Pow(u, 2);

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Point mousePos = e.GetPosition(_canvas);
        _draggingIndex = -1;

        for (int i = 0; i < _ellipses.Count; i++)
        {
            if (_ellipses[i].InputHitTest(e.GetPosition(_ellipses[i])) != null)
            {
                _draggingEllipse = _ellipses[i];
                _draggingIndex = i;
                _draggingEllipse.CaptureMouse();
                return;
            }
        }

        AddPoint(mousePos);
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (_draggingEllipse != null && _draggingIndex != -1)
        {
            Point mousePos = e.GetPosition(_canvas);
            _points[_draggingIndex] = mousePos;

            Canvas.SetLeft(_draggingEllipse, mousePos.X - 4);
            Canvas.SetTop(_draggingEllipse, mousePos.Y - 4);

            Redraw();
        }
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_draggingEllipse != null)
        {
            _draggingEllipse.ReleaseMouseCapture();
            _draggingEllipse = null;
            _draggingIndex = -1;
        }
    }

    private void AddPoint(Point p)
    {
        _points.Add(p);
        int index = _points.Count - 1;

        Ellipse el = new Ellipse
        {
            Width = 8,
            Height = 8,
            Fill = (index % 2 == 0) ? Brushes.Blue : Brushes.Green,
            Cursor = Cursors.Hand,
            IsHitTestVisible = true
        };

        Canvas.SetLeft(el, p.X - 4);
        Canvas.SetTop(el, p.Y - 4);

        _ellipses.Add(el);
        _canvas.Children.Add(el);

        Redraw();
    }

    private void Redraw()
    {
        _curvePolyline.Points.Clear();
        foreach (var line in _tangentVisuals) _canvas.Children.Remove(line);
        _tangentVisuals.Clear();

        int n = _points.Count;

        for (int i = 0; i < n - 1; i += 2)
        {
            Line tLine = new Line
            {
                X1 = _points[i].X,
                Y1 = _points[i].Y,
                X2 = _points[i + 1].X,
                Y2 = _points[i + 1].Y,
                Stroke = Brushes.LightGray,
                StrokeDashArray = new DoubleCollection { 2, 1 }
            };
            _tangentVisuals.Add(tLine);
            _canvas.Children.Add(tLine);
        }

        if (n >= 4)
        {
            for (int i = 0; i <= n - 4; i += 2)
            {
                Point Pi = _points[i];
                Point Pnext = _points[i + 2];

                Vector a = _points[i + 1] - Pi;
                Vector b = _points[i + 3] - Pnext;

                for (double u = 0; u <= 1.001; u += 0.05)
                {
                    double x = F1(u) * Pi.X + F2(u) * Pnext.X + F3(u) * a.X + F4(u) * b.X;
                    double y = F1(u) * Pi.Y + F2(u) * Pnext.Y + F3(u) * a.Y + F4(u) * b.Y;
                    _curvePolyline.Points.Add(new Point(x, y));
                }
            }
        }
    }

    public void Reset()
    {
        _points.Clear();
        _ellipses.Clear();
        _canvas.Children.Clear();
        _curvePolyline.Points.Clear();
        _canvas.Children.Add(_curvePolyline);
        _tangentVisuals.Clear();
    }
}
