namespace Zalando.Teaser.Geometry
{
    public class Point
    {
        public Point() { }

        public Point( double x, double y )
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public static Vector operator-(Point p2, Point p1)
        {
            return new Vector(p2.X - p1.X, p2.Y - p1.Y );
        }

        public static Point operator+(Point p, Vector v)
        {
            return new Point(p.X + v.X, p.Y + v.Y );
        }

        public static Point operator+(Vector v, Point p)
        {
            return new Point(p.X + v.X, p.Y + v.Y );
        }
    }
}
