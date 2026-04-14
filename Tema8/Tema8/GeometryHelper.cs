using System.Windows;

namespace Tema8
{
    public static class GeometryHelper
    {
        public static double F(Point A, Point B, Point C)
        {
            return (B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X);
        }

        public static int Sgn(Point A, Point B, Point C)
        {
            double val = F(A, B, C);
            if (Math.Abs(val) < 1e-7) return 0;
            return val > 0 ? 1 : -1;
        }

        public static bool PunctInteriorTriunghi(Point A, Point B, Point C, Point D)
        {
            int s1 = Sgn(A, B, C);
            int s2 = Sgn(A, C, D);
            int s3 = Sgn(A, D, B);
            return (s1 == s2) && (s2 == s3) && (s1 != 0);
        }

        public static bool SemiplaneDiferite(Point A, Point B, Point C, Point D)
        {
            return Sgn(A, C, D) * Sgn(B, C, D) < 0;
        }

        public static bool PatrulaterConvex(Point A, Point B, Point C, Point D)
        {
            int s1 = Sgn(A, B, C);
            int s2 = Sgn(B, C, D);
            int s3 = Sgn(C, D, A);
            int s4 = Sgn(D, A, B);
            return (s1 == s2) && (s2 == s3) && (s3 == s4) && (s1 != 0);
        }

        public static bool IntersectieSegmente(Point A, Point B, Point C, Point D)
        {
            return SemiplaneDiferite(A, B, C, D) && SemiplaneDiferite(C, D, A, B);
        }

        public static bool PoligonConvex(List<Point> varfuri)
        {
            if (varfuri.Count < 3) return false;
            int n = varfuri.Count;
            int sign = 0;
            for (int i = 0; i < n; i++)
            {
                int s = Sgn(varfuri[i], varfuri[(i + 1) % n], varfuri[(i + 2) % n]);
                if (s != 0)
                {
                    if (sign == 0) sign = s;
                    else if (sign != s) return false;
                }
            }
            return true;
        }

        public static bool PunctInteriorPoligon(Point A, List<Point> varfuri)
        {
            bool isInside = false;
            int n = varfuri.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((varfuri[i].Y > A.Y) != (varfuri[j].Y > A.Y)) &&
                    (A.X < (varfuri[j].X - varfuri[i].X) * (A.Y - varfuri[i].Y) / (varfuri[j].Y - varfuri[i].Y) + varfuri[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
    }
}