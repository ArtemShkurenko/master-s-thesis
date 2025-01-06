using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staff.Core
{
    public class ReportService<StaffPrediction>
    {
        private readonly IReportRepository<StaffPrediction> _jsonRepository;

        internal readonly string _reporDir;
        internal string appDir = Directory.GetCurrentDirectory();

        public ReportService(IReportRepository<StaffPrediction> jsonRepository)
        {
            _jsonRepository = jsonRepository;
            _reporDir = Path.Combine(appDir, "Reports");
            Directory.CreateDirectory(_reporDir);
        }
        public void CreateReport(IEnumerable<StaffPrediction> entity)
        {
            var filepath = Path.Combine(_reporDir, $"{typeof(StaffPrediction).Name}-{DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss")}.json");
            _jsonRepository.SaveRecords((IEnumerable<StaffPrediction>)entity, filepath);
            Console.WriteLine(filepath);
        }
        public IEnumerable<StaffPrediction> LoadReport(string fileName)
        {
            var reportFilePath = Path.Combine(_reporDir, fileName);
            if (string.IsNullOrEmpty(reportFilePath))
                throw new ArgumentNullException(nameof(fileName));

            if (!File.Exists(reportFilePath))
                throw new FileNotFoundException($"File not found: {fileName}");

            var extension = Path.GetExtension(fileName);
            return (IEnumerable<StaffPrediction>)_jsonRepository.ReadRecords(reportFilePath);

        }
    }
}
