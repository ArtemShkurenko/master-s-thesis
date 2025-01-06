using DocumentFormat.OpenXml.EMMA;
using Microsoft.ML;
using Newtonsoft.Json;
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
        private ITransformer _model; // Поле для зберігання моделі
        private List<StaffPredictionWithComparison> predictionsWithComparison;
        private readonly InMemoryRepository<StaffData> _repository;

        public PredictWindow(InMemoryRepository<StaffData> repository)
        {
            InitializeComponent();
            _repository = repository;
            predictionService = new StaffPredictionService(_repository);
            _model = _repository.GetModel(); // Ініціалізує модель
        }

        private void BackToMainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void PredictFastTreeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_model != null)
                {
                    var (predictionsWithComparison, rae) = predictionService.PredictTestDataFastTree();

                    ResultsTextBox.AppendText($"RAE для FastTree моделі: {rae:F4}\n");

                    foreach (var prediction in predictionsWithComparison)
                    {
                        float diff = (prediction.ActualStaffLevel - prediction.PredictedStaffLevel);
                        ResultsTextBox.AppendText($"Фактичний: {prediction.ActualStaffLevel} Прогнозований: {prediction.PredictedStaffLevel} Відхилення: {diff:F2}\n");
                    }
                }
                else
                {
                    ResultsTextBox.AppendText("Помилка: модель не була навчена!\n");
                }
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Помилка прогнозування FastTree: {ex.Message}\n");
            }
        }
        private void PredictFastForestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_model != null)
                {
                    var (predictionsWithComparison, rae) = predictionService.PredictTestDataFastForest();

                    ResultsTextBox.AppendText($"RAE для FastForest моделі: {rae:F4}\n");

                    foreach (var prediction in predictionsWithComparison)
                    {
                        float diff = (prediction.ActualStaffLevel - prediction.PredictedStaffLevel);
                        ResultsTextBox.AppendText($"Фактичний: {prediction.ActualStaffLevel} Прогнозований: {prediction.PredictedStaffLevel} Відхилення: {diff:F2}\n");
                    }
                }
                else
                {
                    ResultsTextBox.AppendText("Помилка: модель не була навчена!\n");
                }
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Помилка прогнозування FastForest: {ex.Message}\n");
            }
        }
        private void SaveToJsonButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}