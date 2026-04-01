using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tema6
{
    public class BezierManager
    {
        private readonly Canvas _canvas;
        private readonly List<Point> _controlPoints = new List<Point>();
        private readonly List<Ellipse> _ellipses = new List<Ellipse>();
        private readonly Polyline _bezierCurve;
        private readonly Polyline _controlPolygon;
        private bool _isFinalized = false;

        private Ellipse _draggingEllipse = null;
        private int _draggingIndex = -1;

        public BezierManager(Canvas canvas)
        {
            _canvas = canvas;
            _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            _canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
            _canvas.MouseMove += Canvas_MouseMove;
            _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            _controlPolygon = new Polyline { Stroke = Brushes.LightGray, StrokeThickness = 1, StrokeDashArray = new DoubleCollection { 4, 2 } };
            _bezierCurve = new Polyline { Stroke = Brushes.Blue, StrokeThickness = 2 };

            _canvas.Children.Add(_controlPolygon);
            _canvas.Children.Add(_bezierCurve);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(_canvas);

            if (!_isFinalized)
            {
                AddPoint(mousePos);
            }
            else
            {
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
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_controlPoints.Count > 1)
            {
                _isFinalized = true;
                DrawBezier();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingEllipse != null && _draggingIndex != -1)
            {
                Point mousePos = e.GetPosition(_canvas);
                _controlPoints[_draggingIndex] = mousePos;
                Canvas.SetLeft(_draggingEllipse, mousePos.X - 4);
                Canvas.SetTop(_draggingEllipse, mousePos.Y - 4);
                DrawBezier();
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
            _controlPoints.Add(p);
            Ellipse el = new Ellipse { Width = 8, Height = 8, Fill = Brushes.Red, Cursor = Cursors.Hand };
            Canvas.SetLeft(el, p.X - 4);
            Canvas.SetTop(el, p.Y - 4);
            _ellipses.Add(el);
            _canvas.Children.Add(el);

            _controlPolygon.Points.Add(p);
        }

        private double Combination(int n, int k)
        {
            if (k < 0 || k > n) return 0;
            if (k == 0 || k == n) return 1;
            if (k > n / 2) k = n - k;

            double res = 1;
            for (int i = 1; i <= k; i++)
                res = res * (n - i + 1) / i;
            return res;
        }

        private double Bernstein(int n, int i, double u)
        {
            return Combination(n, i) * Math.Pow(u, i) * Math.Pow(1 - u, n - i);
        }

        private void DrawBezier()
        {
            _bezierCurve.Points.Clear();
            _controlPolygon.Points.Clear();
            foreach (var p in _controlPoints) _controlPolygon.Points.Add(p);

            int n = _controlPoints.Count - 1;
            if (n < 1) return;

            for (double u = 0; u <= 1.001; u += 0.01)
            {
                double x = 0;
                double y = 0;

                for (int i = 0; i <= n; i++)
                {
                    double b = Bernstein(n, i, u);
                    x += _controlPoints[i].X * b;
                    y += _controlPoints[i].Y * b;
                }
                _bezierCurve.Points.Add(new Point(x, y));
            }
        }

        public void Reset()
        {
            _controlPoints.Clear();
            _ellipses.Clear();
            _isFinalized = false;
            _canvas.Children.Clear();
            _bezierCurve.Points.Clear();
            _controlPolygon.Points.Clear();
            _canvas.Children.Add(_controlPolygon);
            _canvas.Children.Add(_bezierCurve);
        }
    }
}
