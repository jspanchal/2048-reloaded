using System.Collections.Generic;
using System.Text;
using Windows.Foundation;

namespace Brain.Extensions
{
    public static class PointExtensions
    {
        public static Point IncrementBy(this Point point, Point value)
        {
            point.X += value.X;
            point.Y += value.Y;
            return point;
        }

        public static Point DecrementBy(this Point point, Point value)
        {
            point.X -= value.X;
            point.Y -= value.Y;
            return point;
        }


        public static Point Multiply(this Point point, double value)
        {
            return new Point(point.X * value, point.Y * value);
        }


        public static string ConvertPointsToGeometryString(List<Point> points, double easing = 150, bool leftToRight = true)
        {
            if ((points == null) || (points.Count < 2))
                return null;

            Point lastPoint;
            var sb = new StringBuilder();
            foreach (var point in points)
            {
                if (sb.Length == 0)
                {
                    lastPoint = point;
                    sb.AppendFormat("M{0},{1} ", point.X, point.Y);
                }
                else if (leftToRight)
                {
                    sb.AppendFormat("C{0},{1} ", lastPoint.X + easing, lastPoint.Y);
                    sb.AppendFormat("{0},{1} ", point.X - easing, point.Y);
                    sb.AppendFormat("{0},{1} ", point.X, point.Y);
                    lastPoint = point;
                }
                else
                {
                    sb.AppendFormat("C{0},{1} ", lastPoint.X - easing, lastPoint.Y);
                    sb.AppendFormat("{0},{1} ", point.X + easing, point.Y);
                    sb.AppendFormat("{0},{1} ", point.X, point.Y);
                    lastPoint = point;
                }
            }
            return sb.ToString();
        }


        public static Point Add(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }

        public static Point Add(this Point point, Size size)
        {
            return new Point(point.X + size.Width, point.Y + size.Height);
        }

        public static Point AddHalf(this Point point, Size size)
        {
            return new Point(point.X + (size.Width / 2), point.Y + (size.Height / 2));
        }

        public static Point Subtract(this Point point, Point other)
        {
            return new Point(point.X - other.X, point.Y - other.Y);
        }

        public static Point Subtract(this Point point, Size size)
        {
            return new Point(point.X - size.Width, point.Y - size.Height);
        }

        public static Point SubtractHalf(this Point point, Size size)
        {
            return new Point(point.X - (size.Width / 2), point.Y - (size.Height / 2));
        }


    }
}
