namespace MedLab3.Models.Inspection
{
    public class InspectionShortModel
    {
        public Guid id { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? CreateTime { get; set; }
        public DiagnosisModel Diagnosis { get; set; }
    }
}
