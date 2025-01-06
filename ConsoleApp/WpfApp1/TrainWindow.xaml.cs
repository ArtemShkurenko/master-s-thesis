using OxyPlot.Series;
using OxyPlot;
using Staff.Core;
using Staff.DALL;
using Staff.Models;
using System.Windows;
using System.Windows.Controls;




namespace WpfApp1
{
    public partial class TrainWindow : Window
    {
        private readonly InMemoryRepository<StaffData> _repository;
        private readonly StaffPredictionService predictionService;
        private readonly ReportService<StaffData> staffPredictionReportService;

        public TrainWindow(InMemoryRepository<StaffData> repository)
        {
            InitializeComponent();
            predictionService = new StaffPredictionService(repository);
            staffPredictionReportService = new ReportService<StaffData>(new JsonFileRepository<StaffData>());
            _repository = repository;
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
                var(predictionsWithComparison, rae, featureImportance) = predictionService.TrainAndPredict();
                if (predictionsWithComparison == null || !predictionsWithComparison.Any())
                {
                    ResultsTextBox.AppendText("Дані не завантаженні.\n");
                    return;
                }

                // Обновлення даних у DataGrid
                ComparisonDataGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                ComparisonDataGrid.ItemsSource = null;
                ComparisonDataGrid.ItemsSource = predictionsWithComparison;
             
                ResultsTextBox.AppendText($"RAE для тестової моделі: {rae:F4}\n");
               
                ResultsTextBox.AppendText("Вага ознак:\n");
                float sumFeature = 0;
                foreach (var item in featureImportance)
                {
                    sumFeature += item.Value;
                }
                foreach (var item in featureImportance)
                {
                    ResultsTextBox.AppendText($"Фактор: {item.Key}, Вага ознаки: {item.Value/sumFeature:F4}\n");
                }

                ResultsTextBox.ScrollToEnd();
                PlotErrors(predictionsWithComparison);  // Додаємо відображення графіка

                ResultsTextBox.AppendText("Прогноз для тестового набору.\n");
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Помилка при виконанні прогнозу: {ex.Message}\n");
            }
        }
  
        private void SaveToJsonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allPrediction = _repository.GetAll();
                staffPredictionReportService.CreateReport(allPrediction);
                ResultsTextBox.AppendText($"Результати збережені у JSON.\n");
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Помилка: {ex.Message}\n");
            }
        }
        private void TrainPredictAndCompareRandomForestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Прогноз та порівняння даних тестових та фактичних
                var (predictionsWithComparison, rae, featureImportance) = predictionService.TrainModel();
              

                if (predictionsWithComparison == null || !predictionsWithComparison.Any())
                {
                    ResultsTextBox.AppendText("Нет данных для отображения.\n");
                    return;
                }

                // Обновлення даних у DataGrid
                ComparisonDataGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                ComparisonDataGrid.ItemsSource = null;
                ComparisonDataGrid.ItemsSource = predictionsWithComparison;

                ResultsTextBox.AppendText($"RAE для тестової моделі: {rae:F4}\n");
                // Отображаем важность признаков
                ResultsTextBox.AppendText("Вага ознак:\n");
                foreach (var item in featureImportance)
                {
                    ResultsTextBox.AppendText($"Фактор: {item.Key}, Вага ознаки: {item.Value:F4}\n");
                }

                ResultsTextBox.ScrollToEnd();
                PlotErrors(predictionsWithComparison);  // Додаємо відображення графіка

            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"Ошибка при выполнении прогноза: {ex.Message}\n");
            }
        }

        private void PlotErrors(List<StaffPredictionWithComparison> predictions)
        {
            var plotModel = new PlotModel { Title = "Помилки навчання моделі" };
            // Додаємо підписи осів
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = "Ітерація"
            });

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Помилки"
            });
            var errors = predictions.Select((item, idx) =>
                new DataPoint(idx, Math.Abs(item.ActualStaffLevel - item.PredictedStaffLevel))
            ).ToList();

            var lineSeries = new LineSeries
            {
                Title = "Помилки",
                MarkerType = MarkerType.Circle
            };

            // Додайте дані у серію
            lineSeries.Points.AddRange(errors);

            plotModel.Series.Add(lineSeries);

            PlotView.Model = plotModel;
        }
        private void PlotStaffLevels(List<StaffData> actualData, List<StaffPrediction> predictedData)
        {
            var plotModel = new PlotModel { Title = "Фактичні та розраховані чисельності" };

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = "Номер ТТ"
            });

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Чисельність персоналу"
            });

            var actualLine = new LineSeries { Title = "Фактичні дані", MarkerType = MarkerType.Circle };
            var predictedLine = new LineSeries { Title = "Розраховані дані", MarkerType = MarkerType.Diamond };

            foreach (var data in actualData)
                actualLine.Points.Add(new DataPoint(data.Name, data.StaffLevel));

            foreach (var prediction in predictedData)
                predictedLine.Points.Add(new DataPoint(prediction.Name, prediction.PredictedStaffLevel));

            plotModel.Series.Add(actualLine);
            plotModel.Series.Add(predictedLine);

            PlotView.Model = plotModel;
        }

    }
}