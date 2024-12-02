using Newtonsoft.Json;
using Staff.Core;

namespace Staff.DALL
{
    public class JsonFileRepository<StaffPrediction> : IReportRepository<StaffPrediction>
    {

        public void SaveRecords(IEnumerable<StaffPrediction> records, string fullFilePath)
        {
            var dataToSave = JsonConvert.SerializeObject(records);
            File.WriteAllText(fullFilePath, dataToSave);
        }
        public List<StaffPrediction> ReadRecords(string fileName)
        {
            string dataReadFromFile = File.ReadAllText(fileName);
            IEnumerable<StaffPrediction> deserializedCollection = JsonConvert.DeserializeObject<IEnumerable<StaffPrediction>>(dataReadFromFile);
            if (deserializedCollection == null)
            {
                throw new Exception("Not found exception");
            }
            return deserializedCollection.ToList();
        }
    }
}
