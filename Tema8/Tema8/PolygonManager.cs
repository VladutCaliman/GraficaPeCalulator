using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tema8
{
    public class PolygonManager
    {
        private readonly Canvas _canvas;
        private List<Point> _vertices = new List<Point>();
        private readonly Polyline _polygonOutline;

        private bool _isFinalized = false;
        private Image _pixelLayer;

        private bool _isDragging = false;
        private Point _lastMousePos;

        public PolygonManager(Canvas canvas)
        {
            _canvas = canvas;
            _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            _canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
            _canvas.MouseMove += Canvas_MouseMove;
            _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            _polygonOutline = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            _pixelLayer = new Image();
            _canvas.Children.Add(_pixelLayer);
            _canvas.Children.Add(_polygonOutline);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(_canvas);

            if (!_isFinalized)
            {
                _vertices.Add(mousePos);
                _polygonOutline.Points.Add(mousePos);

                Ellipse el = new Ellipse { Width = 6, Height = 6, Fill = Brushes.Red };
                Canvas.SetLeft(el, mousePos.X - 3);
                Canvas.SetTop(el, mousePos.Y - 3);
                _canvas.Children.Add(el);
            }
            else
            {
                if (GeometryHelper.PunctInteriorPoligon(mousePos, _vertices))
                {
                    _isDragging = true;
                    _lastMousePos = mousePos;
                    _canvas.CaptureMouse();
                }
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isFinalized && _vertices.Count >= 3)
            {
                _isFinalized = true;
                _polygonOutline.Points.Add(_vertices[0]);

                FillPolygon();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _isFinalized)
            {
                Point currentPos = e.GetPosition(_canvas);
                double dx = currentPos.X - _lastMousePos.X;
                double dy = currentPos.Y - _lastMousePos.Y;

                for (int i = 0; i < _vertices.Count; i++)
                {
                    _vertices[i] = new Point(_vertices[i].X + dx, _vertices[i].Y + dy);
                }

                _polygonOutline.Points.Clear();
                foreach (var p in _vertices) _polygonOutline.Points.Add(p);
                _polygonOutline.Points.Add(_vertices[0]);

                var ellipses = _canvas.Children.OfType<Ellipse>().ToList();
                for (int i = 0; i < ellipses.Count; i++)
                {
                    Canvas.SetLeft(ellipses[i], _vertices[i].X - 3);
                    Canvas.SetTop(ellipses[i], _vertices[i].Y - 3);
                }

                _lastMousePos = currentPos;
                FillPolygon();
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _canvas.ReleaseMouseCapture();
            }
        }

        private void FillPolygon()
        {
            int w = (int)_canvas.ActualWidth;
            int h = (int)_canvas.ActualHeight;

            if (w == 0 || h == 0) return;

            WriteableBitmap wb = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgra32, null);
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];

            int minX = Math.Max(0, (int)_vertices.Min(p => p.X));
            int maxX = Math.Min(w - 1, (int)_vertices.Max(p => p.X));
            int minY = Math.Max(0, (int)_vertices.Min(p => p.Y));
            int maxY = Math.Min(h - 1, (int)_vertices.Max(p => p.Y));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Point pixelPoint = new Point(x, y);

                    if (GeometryHelper.PunctInteriorPoligon(pixelPoint, _vertices))
                    {
                        int idx = y * stride + x * 4;
                        pixels[idx] = 0;
                        pixels[idx + 1] = 255;
                        pixels[idx + 2] = 0;
                        pixels[idx + 3] = 150;
                    }
                }
            }

            wb.WritePixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            _pixelLayer.Source = wb;
        }

        public void Reset()
        {
            _vertices.Clear();
            _polygonOutline.Points.Clear();
            _pixelLayer.Source = null;
            _isFinalized = false;
            _isDragging = false;

            var elementsToRemove = _canvas.Children.OfType<Ellipse>().ToList();
            foreach (var el in elementsToRemove) _canvas.Children.Remove(el);
        }
    }
}