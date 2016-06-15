using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Accord.Math.Optimization;
using Zalando.Teaser.Geometry;
using Zalando.Teaser.Map;
using Zalando.Teaser.ViewModel;

namespace Zalando.Teaser
{
    public class Optimizer
    {
        private Distribution distribution = new Distribution();
        private BackgroundWorker worker;
        private MainViewModel viewModel;

        private BroydenFletcherGoldfarbShanno bfgs = null;

        public Optimizer( MainViewModel viewModel )
        {
            this.viewModel = viewModel;

            bfgs = new BroydenFletcherGoldfarbShanno(2, Function, Gradient);
            bfgs.Progress += Bfgs_Progress;
        }

        public bool CancelRequested { get; private set; }

        public void Start(double latitude, double longitude)
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.DoWork += Worker_DoWork;

            worker.RunWorkerAsync(new LatLong(latitude, longitude));
        }

        public void Stop()
        {
            CancelRequested = true;
            worker.CancelAsync();
        }

        //-----------------------------------------------------------------------

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            LatLong startLocation = e.Argument as LatLong;
            double[] v = new double[] { startLocation.Latitude, startLocation.Longitude };

            bfgs.Maximize(v);

            e.Result = bfgs.Solution;
        }

        private void Bfgs_Progress(object sender, OptimizationProgressEventArgs e)
        {
            worker.ReportProgress(0, e.Solution);

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double[] solution = (double[])e.UserState;

            viewModel.IsBestSolutionVisible = true;
            viewModel.BestSolution = new MapControl.Location(solution[0], solution[1]);
            viewModel.ProgressPercent = e.ProgressPercentage;
            viewModel.IsProgressBarVisible = true;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if( e.Error == null )
            {
                double[] solution = (double[])e.Result;

                viewModel.Center = new MapControl.Location(solution[0], solution[1]);
                viewModel.Zoom = 21;

                viewModel.IsProgressBarVisible = false;
            }
        }

        //---------------------------------------------------------------------

        private double Function(double[] x)
        {
            return distribution.CalculateDensity(x[0], x[1]);
        }

        private double[] Gradient(double[] vector)
        {
            double x = vector[0];
            double y = vector[1];
            double[] g = new double[2] { 0, 0 };
            double h = 1e-5;

            Func<double,double,double> f = distribution.CalculateDensity;
            
            // Five-point stencil
            g[0] = (-f(x+2*h,y) + 8*f(x+h,y) - 8*f(x-h,y) + f(x-2*h,y)) / (12*h);
            g[1] = (-f(x,y+2*h) + 8*f(x,y+h) - 8*f(x,y-h) + f(x,y-2*h)) / (12*h);

            return g;
        }
    }
}
