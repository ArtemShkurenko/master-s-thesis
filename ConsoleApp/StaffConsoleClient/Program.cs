using Staff.Core;
using Staff.DALL;
using Staff.Models;


class program
{
    static void Main(string[] args)
    {
        string trainingFilePath = "C:\\Users\\Artyom\\Documents\\ComputerScience\\Диплом\\New Microsoft Excel Worksheet1.xlsx";
        string testFilePath = "C:\\Users\\Artyom\\Documents\\ComputerScience\\Диплом\\New Microsoft Excel Worksheet2.xlsx";
        // Создание репозитория
        var repository = new InMemoryRepository<StaffData>();

        // Создание сервиса предсказаний
        var predictionService = new StaffPredictionService(repository);

        // Тренировка модели и предсказания
        predictionService.TrainAndPredict();

        var staffPredictionReportService = new ReportService<StaffData>(new JsonFileRepository<StaffData>());
        var allPrediction = repository.GetAll();
        staffPredictionReportService.CreateReport(allPrediction);


        Console.ReadLine();
    }
}
