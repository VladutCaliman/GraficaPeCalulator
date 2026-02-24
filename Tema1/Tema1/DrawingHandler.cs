using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tema1
{
    public class DrawingHandler
    {
        private readonly Canvas _canvas;
        private readonly List<Point> _points = new List<Point>();

        private Polyline _polyline;
        private Polygon _polygon;
        private bool _isClosed;

        public DrawingHandler(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void AddPoint(Point point)
        {
            if (_isClosed)
            {
                return;
            }

            _points.Add(point);

            if (_polyline == null)
            {
                _polyline = new Polyline
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                _canvas.Children.Add(_polyline);
            }

            _polyline.Points.Add(point);
        }

        public void Rotate(double angleDegrees, bool useOrigin)
        {
            if (_points.Count == 0)
            {
                return;
            }

            var transformed = Transformare.Rotate(_points, angleDegrees, useOrigin);
            UpdateShape(transformed);
        }

        public void Scale(double scaleX, double scaleY, bool useOrigin)
        {
            if (_points.Count == 0)
            {
                return;
            }

            var transformed = Transformare.Scale(_points, scaleX, scaleY, useOrigin);
            UpdateShape(transformed);
        }

        public void Translate(double offsetX, double offsetY, bool useOrigin)
        {
            if (_points.Count == 0)
            {
                return;
            }

            var transformed = Transformare.Translate(_points, offsetX, offsetY, useOrigin);
            UpdateShape(transformed);
        }

        public void TranslateTo(Point targetCenter)
        {
            if (_points.Count == 0)
            {
                return;
            }

            double sumX = 0;
            double sumY = 0;
            for (int i = 0; i < _points.Count; i++)
            {
                sumX += _points[i].X;
                sumY += _points[i].Y;
            }

            var currentCenter = new Point(sumX / _points.Count, sumY / _points.Count);
            var offsetX = targetCenter.X - currentCenter.X;
            var offsetY = targetCenter.Y - currentCenter.Y;

            var transformed = Transformare.Translate(_points, offsetX, offsetY, true);
            UpdateShape(transformed);
        }

        public void SymmetryOrigin(bool useOrigin)
        {
            if (_points.Count == 0)
            {
                return;
            }

            var transformed = Transformare.SymmetryOrigin(_points, useOrigin);
            UpdateShape(transformed);
        }

        public void SymmetryLine(Point a, Point b)
        {
            if (_points.Count == 0)
            {
                return;
            }

            var transformed = Transformare.SymmetryLine(_points, a, b);
            UpdateShape(transformed);
        }

        public void ClosePolygon()
        {
            if (_isClosed)
            {
                return;
            }

            if (_points.Count < 3)
            {
                return;
            }

            _isClosed = true;

            _polygon = new Polygon
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
                Opacity = 0.6
            };

            foreach (var p in _points)
            {
                _polygon.Points.Add(p);
            }

            _canvas.Children.Clear();
            _canvas.Children.Add(_polygon);
        }

        public void Reset()
        {
            _points.Clear();
            _isClosed = false;
            _polyline = null;
            _polygon = null;
            _canvas.Children.Clear();
        }

        private void UpdateShape(IList<Point> newPoints)
        {
            _points.Clear();
            foreach (var p in newPoints)
            {
                _points.Add(p);
            }

            if (_isClosed)
            {
                if (_polygon == null)
                {
                    _polygon = new Polygon
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Fill = Brushes.LightBlue,
                        Opacity = 0.6
                    };
                    _canvas.Children.Clear();
                    _canvas.Children.Add(_polygon);
                }

                _polygon.Points.Clear();
                foreach (var p in _points)
                {
                    _polygon.Points.Add(p);
                }
            }
            else
            {
                if (_polyline == null)
                {
                    _polyline = new Polyline
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    _canvas.Children.Clear();
                    _canvas.Children.Add(_polyline);
                }

                _polyline.Points.Clear();
                foreach (var p in _points)
                {
                    _polyline.Points.Add(p);
                }
            }
        }
    }
}
