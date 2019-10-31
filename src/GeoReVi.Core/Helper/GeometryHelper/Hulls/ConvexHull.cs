using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace GeoReVi
{
    public static class ConvexHull
    {

        private static int Orientation(LocationTimeValue p1, LocationTimeValue p2, LocationTimeValue p)
        {
            // Determinant
            double Orin = (p2.X - p1.X) * (p.Y - p1.Y) - (p.X - p1.X) * (p2.Y - p1.Y);

            if (Orin > 0)
                return -1; //          (* Orientation is to the left-hand side  *)
            if (Orin < 0)
                return 1; // (* Orientation is to the right-hand side *)

            return 0; //  (* Orientation is neutral aka collinear  *)
        }

        /// <summary>
        /// Computing the 2D convex hull of a set of points
        /// </summary>
        /// <param name="P"></param>
        /// <returns></returns>
        public static IEnumerable<LocationTimeValue> ComputeConvexHull2D(List<LocationTimeValue> points)
        {
            if (points.Count() < 3)
            {
                throw new ArgumentException("At least 3 points required", "points");
            }

            points = points.GroupBy(x => x.X).Select(x => x.First()).Distinct().ToList();
            points = points.GroupBy(x => x.Y).Select(x => x.First()).Distinct().ToList();

            List<LocationTimeValue> hull = new List<LocationTimeValue>();

            // get leftmost point
            LocationTimeValue vPointOnHull = points.Where(p => p.X == points.Min(min => min.X)).First();

            LocationTimeValue vEndpoint;
            do
            {
                hull.Add(vPointOnHull);
                vEndpoint = points[0];

                for (int i = 1; i < points.Count; i++)
                {
                    if ((vPointOnHull == vEndpoint)
                        || (Orientation(vPointOnHull, vEndpoint, points[i]) == -1))
                    {
                        vEndpoint = points[i];
                    }
                }

                if (hull.Count() > points.Count())
                    return new List<LocationTimeValue>();

                vPointOnHull = vEndpoint;

            }
            while (vEndpoint != hull[0]);

            return hull;
        }

        /// <summary>
        /// Checks if a polygon contains a points
        /// </summary>
        /// <param name="polygon2D"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool ContainsXY(this IEnumerable<LocationTimeValue> polygon2D, LocationTimeValue point)
        {
            Point p1, p2;
            Point p =new Point(point.X, point.Y);

            Point[] poly = polygon2D.Select(x => new Point(x.X, x.Y)).ToArray();

            bool inside = false;

            if (poly.Length < 3)
            {
                return inside;
            }

            Point oldPoint = new Point(poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

            for (int i = 0; i < poly.Length; i++)
            {
                Point newPoint = new Point(poly[i].X, poly[i].Y);
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }

                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X) <= ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;

            }

            return inside;
        }
    }
}
