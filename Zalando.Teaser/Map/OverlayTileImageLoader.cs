using MapControl;
using System.Collections.Generic;

namespace Zalando.Teaser.Map
{
    public class OverlayTileImageLoader : ITileImageLoader
    {
        private Distribution distribution = new Distribution();

        public OverlayTileImageLoader()
        {

        }

        public void BeginLoadTiles(TileLayer tileLayer, IEnumerable<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                TilePainter tilePainter = new TilePainter(tile, distribution);

                tilePainter.Paint();
            }
        }

        public void CancelLoadTiles(TileLayer tileLayer)
        {
        }
    }
}
