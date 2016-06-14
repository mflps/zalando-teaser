using MapControl;

namespace Zalando.Teaser.Map
{
    public class DensityTileLayer : TileLayer
    {
        public DensityTileLayer() : base( new OverlayTileImageLoader() )
        {
            MinZoomLevel = 1;
            MaxZoomLevel = 22;
            Background = null;
            Foreground = null;
            TileSource = new DensityTileSource();
        }
    }
}
