using Accord.Statistics.Distributions.Univariate;
using System;
using Zalando.Teaser.Map;

namespace Zalando.Teaser
{
    public class Distribution
    {
        private NormalDistribution riverDistribution = new NormalDistribution(0.0, 2.730 / 1.96);
        private NormalDistribution satelliteDistribution = new NormalDistribution(0.0, 2.400 / 1.96);
        private LognormalDistribution brandenburgDistribution = null;

        public Distribution()
        {
            // Brandenburg log normal distribution
            double mean = 4.700;
            double mode = 3.877;
            double location = Math.Log(mode) + 2.0 * (Math.Log(mean) - Math.Log(mode)) / 3.0;
            double shape = Math.Sqrt(2.0 * (Math.Log(mean) - Math.Log(mode)) / 3.0);

            brandenburgDistribution = new LognormalDistribution(location, shape);
        }

        public double CalculateDensity(double latitude, double longitude)
        {
            double density = 0;

            double d1 = DistanceTo.Satellite(latitude, longitude);
            double d2 = DistanceTo.River(latitude, longitude);
            double d3 = DistanceTo.Brandenburg(latitude, longitude);

            if (d1 >= 0)
            {
                density += satelliteDistribution.ProbabilityDensityFunction(d1);
            }

            if (d2 >= 0)
            {
                density += riverDistribution.ProbabilityDensityFunction(d2);
            }

            density += brandenburgDistribution.ProbabilityDensityFunction(d3);
            return density;
        }

        public double SatelliteDensity(double latitude, double longitude)
        {
            double density = 0;

            double d1 = DistanceTo.Satellite(latitude, longitude);

            if (d1 >= 0)
            {
                density += satelliteDistribution.ProbabilityDensityFunction(d1);
            }
            return density;
        }

        public double RiverDensity(double latitude, double longitude)
        {
            double density = 0;

            double d2 = DistanceTo.River(latitude, longitude);

            if (d2 >= 0)
            {
                density += riverDistribution.ProbabilityDensityFunction(d2);
            }
            return density;
        }

        public double BrandenburgDensity(double latitude, double longitude)
        {
            double density = 0;

            double d3 = DistanceTo.Brandenburg(latitude, longitude);

            density += brandenburgDistribution.ProbabilityDensityFunction(d3);
            return density;
        }

        public double MaxDensity
        {
            get
            {
                return  brandenburgDistribution.ProbabilityDensityFunction(3.877) + 
                        satelliteDistribution.ProbabilityDensityFunction(0.0) + 
                        riverDistribution.ProbabilityDensityFunction(0.0);
            }
        }

        public double MaxSatellite
        {
            get { return satelliteDistribution.ProbabilityDensityFunction(0.0); }
        }

        public double MaxRiver
        {
            get { return riverDistribution.ProbabilityDensityFunction(0.0); }
        }

        public double MaxBrandenburg
        {
            get { return brandenburgDistribution.ProbabilityDensityFunction(3.877); }
        }
    }
}
