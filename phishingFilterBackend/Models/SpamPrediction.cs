using Microsoft.ML.Data;

namespace phishingFilterBackend.Models
{
    /// <summary>
    /// La clase SpamPrediction contiene una unica predicción de spam.
    /// </summary>
    public class SpamPrediction
    {
        [ColumnName("PredictedLabel")] public bool IsSpam { get; set; }
        public float Score { get; set; }
        public float Probability { get; set; }
    }
}
