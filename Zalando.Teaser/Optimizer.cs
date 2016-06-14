using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Zalando.Teaser.Geometry;
using Zalando.Teaser.Map;
using Zalando.Teaser.ViewModel;

namespace Zalando.Teaser
{
    public class Optimizer
    {
        private SortedList<double, TileInfo> sortedTiles = new SortedList<double, TileInfo>(new DescendingDoubleComparer());
        private Distribution distribution = new Distribution();
        private BackgroundWorker worker;
        private MainViewModel viewModel;

        private const int maxIterations = 1000000;

        public Optimizer( MainViewModel viewModel )
        {
            this.viewModel = viewModel;
        }

        public void Start(double latitude, double longitude, int bufferTiles = 2)
        {
            Point pixel = MapMath.LatLongToPixel(latitude, longitude, 12);
            int tileX = (int)Math.Round(pixel.X) / 256;
            int tileY = (int)Math.Round(pixel.Y) / 256;

            for( int xTile = tileX - bufferTiles; xTile < tileX + bufferTiles + 1; xTile++ )
            {
                for( int yTile = tileY - bufferTiles; yTile < tileY + bufferTiles + 1; yTile++ )
                {
                    TileInfo tileInfo = CreateTile(xTile, yTile, 12);

                    AddToList(tileInfo);
                }
            }

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.DoWork += Worker_DoWork;

            worker.RunWorkerAsync();
        }

        //-----------------------------------------------------------------------

        private TileInfo CreateTile(int xTile, int yTile, int zoom)
        {
            int pixelX = xTile * 256 + 127;
            int pixelY = yTile * 256 + 127;

            LatLong location = MapMath.PixelToLatLong(pixelX, pixelY, zoom);

            TileInfo tileInfo = new TileInfo() { X = xTile, Y = yTile, Zoom = zoom, Density = distribution.CalculateDensity(location.Latitude, location.Longitude) };

            return tileInfo;
        }

        private void AddToList(TileInfo tileInfo)
        {
            if (!sortedTiles.ContainsKey(tileInfo.Heuristic))
                sortedTiles.Add(tileInfo.Heuristic, tileInfo);
        }

        //-----------------------------------------------------------------------

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            TileInfo bestTile = sortedTiles.First().Value;
            TileInfo incumbent = bestTile;

            int iterationCount = 0;

            while( bestTile.Zoom < 24 && !worker.CancellationPending && iterationCount < maxIterations )
            {
                sortedTiles.RemoveAt(0);

                AddToList(CreateTile(bestTile.X * 2    , bestTile.Y * 2    , bestTile.Zoom + 1));
                AddToList(CreateTile(bestTile.X * 2 + 1, bestTile.Y * 2    , bestTile.Zoom + 1));
                AddToList(CreateTile(bestTile.X * 2    , bestTile.Y * 2 + 1, bestTile.Zoom + 1));
                AddToList(CreateTile(bestTile.X * 2 + 1, bestTile.Y * 2 + 1, bestTile.Zoom + 1));

                bestTile = sortedTiles.First().Value;
                if (bestTile.Density > incumbent.Density)
                    incumbent = bestTile;

                if( iterationCount++ % 100 == 0)
                {
                    int percent = (int)((double)iterationCount / (double)maxIterations * 100.0);
                    worker.ReportProgress(percent, incumbent);
                }
            }
            e.Result = incumbent;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TileInfo tileInfo = e.UserState as TileInfo;

            if( tileInfo != null )
            {
                LatLong location = MapMath.PixelToLatLong(tileInfo.X * 256 + 127, tileInfo.Y * 256 + 127, tileInfo.Zoom);
                MapControl.Location center = new MapControl.Location(location.Latitude, location.Longitude);

                viewModel.IsBestSolutionVisible = true;
                viewModel.BestSolution = center;
                viewModel.ProgressPercent = e.ProgressPercentage;
                viewModel.IsProgressBarVisible = true;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if( e.Error == null )
            {
                TileInfo tileInfo = e.Result as TileInfo;
                LatLong location = MapMath.PixelToLatLong(tileInfo.X * 256 + 127, tileInfo.Y * 256 + 127, tileInfo.Zoom);
                MapControl.Location center = new MapControl.Location(location.Latitude, location.Longitude);

                viewModel.Zoom = tileInfo.Zoom;
                viewModel.Center = center;

                viewModel.IsProgressBarVisible = false;
            }
        }
    }

    /// <summary>
    /// Sort doubles in descending order
    /// </summary>
    public class DescendingDoubleComparer : IComparer<double>
    {
        public int Compare(double x, double y)
        {
            return y.CompareTo(x);
        }
    }
}
