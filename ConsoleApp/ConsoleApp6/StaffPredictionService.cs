using Microsoft.ML;
using Staff.Models;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;

namespace Staff.Core
{
    public class StaffPredictionService
    {
        private readonly IRepository<StaffData> _repository;
        private List<StaffData> _staffData = new List<StaffData>();
        public List<string> TrainingProgress { get; private set; } = new List<string>();
        public StaffPredictionService(IRepository<StaffData> repository)
        {
            _repository = repository;
        }
        private ITransformer _model;
        public (List<StaffPredictionWithComparison> PredictionsWithComparison, double raeTrain, Dictionary<string, float> FeatureImportance) TrainAndPredict()
        {
            var context = new MLContext();
            TrainingProgress.Clear();
            TrainingProgress.Add("Початок навчання моделі...");

            // Завантаження даних для тренування з файлу 1
            string trainingFilePath = "C:\\Users\\Artyom\\Documents\\ComputerScience\\Диплом\\New Microsoft Excel Worksheet1.xlsx";
            _repository.LoadFromExcel(trainingFilePath, hasStaffLevel: true);
            var trainingData = _repository.GetAll();
            var dataView = context.Data.LoadFromEnumerable(trainingData);
            var predictionsWithComparison = new List<StaffPredictionWithComparison>();
            // Побудова та навчання моделі
            var pipeline = context.Transforms.Concatenate("Features", "Sales", "Conversion", "Checks", "Area")
                .Append(context.Regression.Trainers.FastTree(
                    labelColumnName: "StaffLevel",
                    numberOfLeaves: 20,
                    numberOfTrees: 20,
                    minimumExampleCountPerLeaf: 2));
            var model = pipeline.Fit(dataView);
            _model = model;
            _repository.SaveModel(model);
            // Розрахунок вагових коефіцієнтів для ознак
            var featureImportanceDict = new Dictionary<string, float>();
            string[] features = { "Sales", "Conversion", "Checks", "Area" };
            var lastTransformer = model.LastTransformer as Microsoft.ML.Data.RegressionPredictionTransformer<Microsoft.ML.Trainers.FastTree.FastTreeRegressionModelParameters>;
            if (lastTransformer != null)
            {
                var modelParameters = lastTransformer.Model;
                VBuffer<float> weights = default;
                modelParameters.GetFeatureWeights(ref weights);

                var featureWeightsArray = weights.DenseValues().ToArray();
                for (int i = 0; i < features.Length; i++)
                {
                    featureImportanceDict[features[i]] = featureWeightsArray[i];
                }
            }
            else
            {
                TrainingProgress.Add("Не вдалося отримати вагові коефіцієнти моделі.");
            }

            // Розрахунок RAE
            float totalAbsoluteErrorTrain = 0;
            float totalAbsoluteActualTrain = 0;

            // Передбачення чисельності персоналу
            var predictionEngineTrain = context.Model.CreatePredictionEngine<StaffData, StaffPrediction>(model);

            foreach (var trainInstance in trainingData)
            {
                var prediction = predictionEngineTrain.Predict(trainInstance);
                var comparison = new StaffPredictionWithComparison
                {
                    Name = trainInstance.Name,
                    ActualStaffLevel = trainInstance.StaffLevel,
                    PredictedStaffLevel = prediction.PredictedStaffLevel
                };
                predictionsWithComparison.Add(comparison);
                float difference = Math.Abs(trainInstance.StaffLevel - prediction.PredictedStaffLevel);
                totalAbsoluteErrorTrain += difference;
                totalAbsoluteActualTrain += Math.Abs(trainInstance.StaffLevel);
                double currentRAE = totalAbsoluteErrorTrain / totalAbsoluteActualTrain;
                TrainingProgress.Add($"Помилка: {difference:F2} РАЕ: {currentRAE:F2} ");
            }

            double raeTrain = totalAbsoluteErrorTrain / totalAbsoluteActualTrain;

            return (predictionsWithComparison, raeTrain, featureImportanceDict);

        }

