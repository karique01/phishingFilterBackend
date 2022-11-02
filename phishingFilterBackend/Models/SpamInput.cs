using Microsoft.ML.Data;

namespace phishingFilterBackend.Models
{
    /// <summary>
    /// La clase SpamInput contiene un solo mensaje que puede ser spam o no
    /// </summary>
    public class SpamInput
    {
        [LoadColumn(0)] public string RawLabel { get; set; }
        [LoadColumn(1)] public string Message { get; set; }
    }
}
