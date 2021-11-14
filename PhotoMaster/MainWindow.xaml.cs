using System.Windows;
using PhotoMaster.Models;

namespace PhotoMaster
{
    internal sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            this.ViewModel = new MainViewModel();
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
        }

        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
        }
    }
}
