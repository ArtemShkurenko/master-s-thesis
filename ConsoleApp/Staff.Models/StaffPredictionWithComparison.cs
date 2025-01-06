

namespace Staff.Models
{
    public class StaffPredictionWithComparison
    {
        public int Name { get; set; }
        public float ActualStaffLevel { get; set; }
        public float PredictedStaffLevel { get; set; }
        public float Difference => PredictedStaffLevel - ActualStaffLevel;
        public string Comparison => Difference >= 0 ? "+" : "-";
    }
}

