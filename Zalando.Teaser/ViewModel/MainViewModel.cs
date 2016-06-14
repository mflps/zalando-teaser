using Accord.Statistics.Distributions.Univariate;
using AForge;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MapControl;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;

namespace Zalando.Teaser.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private LocationCollection riverSpree = new LocationCollection();
        private LocationCollection riverRange = new LocationCollection();
        private Location brandenburgGate = new Location(52.516288, 13.377689);
        private LocationCollection satellite = new LocationCollection();
        private LocationCollection satelliteRange = new LocationCollection();
        private LocationCollection brandenburgRange = new LocationCollection();
        private TileLayer densityTileLayer = new TileLayer(new Map.OverlayTileImageLoader());
        private Location mouseLocation = null;
        private Location bestSolution = null;

        private bool isRiverRangeVisible = false;
        private bool isSatelliteRangeVisible = false;
        private bool isBrandenburgRangeVisible = false;
        private double density = 0;
        private bool isDensityVisible = false;

        private SqlGeography geoRiverRange = null;
        private SqlGeography geoSatelliteRange = null;
        private SqlGeography geoBrandenburgRange = null;

        private Distribution distribution = new Distribution();

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            riverSpree.Add(new Location(52.529198, 13.274099));
            riverSpree.Add(new Location(52.531835, 13.29234));
            riverSpree.Add(new Location(52.522116, 13.298541));
            riverSpree.Add(new Location(52.520569, 13.317349));
            riverSpree.Add(new Location(52.524877, 13.322434));
            riverSpree.Add(new Location(52.522788, 13.329));
            riverSpree.Add(new Location(52.517056, 13.332075));
            riverSpree.Add(new Location(52.522514, 13.340743));
            riverSpree.Add(new Location(52.517239, 13.356665));
            riverSpree.Add(new Location(52.523063, 13.372158));
            riverSpree.Add(new Location(52.519198, 13.379453));
            riverSpree.Add(new Location(52.522462, 13.392328));
            riverSpree.Add(new Location(52.520921, 13.399703));
            riverSpree.Add(new Location(52.515333, 13.406054));
            riverSpree.Add(new Location(52.514863, 13.416354));
            riverSpree.Add(new Location(52.506034, 13.435923));
            riverSpree.Add(new Location(52.496473, 13.461587));
            riverSpree.Add(new Location(52.487641, 13.483216));
            riverSpree.Add(new Location(52.488739, 13.491456));
            riverSpree.Add(new Location(52.464011, 13.503386));

            satellite.Add(new Location(52.590117, 13.39915));
            satellite.Add(new Location(52.437385, 13.553989));

            if( !IsInDesignMode )
            {
                FindMaximumCommand = new RelayCommand(FindMaximum);
            }
        }

        public RelayCommand FindMaximumCommand { get; private set; }

        public LocationCollection RiverSpree
        {
            get { return riverSpree; }
        }

        public LocationCollection RiverRange
        {
            get { return riverRange; }
        }

        public Location BrandenburgGate
        {
            get { return brandenburgGate; }
        }

        public LocationCollection Satellite
        {
            get { return satellite; }
        }

        public LocationCollection SatelliteRange
        {
            get { return satelliteRange; }
        }

        public LocationCollection BrandenburgRange
        {
            get { return brandenburgRange; }
        }

        public bool IsRiverRangeVisible
        {
            get { return isRiverRangeVisible; }
            set
            {
                if(isRiverRangeVisible != value)
                {
                    isRiverRangeVisible = value;
                    RaisePropertyChanged("IsRiverRangeVisible");

                    if(isRiverRangeVisible)
                        CalculateRiverRange();
                }
            }
        }

        public bool IsSatelliteRangeVisible
        {
            get { return isSatelliteRangeVisible; }
            set
            {
                if (isSatelliteRangeVisible != value)
                {
                    isSatelliteRangeVisible = value;
                    RaisePropertyChanged("IsSatelliteRangeVisible");

                    if (isSatelliteRangeVisible)
                        CalculateSatelliteRange();
                }
            }
        }

        public bool IsBrandenburgRangeVisible
        {
            get { return isBrandenburgRangeVisible; }
            set
            {
                if (isBrandenburgRangeVisible != value)
                {
                    isBrandenburgRangeVisible = value;
                    RaisePropertyChanged("IsBrandenburgRangeVisible");

                    if (isBrandenburgRangeVisible)
                        CalculateBrandenburgRange();
                }
            }
        }

        public TileLayer DensityTileLayer
        {
            get { return densityTileLayer; }
        }

        public double Density
        {
            get { return density; }
            set
            {
                if(density != value)
                {
                    density = value;
                    RaisePropertyChanged("Density");
                }
            }
        }

        public bool IsDensityVisible
        {
            get { return isDensityVisible; }
            set
            {
                if(isDensityVisible != value)
                {
                    isDensityVisible = value;
                    RaisePropertyChanged("IsDensityVisible");
                }
            }
        }

        public Location MouseLocation
        {
            get { return mouseLocation; }
            set
            {
                if(mouseLocation != value)
                {
                    mouseLocation = value;
                    RaisePropertyChanged("MouseLocation");

                    Density = distribution.CalculateDensity(mouseLocation.Latitude, mouseLocation.Longitude);
                }
            }
        }

        private Location center = new Location(52.516288, 13.377689);
        
        public Location Center
        {
            get { return center; }
            set
            {
                if(center != value)
                {
                    center = value;
                    RaisePropertyChanged("Center");
                }
            }
        }

        private double zoom = 12;

        public double Zoom
        {
            get { return zoom; }
            set
            {
                if(zoom != value)
                {
                    zoom = value;
                    RaisePropertyChanged("Zoom");
                }
            }
        }

        public Location BestSolution
        {
            get { return bestSolution; }
            set
            {
                if( bestSolution != value )
                {
                    bestSolution = value;
                    RaisePropertyChanged("BestSolution");
                }
            }
        }

        private bool isBestSolutionVisible = false;

        public bool IsBestSolutionVisible
        {
            get { return isBestSolutionVisible; }
            set
            {
                if( isBestSolutionVisible != value )
                {
                    isBestSolutionVisible = value;
                    RaisePropertyChanged("IsBestSolutionVisible");
                }
            }
        }

        private string statusBarText = "Ready...";

        public string StatusBarText
        {
            get { return statusBarText; }
            set
            {
                if(statusBarText != value)
                {
                    statusBarText = value;
                    RaisePropertyChanged("StatusBarText");
                }
            }
        }

        private bool isProgressBarVisible = false;

        public bool IsProgressBarVisible
        {
            get { return isProgressBarVisible; }
            set
            {
                if( isProgressBarVisible != value )
                {
                    isProgressBarVisible = value;
                    RaisePropertyChanged("IsProgressBarVisible");
                }
            }
        }

        private int progressPercent = 0;

        public int ProgressPercent
        {
            get { return progressPercent; }
            set
            {
                if( progressPercent != value )
                {
                    progressPercent = value;
                    RaisePropertyChanged("ProgressPercent");
                }
            }
        }

        //---------------------------------------------------------------------

        private void CalculateRiverRange()
        {
            geoRiverRange = CalculateRange(riverSpree, 2730.0);

            GeoRangeToPolygon(geoRiverRange, riverRange);
        }

        private void CalculateSatelliteRange()
        {
            geoSatelliteRange = CalculateRange(satellite, 2400.0);

            GeoRangeToPolygon(geoSatelliteRange, satelliteRange);
        }

        private void CalculateBrandenburgRange()
        {
            double mean = 4700.0;
            double mode = 3877.0;
            double location = System.Math.Log(mode) + 2.0 * (System.Math.Log(mean) - System.Math.Log(mode)) / 3.0;
            double shape = System.Math.Sqrt(2.0 * (System.Math.Log(mean) - System.Math.Log(mode)) / 3.0);
            LognormalDistribution logNormal = new LognormalDistribution(location, shape);
            DoubleRange range =  logNormal.GetRange(0.95);
            SqlGeography geoCenter = SqlGeography.Point(brandenburgGate.Latitude, brandenburgGate.Longitude, 4326);

            geoBrandenburgRange = geoCenter.STBuffer(mode);

            GeoRangeToPolygon(geoBrandenburgRange, brandenburgRange);
        }

        private SqlGeography CalculateRange( LocationCollection polyline, double range )
        {
            StringBuilder text = new StringBuilder("LINESTRING(");

            for (int i = 0; i < polyline.Count; i++)
            {
                if (i > 0)
                    text.Append(", ");
                text.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}", polyline[i].Longitude, polyline[i].Latitude);
            }
            text.Append(")");

            SqlChars sqlText = new SqlChars(text.ToString());
            SqlGeography geoPolyLine = SqlGeography.STLineFromText(sqlText, 4326);

            SqlGeography geoRange = geoPolyLine.STBuffer(range);

            return geoRange;
        }

        private void GeoRangeToPolygon( SqlGeography geoRange, LocationCollection polygon )
        {
            int numPoints = geoRange.STNumPoints().Value;

            polygon.Clear();
            for (int i = 1; i <= numPoints; i++)
            {
                SqlGeography point = geoRange.STPointN(i);

                polygon.Add(new Location(point.Lat.Value, point.Long.Value));
            }
        }

        private void FindMaximum()
        {
            Optimizer optimizer = new Optimizer(this);

            optimizer.Start(brandenburgGate.Latitude, brandenburgGate.Longitude);
        }
    }
}