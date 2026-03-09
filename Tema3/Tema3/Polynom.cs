using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tema3
{
    public class Polynom
    {
        private readonly Canvas _canvas;
        private readonly List<Point> _points = new List<Point>();
        private readonly List<Ellipse> _ellipses = new List<Ellipse>();
        private Polyline? _polyline;
        private bool _isDrawn = false;

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
                if (_points.Count > 0 && clickPoint.X <= _points[_points.Count - 1].X)
                    return;

                _points.Add(clickPoint);
                Ellipse ellipse = Polynom_CreateEllipse(clickPoint);
                _ellipses.Add(ellipse);
                _canvas.Children.Add(ellipse);
            }
            else
            {
                for (int i = 0; i < _ellipses.Count; i++)
                {
                    var ellipse = _ellipses[i];
                    Point pos = new Point(Canvas.GetLeft(ellipse) + 3, Canvas.GetTop(ellipse) + 3);
                    double dx = clickPoint.X - pos.X;
                    double dy = clickPoint.Y - pos.Y;

                    if (dx * dx + dy * dy <= 36)
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
            if (_draggingEllipse == null || _draggingIndex == -1)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                Polynom_StopDragging();
                return;
            }

            Point mousePos = e.GetPosition(_canvas);
            double newX = mousePos.X - _dragOffset.X;
            double newY = mousePos.Y - _dragOffset.Y;

            if (_draggingIndex == 0)
                newX = Math.Max(0, Math.Min(_points[1].X - 1, newX));
            else if (_draggingIndex == _points.Count - 1)
                newX = Math.Max(_points[_points.Count - 2].X + 1, Math.Min(_canvas.ActualWidth, newX));
            else
                newX = Math.Max(_points[_draggingIndex - 1].X + 1, Math.Min(_points[_draggingIndex + 1].X - 1, newX));

            newY = Math.Max(0, Math.Min(_canvas.ActualHeight, newY));

            _points[_draggingIndex] = new Point(newX, newY);
            Canvas.SetLeft(_draggingEllipse, newX - 3);
            Canvas.SetTop(_draggingEllipse, newY - 3);

            if (_isDrawn)
                Polynom_DrawLagrangeCurve();
        }

        private void Polynom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Polynom_StopDragging();
        }

        private void Polynom_StopDragging()
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
            Polynom_DrawLagrangeCurve();
        }

        private void Polynom_DrawLagrangeCurve()
        {
            if (_polyline != null)
                _canvas.Children.Remove(_polyline);

            _polyline = new Polyline
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };

            double minX = _points[0].X;
            double maxX = _points[_points.Count - 1].X;

            for (double x = minX; x <= maxX; x += 1)
            {
                double y = Polynom_LagrangeY(x);
                _polyline.Points.Add(new Point(x, y));
            }

            _canvas.Children.Add(_polyline);
        }
        private double Polynom_LagrangeY(double x)
        {
            double result = 0;
            int n = _points.Count;

            for (int i = 0; i < n; i++)
            {
                double term = _points[i].Y;
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                        term *= (x - _points[j].X) / (_points[i].X - _points[j].X);
                }
                result += term;
            }
            return result;
        }

        private Ellipse Polynom_CreateEllipse(Point point)
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
            if (_polyline != null)
            {
                _canvas.Children.Remove(_polyline);
                _polyline = null;
            }
            _canvas.Children.Clear();
            _isDrawn = false;
            Polynom_StopDragging();
        }
    }
}