using System;

namespace Zalando.Teaser.Geometry
{
    public class Vector
    {
        public Vector() { }

        public Vector( double x, double y )
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public static double operator*(Vector u, Vector v)
        {
            return u.X * v.X + u.Y * v.Y;
        }

        public static Vector operator*(Vector v, double r)
        {
            return new Vector(r * v.X, r * v.Y);
        }

        public static Vector operator *(double r, Vector v)
        {
            return new Vector(r * v.X, r * v.Y);
        }

        public static Vector operator+(Vector u, Vector v)
        {
            return new Vector(u.X + v.X, u.Y + v.Y );
        }

        public double Norm
        {
            get { return Math.Sqrt(X * X + Y * Y); }
        }
    }
}
