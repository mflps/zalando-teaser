using MapControl;
using System.Windows;
using System.Windows.Input;
using Zalando.Teaser.ViewModel;

namespace Zalando.Teaser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowDensityLayer_Checked(object sender, RoutedEventArgs e)
        {
            map.TileLayers.Add(Resources["DensityTileLayer"] as Map.DensityTileLayer);
        }

        private void ShowDensityLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            map.TileLayers.Remove(Resources["DensityTileLayer"] as Map.DensityTileLayer);
        }

        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            MainViewModel viewModel = DataContext as MainViewModel;

            if( viewModel != null )
            {
                Point mousePoint = e.GetPosition(map);
                Location mouseLocation = map.ViewportPointToLocation(mousePoint);

                viewModel.MouseLocation = mouseLocation;
            }
        }
    }
}
