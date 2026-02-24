using System.Windows;

namespace Tema1
{
    internal static class Transformare
    {
        private static readonly double[,] _transformationMatrix;
        private enum APPLY
        {
            ORIGIN,
            OCENTER,
        }
        static Transformare()
        {
            _transformationMatrix = new double[3, 3];
            SetIdentity(_transformationMatrix);
        }

        private static double[] MakePointBeVector(Point point)
            => new double[] { point.X, point.Y, 1.0f };
        private static double[] MultiplyMatrixByVector(double[,] matrix, double[] vector)
        {
            double[] result = new double[3];

            for (int i = 0; i < 3; i++)
            {
                result[i] = 0;
                for (int j = 0; j < 3; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
            }

            return result;
        }
        private static double[,] MultiplyMatrices(double[,] a, double[,] b)
        {
            var result = new double[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }

                    result[i, j] = sum;
                }
            }

            return result;
        }
        private static void SetIdentity(double[,] matrix)
        {
            matrix[0, 0] = 1; matrix[0, 1] = 0; matrix[0, 2] = 0;
            matrix[1, 0] = 0; matrix[1, 1] = 1; matrix[1, 2] = 0;
            matrix[2, 0] = 0; matrix[2, 1] = 0; matrix[2, 2] = 1;
        }
        private static Point ComputeCenter(IList<Point> points)
        {
            double sumX = 0;
            double sumY = 0;

            for (int i = 0; i < points.Count; i++)
            {
                sumX += points[i].X;
                sumY += points[i].Y;
            }

            return new Point(sumX / points.Count, sumY / points.Count);
        }

        private static List<Point> ApplyTransformation(
            IList<Point> points,
            double[,] matrix,
            APPLY applyWhere)
        {
            var transformed = new List<Point>(points.Count);
            Point center = new Point(0, 0);

            if (applyWhere == APPLY.OCENTER && points.Count > 0)
            {
                center = ComputeCenter(points);
            }

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];

                if (applyWhere == APPLY.OCENTER)
                {
                    p = new Point(p.X - center.X, p.Y - center.Y);
                }

                var vector = MakePointBeVector(p);
                var result = MultiplyMatrixByVector(matrix, vector);
                var transformedPoint = new Point(result[0], result[1]);

                if (applyWhere == APPLY.OCENTER)
                {
                    transformedPoint = new Point(
                        transformedPoint.X + center.X,
                        transformedPoint.Y + center.Y);
                }

                transformed.Add(transformedPoint);
            }

            return transformed;
        }

        public static List<Point> Rotate(IList<Point> points, double angleDegrees, bool useOrigin)
        {
            if (points == null || points.Count == 0)
            {
                return new List<Point>();
            }

            double angleRadians = angleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);

            _transformationMatrix[0, 0] = cos;
            _transformationMatrix[0, 1] = -sin;
            _transformationMatrix[0, 2] = 0;

            _transformationMatrix[1, 0] = sin;
            _transformationMatrix[1, 1] = cos;
            _transformationMatrix[1, 2] = 0;

            _transformationMatrix[2, 0] = 0;
            _transformationMatrix[2, 1] = 0;
            _transformationMatrix[2, 2] = 1;

            return ApplyTransformation(points, _transformationMatrix,
                useOrigin ? APPLY.ORIGIN : APPLY.OCENTER);
        }
        public static List<Point> Scale(IList<Point> points, double scaleX, double scaleY, bool useOrigin)
        {
            if (points == null || points.Count == 0)
            {
                return new List<Point>();
            }

            _transformationMatrix[0, 0] = scaleX;
            _transformationMatrix[0, 1] = 0;
            _transformationMatrix[0, 2] = 0;

            _transformationMatrix[1, 0] = 0;
            _transformationMatrix[1, 1] = scaleY;
            _transformationMatrix[1, 2] = 0;

            _transformationMatrix[2, 0] = 0;
            _transformationMatrix[2, 1] = 0;
            _transformationMatrix[2, 2] = 1;

            return ApplyTransformation(points, _transformationMatrix,
                useOrigin ? APPLY.ORIGIN : APPLY.OCENTER);
        }
        public static List<Point> Translate(IList<Point> points, double offsetX, double offsetY, bool useOrigin)
        {
            if (points == null || points.Count == 0)
            {
                return new List<Point>();
            }

            _transformationMatrix[0, 0] = 1;
            _transformationMatrix[0, 1] = 0;
            _transformationMatrix[0, 2] = offsetX;

            _transformationMatrix[1, 0] = 0;
            _transformationMatrix[1, 1] = 1;
            _transformationMatrix[1, 2] = offsetY;

            _transformationMatrix[2, 0] = 0;
            _transformationMatrix[2, 1] = 0;
            _transformationMatrix[2, 2] = 1;

            return ApplyTransformation(points, _transformationMatrix,
                useOrigin ? APPLY.ORIGIN : APPLY.OCENTER);
        }
        public static List<Point> SymmetryOrigin(IList<Point> points, bool useOrigin)
        {
            if (points == null || points.Count == 0)
            {
                return new List<Point>();
            }

            _transformationMatrix[0, 0] = -1;
            _transformationMatrix[0, 1] = 0;
            _transformationMatrix[0, 2] = 0;

            _transformationMatrix[1, 0] = 0;
            _transformationMatrix[1, 1] = -1;
            _transformationMatrix[1, 2] = 0;

            _transformationMatrix[2, 0] = 0;
            _transformationMatrix[2, 1] = 0;
            _transformationMatrix[2, 2] = 1;

            return ApplyTransformation(points, _transformationMatrix,
                useOrigin ? APPLY.ORIGIN : APPLY.OCENTER);
        }
        public static List<Point> SymmetryLine(IList<Point> points, Point a, Point b)
        {
            if (points == null || points.Count == 0)
            {
                return new List<Point>();
            }

            if (a == b)
            {
                return new List<Point>(points);
            }

            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            double angle = Math.Atan2(dy, dx);
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            double[,] t1 =
            {
                { 1, 0, -a.X },
                { 0, 1, -a.Y },
                { 0, 0, 1 }
            };

            double[,] r1 =
            {
                { cos,  sin, 0 },
                { -sin, cos, 0 },
                { 0,   0,   1 }
            };

            double[,] s =
            {
                { 1,  0, 0 },
                { 0, -1, 0 },
                { 0,  0, 1 }
            };

            double[,] r2 =
            {
                { cos, -sin, 0 },
                { sin,  cos, 0 },
                { 0,   0,   1 }
            };

            double[,] t2 =
            {
                { 1, 0, a.X },
                { 0, 1, a.Y },
                { 0, 0, 1 }
            };

            var m = MultiplyMatrices(t2,
                      MultiplyMatrices(r2,
                      MultiplyMatrices(s,
                      MultiplyMatrices(r1, t1))));

            return ApplyTransformation(points, m, APPLY.ORIGIN);
        }
    }
}
