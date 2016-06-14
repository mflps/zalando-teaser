using MapControl;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Zalando.Teaser.Map
{
    public class TilePainter
    {
        private PropertyInfo dpiXProperty;
        private PropertyInfo dpiYProperty;
        private Distribution distribution;

        private Tile tile;

        public TilePainter( Tile tile, Distribution distribution )
        {
            dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            this.tile = tile;
            this.distribution = distribution;
        }

        public async void Paint()
        {
            int dpiX = (int)dpiXProperty.GetValue(null, null);
            int dpiY = (int)dpiYProperty.GetValue(null, null);

            byte[] buffer = await Task.Run( () => BackgroundPaint() );

            WriteableBitmap bitmap = new WriteableBitmap(256, 256, dpiX, dpiY, PixelFormats.Bgra32, null);
            Int32Rect rect = new Int32Rect(0, 0, 256, 256);

            bitmap.WritePixels(rect, buffer, 256 * 4, 0);

            tile.SetImage(bitmap);
        }

        //

        private byte[] BackgroundPaint()
        {
            byte[] buffer = new byte[256 * 256 * 4];

            Parallel.For(0, 256, y =>
            {
                Parallel.For(0, 256, x =>
                {
                    // Convert the tile's (x,y) coordinates to proper (lat,lon) coordinates and feed them to the distribution function.
                    int pixelX = tile.X * 256 + x;
                    int pixelY = tile.Y * 256 + y;
                    LatLong location = MapMath.PixelToLatLong(pixelX, pixelY, tile.ZoomLevel);

                    int b = x * 4 + y * 256 * 4;
                    int g = b + 1;
                    int r = b + 2;
                    int a = b + 3;

                    byte colorG = (byte)(distribution.BrandenburgDensity(location.Latitude, location.Longitude) / distribution.MaxBrandenburg * 255);
                    byte colorB = (byte)(distribution.RiverDensity(location.Latitude, location.Longitude) / distribution.MaxRiver * 255);
                    byte colorR = (byte)(distribution.SatelliteDensity(location.Latitude, location.Longitude) / distribution.MaxSatellite * 255);

                    buffer[b] = colorB;
                    buffer[g] = colorG;
                    buffer[r] = colorR;
                    buffer[a] = 128;
                });
            });
            return buffer;
        }
    }
}
