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

    public partial class MainWindow : Window
    {
        private readonly InMemoryRepository<StaffData> _repository;
        public MainWindow()
        {
            InitializeComponent();
            _repository = new InMemoryRepository<StaffData>();
        }

        private void TrainModelButton_Click(object sender, RoutedEventArgs e)
        {
            var trainWindow = new TrainWindow(_repository);
            trainWindow.Show();
           // this.Close();
        }

        private void PredictionButton_Click(object sender, RoutedEventArgs e)
        {
            var predictWindow = new PredictWindow(_repository); 
            predictWindow.Show();
          //  this.Close();
        }
    }
}