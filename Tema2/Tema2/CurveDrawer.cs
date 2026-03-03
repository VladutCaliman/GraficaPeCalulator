using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tema2
{
    public static class CurveDrawer
    {
        public static void DrawAxes(Canvas canvas)
        {
            canvas.Children.Clear();

            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            double centerX = width / 2;
            double centerY = height / 2;

            Line xAxis = new Line
            {
                X1 = 0,
                Y1 = centerY,
                X2 = width,
                Y2 = centerY,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Line yAxis = new Line
            {
                X1 = centerX,
                Y1 = 0,
                X2 = centerX,
                Y2 = height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Ellipse origin = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(origin, centerX - 4);
            Canvas.SetTop(origin, centerY - 4);

            canvas.Children.Add(xAxis);
            canvas.Children.Add(yAxis);
            canvas.Children.Add(origin);
        }
        public static void DrawEllipse(Canvas canvas)
        {
            DrawAxes(canvas);

            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            double centerX = width / 2;
            double centerY = height / 2;

            double scale = 100;

            Polyline ellipse = new Polyline
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };

            for (double u = 0; u <= 2 * Math.PI; u += 0.01)
            {
                double x = Math.Cos(u);
                double y = 2 * Math.Sin(u);

                double screenX = centerX + x * scale;
                double screenY = centerY - y * scale;

                ellipse.Points.Add(new Point(screenX, screenY));
            }

            canvas.Children.Add(ellipse);
        }
        public static void DrawSpiral(Canvas canvas)
        {
            DrawAxes(canvas); // to clean old drawing

            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            double centerX = width / 2;
            double centerY = height / 2;

            double scale = 10;

            Polyline spiral = new Polyline
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };

            for (double u = 0; u <= 20; u += 0.01)
            {
                double x = u * Math.Cos(u);
                double y = u * Math.Sin(u);

                double screenX = centerX + x * scale;
                double screenY = centerY - y * scale;

                spiral.Points.Add(new Point(screenX, screenY));
            }

            canvas.Children.Add(spiral);
        }
        public static void DrawParabola(Canvas canvas)
        {
            DrawAxes(canvas); // to clean old drawing

            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            double centerX = width / 2;
            double centerY = height / 2;

            double scale = 80;

            Polyline parabola = new Polyline
            {
                Stroke = Brushes.Purple,
                StrokeThickness = 2
            };

            for (double u = -2; u <= 2; u += 0.01)
            {
                double x = u;
                double y = u * u + 1;

                double screenX = centerX + x * scale;
                double screenY = centerY - y * scale;

                parabola.Points.Add(new Point(screenX, screenY));
            }

            canvas.Children.Add(parabola);
        }
    }
}