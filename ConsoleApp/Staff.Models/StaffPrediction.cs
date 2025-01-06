using Microsoft.ML.Data;

namespace Staff.Models
{
    public class StaffPrediction
    {
        [NoColumn]
        public Guid Id { get; set; }
        [NoColumn]
        public  int Name { get; set; }
        [ColumnName("Score")]
        public float PredictedStaffLevel { get; set; }
        [NoColumn]
        public float Sales { get; set; }
        [NoColumn]
        public float Conversion { get; set; }
        [NoColumn]
        public float Checks { get; set; }
        [NoColumn]
        public float Area { get; set; }

    }
}