        public (List<StaffPredictionWithComparison> PredictionsWithComparison, double raeTrain, Dictionary<string, float> FeatureImportance) TrainModel()
        {
            var context = new MLContext();
            TrainingProgress.Clear();
            TrainingProgress.Add("Початок навчання моделі...");

            // Завантаження даних для навчання
            string trainingFilePath = "C:\\Users\\Artyom\\Documents\\ComputerScience\\Диплом\\New Microsoft Excel Worksheet1.xlsx";
            _repository.LoadFromExcel(trainingFilePath, hasStaffLevel: true);
            var trainingData = _repository.GetAll();
            var dataView = context.Data.LoadFromEnumerable(trainingData);
            var predictionsWithComparison = new List<StaffPredictionWithComparison>();

            // Побудова та навчання моделі за допомогою Random Forest
            var pipeline = context.Transforms.Concatenate("Features", "Sales", "Conversion", "Checks", "Area")
                .Append(context.Regression.Trainers.FastForest(
                    labelColumnName: "StaffLevel",
                    numberOfLeaves: 20,
                    numberOfTrees: 100,
                    minimumExampleCountPerLeaf: 2));

           var model = pipeline.Fit(dataView);
            _model = model;
            _repository.SaveModel(model);
            var featureImportanceDict = new Dictionary<string, float>();
            string[] features = { "Sales", "Conversion", "Checks", "Area" };
            // Вытаскиваем последний трансформер из модели
            var lastTransformer = model.LastTransformer as Microsoft.ML.Data.RegressionPredictionTransformer<Microsoft.ML.Trainers.FastTree.FastForestRegressionModelParameters>;
            if (lastTransformer != null)
            {
                // Получаем параметры модели
                var modelParameters = lastTransformer.Model;

                // Создаем буфер для хранения весов признаков
                VBuffer<float> weights = default;

                // Заполняем буфер весами
                modelParameters.GetFeatureWeights(ref weights);

                // Преобразуем VBuffer в массив
                var featureWeightsArray = weights.DenseValues().ToArray();

                // Заполняем словарь с именами признаков
                for (int i = 0; i < features.Length; i++)
                {
                    featureImportanceDict[features[i]] = featureWeightsArray[i];
                }
            }
            else
            {
                TrainingProgress.Add("Не удалось извлечь параметры модели.");
            }


            // Розрахунок RAE
            float totalAbsoluteErrorTrain = 0;
            float totalAbsoluteActualTrain = 0;


            var predictionEngineTrain = context.Model.CreatePredictionEngine<StaffData, StaffPrediction>(model);

            foreach (var trainInstance in trainingData)
            {
                var prediction = predictionEngineTrain.Predict(trainInstance);
                var comparison = new StaffPredictionWithComparison
                {
                    Name = trainInstance.Name,
                    ActualStaffLevel = trainInstance.StaffLevel,
                    PredictedStaffLevel = prediction.PredictedStaffLevel
                };
                predictionsWithComparison.Add(comparison);
                float difference = Math.Abs(trainInstance.StaffLevel - prediction.PredictedStaffLevel);
                totalAbsoluteErrorTrain += difference;
                totalAbsoluteActualTrain += Math.Abs(trainInstance.StaffLevel);
                double currentRAE = totalAbsoluteErrorTrain / totalAbsoluteActualTrain;
                TrainingProgress.Add($"Помилка: {difference:F2} РАЕ: {currentRAE:F2} ");
            }

            double raeTrain = totalAbsoluteErrorTrain / totalAbsoluteActualTrain;

            return (predictionsWithComparison,raeTrain, featureImportanceDict);
        }
        public (List<StaffPredictionWithComparison> PredictionsWithComparison, double RAE) PredictTestDataFastTree()
        {
            var context = new MLContext();
            string testFilePath = "C:\\Users\\Artyom\\Documents\\ComputerScience\\Диплом\\New Microsoft Excel Worksheet2.xlsx";
            _repository.ClearTrainingData();
            // Загружаем тестовые данные
            _repository.LoadFromExcel(testFilePath, hasStaffLevel: true);
            var testData = _repository.GetAll();
            if (_model == null)
            {
                _model = _repository.GetModel();
                throw new InvalidOperationException("Модель не загружена. Проверьте сохранение и загрузку.");
            }
            var predictionEngine = context.Model.CreatePredictionEngine<StaffData, StaffPrediction>(_model);  // Model FastTree
            var predictionsWithComparison = new List<StaffPredictionWithComparison>();

            float totalAbsoluteErrorTest = 0;
            float totalAbsoluteActualTest = 0;

            foreach (var testInstance in testData)
            {
                var prediction = predictionEngine.Predict(testInstance);
                var comparison = new StaffPredictionWithComparison
                {
                    Name = testInstance.Name,
                    ActualStaffLevel = testInstance.StaffLevel,
                    PredictedStaffLevel = prediction.PredictedStaffLevel
                };

                predictionsWithComparison.Add(comparison);

                float difference = Math.Abs(comparison.ActualStaffLevel - comparison.PredictedStaffLevel);
                totalAbsoluteErrorTest += difference;
                totalAbsoluteActualTest += Math.Abs(comparison.ActualStaffLevel);
            }

            double raeTest = totalAbsoluteErrorTest / totalAbsoluteActualTest;

            return (predictionsWithComparison, raeTest);
        }

        public (List<StaffPredictionWithComparison> PredictionsWithComparison, double RAE) PredictTestDataFastForest()
        {
            var context = new MLContext();
            string testFilePath = "C:\\Users\\Artyom\\Documents\\ComputerScience\\Диплом\\New Microsoft Excel Worksheet2.xlsx";
            _repository.ClearTrainingData();
            _repository.LoadFromExcel(testFilePath, hasStaffLevel: true);
            var testData = _repository.GetAll();
            if (_model == null)
            {
                _model = _repository.GetModel();
                throw new InvalidOperationException("Модель не загружена. Проверьте сохранение и загрузку.");
            }
            var predictionEngine = context.Model.CreatePredictionEngine<StaffData, StaffPrediction>(_model);  // Model FastForest
            var predictionsWithComparison = new List<StaffPredictionWithComparison>();

            float totalAbsoluteErrorTest = 0;
            float totalAbsoluteActualTest = 0;

            foreach (var testInstance in testData)
            {
                var prediction = predictionEngine.Predict(testInstance);
                var comparison = new StaffPredictionWithComparison
                {
                    ActualStaffLevel = testInstance.StaffLevel,
                    PredictedStaffLevel = prediction.PredictedStaffLevel
                };

                predictionsWithComparison.Add(comparison);

                float difference = Math.Abs(comparison.ActualStaffLevel - comparison.PredictedStaffLevel);
                totalAbsoluteErrorTest += difference;
                totalAbsoluteActualTest += Math.Abs(comparison.ActualStaffLevel);
            }

            double raeTest = totalAbsoluteErrorTest / totalAbsoluteActualTest;

            return (predictionsWithComparison, raeTest);
        }
        public void ClearData()
        {
            _staffData.Clear();  // Очищаем коллекцию данных в репозитории
        }
        
    }
}

