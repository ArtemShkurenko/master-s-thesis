using Microsoft.ML.Data;

namespace Staff.Models
{
    public class StaffData
    {
        [NoColumn]
        public int Name { get; set; }
        [LoadColumn(1)]
        public float Sales { get; set; }

        [LoadColumn(2)]
        public float Conversion { get; set; }

        [LoadColumn(3)]
        public float Checks { get; set; }

        [LoadColumn(4)]
        public float Area { get; set; }

        [LoadColumn(5)]
        public float StaffLevel { get; set; }
    }
}
