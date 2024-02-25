using MedLab3.Models.Enums;

namespace MedLab3.Models
{
    public class DiagnosisCreateModel
    {
        public Guid IcdDiagnosisId { get; set; }
        public string? Description { get; set; }
        public DiagnosisType Type { get; set; }
    }
}
