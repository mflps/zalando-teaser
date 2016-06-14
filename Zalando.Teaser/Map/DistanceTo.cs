using System;
using Zalando.Teaser.Geometry;

namespace Zalando.Teaser.Map
{
    public static class DistanceTo
    {
        private static Point[] riverSpree;
        private static Point[] satellite;
        private static Point   brandenburg = MapMath.LocationToPoint(52.516288, 13.377689);

        static DistanceTo()
        {
            riverSpree = new Point[]
            {
                MapMath.LocationToPoint(52.529198,13.274099),
                MapMath.LocationToPoint(52.531835,13.29234),
                MapMath.LocationToPoint(52.522116,13.298541),
                MapMath.LocationToPoint(52.520569,13.317349),
                MapMath.LocationToPoint(52.524877,13.322434),
                MapMath.LocationToPoint(52.522788,13.329),
                MapMath.LocationToPoint(52.517056,13.332075),
                MapMath.LocationToPoint(52.522514,13.340743),
                MapMath.LocationToPoint(52.517239,13.356665),
                MapMath.LocationToPoint(52.523063,13.372158),
                MapMath.LocationToPoint(52.519198,13.379453),
                MapMath.LocationToPoint(52.522462,13.392328),
                MapMath.LocationToPoint(52.520921,13.399703),
                MapMath.LocationToPoint(52.515333,13.406054),
                MapMath.LocationToPoint(52.514863,13.416354),
                MapMath.LocationToPoint(52.506034,13.435923),
                MapMath.LocationToPoint(52.496473,13.461587),
                MapMath.LocationToPoint(52.487641,13.483216),
                MapMath.LocationToPoint(52.488739,13.491456),
                MapMath.LocationToPoint(52.464011,13.503386)
            };

            satellite = new Point[]
            {
                MapMath.LocationToPoint(52.590117,13.39915),
                MapMath.LocationToPoint(52.437385,13.553989)
            };
        }

        public static double Satellite(double latitude, double longitude)
        {
            return DistanceToSatellite(MapMath.LocationToPoint(latitude, longitude));
        }

        public static double River(double latitude, double longitude)
        {
            return DistanceToRiver(MapMath.LocationToPoint(latitude, longitude));
        }

        public static double Brandenburg(double latitude, double longitude)
        {
            return DistanceToBrandenburg(MapMath.LocationToPoint(latitude, longitude));
        }

        //---------------------------------------------------------------------

        private static double DistanceToSatellite(Point pt)
        {
            LineSegment line = new LineSegment(satellite[0], satellite[1]);

            return line.DistanceTo(pt);
        }

        private static double DistanceToRiver(Point pt)
        {
            double distance = double.MaxValue;

            for (int i = 0; i < riverSpree.Length - 1; i++)
            {
                LineSegment line = new LineSegment(riverSpree[i], riverSpree[i + 1]);
                double d = line.DistanceTo(pt);

                // If d == -1 then the point has no orthogonal projection on the line.
                if (d >= 0.0)
                    distance = Math.Min(distance, d);
            }
            return distance == double.MaxValue ? -1 : distance;
        }

        private static double DistanceToBrandenburg(Point pt)
        {
            Vector v = pt - brandenburg;
            return v.Norm;
        }
    }
}
