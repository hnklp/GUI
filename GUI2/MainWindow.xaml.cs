using System.Windows;
using EFCore.Data;

namespace EFCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel = new MainWindowViewModel(new MobsContext());

            Loaded += MainWindow_Loaded;
        }

        //Be careful when using async void as these are fire and forget
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e) => await ViewModel.LoadData();
    }
}
