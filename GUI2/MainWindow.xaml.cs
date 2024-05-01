using System.Windows;
using EFCore.Data;

namespace EFCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel(new MobsContext());
            DataContext = ViewModel;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadData();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveChanges();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteAllMobs();
            ViewModel.GenerateMobs();
        }
    }
}
