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
  
    public partial class TrainWindow : Window
    {
        private readonly IRepository<StaffData> repository;
        private readonly StaffPredictionService predictionService;
        private readonly ReportService<StaffData> staffPredictionReportService;

        public TrainWindow()
        {
            InitializeComponent();
            repository = new InMemoryRepository<StaffData>();
            predictionService = new StaffPredictionService(repository);
            staffPredictionReportService = new ReportService<StaffData>(new JsonFileRepository<StaffData>());
        }
        private void BackToMainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void TrainAndPredictButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                predictionService.TrainAndPredict();
                ResultsTextBox.AppendText("Прогноз виконано.\n");
                var predictions = predictionService.TrainAndPredict();
                lstPredictions.Items.Clear();
                foreach (var prediction in predictions)
                {
                    lstPredictions.Items.Add($"Predicted StaffLevel: {prediction.PredictedStaffLevel:F2}");
                }
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Помилка при виконанні: {ex.Message}\n");
            }
        }
        private void SaveToJsonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allPrediction = repository.GetAll();
                staffPredictionReportService.CreateReport(allPrediction);
                ResultsTextBox.AppendText($"Результати збережені у JSON.\n");
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Помилка: {ex.Message}\n");
            }
        }
    }
}