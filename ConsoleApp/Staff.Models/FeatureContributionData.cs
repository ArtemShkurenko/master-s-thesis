using Microsoft.ML.Data;

namespace Staff.Models
{
    public class FeatureContributionData
    {
        [VectorType(4)]
        public float[] FeatureContributions { get; set; }
    }
}
