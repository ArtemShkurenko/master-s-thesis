using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Staff.Models;

namespace Staff.Core
{
    public interface IReportRepository<StaffPrediction>
    {
        public void SaveRecords(IEnumerable<StaffPrediction> records, string fullFilePath);
        public List<StaffPrediction> ReadRecords(string fileName);
    }
}
