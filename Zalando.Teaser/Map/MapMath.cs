using System;
using Zalando.Teaser.Geometry;

namespace Zalando.Teaser.Map
{
    /// <summary>
    /// Map math based on the code available here: https://msdn.microsoft.com/en-us/library/bb259689.aspx
    /// </summary>
    public static class MapMath
    {
        private const double southWestLon = 13.274099;
        private const double southWestLat = 52.464011;

        public static Point LocationToPoint( double latitude, double longitude )
        {
            Point pt = new Point();

            pt.X = (longitude - southWestLon) * Math.Cos(southWestLat * Math.PI / 180.0) * 111.323;
            pt.Y = (latitude - southWestLat) * 111.323;
            return pt;
        }

        public static LatLong PixelToLatLong(int pixelX, int pixelY, int zoom)
        {
            LatLong latLong = new LatLong();
            double mapSize = ((uint)256 << zoom);

            double x = (Clip(pixelX, 0, mapSize - 1) / mapSize) - 0.5;
            double y = 0.5 - (Clip(pixelY, 0, mapSize - 1) / mapSize);

            latLong.Latitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
            latLong.Longitude = 360 * x;

            return latLong;
        }

        public static Point LatLongToPixel( LatLong location, int zoom )
        {
            double x = (location.Longitude + 180) / 360;
            double sinLatitude = Math.Sin(location.Latitude * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);
            double mapSize = ((uint)256 << zoom);

            return new Point(x * mapSize, y * mapSize);
        }

        public static Point LatLongToPixel(double latitude, double longitude, int zoom)
        {
            double x = (longitude + 180) / 360;
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);
            double mapSize = ((uint)256 << zoom);

            return new Point(x * mapSize, y * mapSize);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }
    }
}
