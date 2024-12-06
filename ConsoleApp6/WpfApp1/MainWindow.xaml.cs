using Staff.Core;
using Staff.DALL;
using Staff.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace WpfApp1
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

        private void TrainModelButton_Click(object sender, RoutedEventArgs e)
        {
            var trainWindow = new TrainWindow();
            trainWindow.Show();
        }

        private void PredictionButton_Click(object sender, RoutedEventArgs e)
        {
            var predictWindow = new PredictWindow();
            predictWindow.Show();
        }
    }
}