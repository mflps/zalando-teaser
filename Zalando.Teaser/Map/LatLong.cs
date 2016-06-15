namespace Zalando.Teaser.Map
{
    public class LatLong
    {
        public LatLong() { }
        public LatLong(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1}", Latitude, Longitude);
        }
    }
}
