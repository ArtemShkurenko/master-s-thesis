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
    public partial class PredictWindow : Window
    {
        private readonly StaffPredictionService predictionService;

        public PredictWindow()
        {
            InitializeComponent();
            predictionService = new StaffPredictionService(new InMemoryRepository<StaffData>());
        }

        private void LoadTestDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Логіка завантаження тестових даних
            ResultsTextBox.AppendText("Фактичні дані завантажено.\n");
        }
        private void BackToMainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void PredictButton_Click(object sender, RoutedEventArgs e)
        {
 
        }
    }
}
