using Microsoft.ML;
using Staff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;

namespace Staff.Core
{
    public class StaffPredictionService
    {
        private readonly IRepository<StaffData> _repository;
        public StaffPredictionService(IRepository<StaffData> repository)
        {
            _repository = repository;
        }
        public List<StaffPrediction> TrainAndPredict()
        {
            var context = new MLContext();
            // Завантаження даних для тренування з файлу 1
            string trainingFilePath = "C:\\Users\\Artyom\\Documents\\ComputerScience\\Диплом\\New Microsoft Excel Worksheet1.xlsx";
            _repository.LoadFromExcel(trainingFilePath, hasStaffLevel: true);
            var trainingData = _repository.GetAll();
            var dataView = context.Data.LoadFromEnumerable(trainingData);

            // Побудова та навчання моделі
            var pipeline = context.Transforms.Concatenate("Features", "Sales", "Conversion", "Checks", "Area")
                .Append(context.Regression.Trainers.FastTree(
                    labelColumnName: "StaffLevel",
                    numberOfLeaves: 20,
                    numberOfTrees: 20,
                    minimumExampleCountPerLeaf: 2));
            var model = pipeline.Fit(dataView);

            // Завантаження тестових даних з файлу 2
            string testFilePath = "C:\\Users\\Artyom\\Documents\\ComputerScience\\Диплом\\New Microsoft Excel Worksheet2.xlsx";
            _repository.LoadFromExcel(testFilePath, hasStaffLevel: false);
            var testData = _repository.GetAll();

            // Передбачення чисельності персоналу
            var predictionEngine = context.Model.CreatePredictionEngine<StaffData, StaffPrediction>(model);
            Console.WriteLine("Predictions:");
            foreach (var testInstance in testData)
            {
                var prediction = predictionEngine.Predict(testInstance);
                Console.WriteLine($"Sales: {testInstance.Sales}, Conversion: {testInstance.Conversion}, " +
                                      $"Checks: {testInstance.Checks}, Area: {testInstance.Area}, " +
                                      $"Predicted StaffLevel: {prediction.PredictedStaffLevel:F2}");
            }
            List<StaffPrediction> predictions = new List<StaffPrediction>();

            foreach (var testInstance in testData)
            {
                var prediction = predictionEngine.Predict(testInstance);
                predictions.Add(prediction); 
            }
            return predictions;
        }
    }
}

