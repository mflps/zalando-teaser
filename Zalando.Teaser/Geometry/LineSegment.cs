namespace Zalando.Teaser.Geometry
{
    public class LineSegment
    {
        private Point p1;
        private Point p2;

        public LineSegment(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public double DistanceTo( Point q )
        {
            Vector u = p2 - p1;
            Vector v = q - p1;
            double r = (u * v) / (u * u);

            if( r >= 0 && r <= 1)
            {
                Point p = p1 + r * u;
                Vector qp = p - q;

                return qp.Norm;
            }

            // The point has no projection on the line segment.
            return -1;
        }
    }
}
