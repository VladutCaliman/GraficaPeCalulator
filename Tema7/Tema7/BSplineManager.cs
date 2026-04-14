using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tema7
{
    public class BSplineManager
    {
        private readonly Canvas _canvas;
        private readonly List<Point> _controlPoints = new List<Point>();
        private readonly List<Ellipse> _ellipses = new List<Ellipse>();
        private readonly List<Polyline> _bSplineCurves = new List<Polyline>();
        private readonly Polyline _controlPolygon;
        private bool _isFinalized = false;

        private Ellipse _draggingEllipse = null;
        private int _draggingIndex = -1;

        public BSplineManager(Canvas canvas)
        {
            _canvas = canvas;
            _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            _canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
            _canvas.MouseMove += Canvas_MouseMove;
            _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            _controlPolygon = new Polyline { Stroke = Brushes.LightGray, StrokeThickness = 1, StrokeDashArray = new DoubleCollection { 4, 2 } };
            _canvas.Children.Add(_controlPolygon);
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
            if (_controlPoints.Count > 2)
            {
                _isFinalized = true;
                DrawBSplines();
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

                DrawBSplines();
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
        private double N(int i, int k, double[] knots, double t)
        {
            if (k == 1)
            {
                return (knots[i] <= t && t < knots[i + 1]) ? 1.0 : 0.0;
            }

            double d1 = knots[i + k - 1] - knots[i];
            double d2 = knots[i + k] - knots[i + 1];

            double term1 = 0.0;
            if (d1 > 0)
            {
                term1 = ((t - knots[i]) / d1) * N(i, k - 1, knots, t);
            }

            double term2 = 0.0;
            if (d2 > 0)
            {
                term2 = ((knots[i + k] - t) / d2) * N(i + 1, k - 1, knots, t);
            }

            return term1 + term2;
        }

        private void DrawBSplines()
        {
            foreach (var curve in _bSplineCurves)
            {
                _canvas.Children.Remove(curve);
            }
            _bSplineCurves.Clear();

            _controlPolygon.Points.Clear();
            foreach (var p in _controlPoints) _controlPolygon.Points.Add(p);

            int N_pts = _controlPoints.Count;
            int n = N_pts - 1;

            if (n < 2) return;

            Brush[] colors = { Brushes.Orange, Brushes.Green, Brushes.Black, Brushes.Brown, Brushes.Blue, Brushes.Purple };
            int colorIndex = 0;

            for (int k = 1; k <= n - 1; k++)
            {
                int order = k + 1;

                Polyline spline = new Polyline
                {
                    Stroke = colors[colorIndex % colors.Length],
                    StrokeThickness = 2
                };

                int numKnots = n + order + 1;
                double[] knots = new double[numKnots];
                for (int i = 0; i < numKnots; i++)
                {
                    knots[i] = i;
                }

                double tMin = knots[order - 1];
                double tMax = knots[n + 1];

                double step = (tMax - tMin) / 200.0;

                for (double t = tMin; t <= tMax; t += step)
                {
                    double x = 0;
                    double y = 0;

                    double evalT = (t >= tMax) ? tMax - 0.000001 : t;

                    for (int i = 0; i <= n; i++)
                    {
                        double basis = N(i, order, knots, evalT);
                        x += _controlPoints[i].X * basis;
                        y += _controlPoints[i].Y * basis;
                    }
                    spline.Points.Add(new Point(x, y));
                }

                _bSplineCurves.Add(spline);
                _canvas.Children.Add(spline);
                colorIndex++;
            }

            _canvas.Children.Remove(_controlPolygon);
            _canvas.Children.Add(_controlPolygon);
            foreach (var el in _ellipses)
            {
                _canvas.Children.Remove(el);
                _canvas.Children.Add(el);
            }
        }

        public void Reset()
        {
            _controlPoints.Clear();
            _ellipses.Clear();
            _isFinalized = false;
            _canvas.Children.Clear();
            _bSplineCurves.Clear();
            _controlPolygon.Points.Clear();
            _canvas.Children.Add(_controlPolygon);
        }
    }
}