using System;

namespace Zalando.Teaser.Map
{
    public class TileInfo : IEquatable<TileInfo>, IComparable<TileInfo>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Zoom { get; set; }
        public double Density { get; set; }

        public double Heuristic
        {
            get
            {
                int pixelX = X * 256 + 127;
                int pixelY = Y * 256 + 127;

                LatLong location = MapMath.PixelToLatLong(pixelX, pixelY, Zoom);

                double d1 = DistanceTo.River(location.Latitude, location.Longitude);
                double d2 = DistanceTo.Satellite(location.Latitude, location.Longitude);
                double d3 = DistanceTo.Brandenburg(location.Latitude, location.Longitude);

                if (d1 < 0 || d2 < 0)
                    return double.MinValue;

                return Density / (d1 + d2 + d3);
            }
        }

        public bool Equals(TileInfo other)
        {
            return X == other.X && Y == other.Y && Zoom == other.Zoom;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TileInfo);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Zoom.GetHashCode();
        }

        public int CompareTo(TileInfo other)
        {
            return Density.CompareTo(other.Density);
        }
    }
}
