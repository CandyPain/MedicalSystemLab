using MedLab3.Models.Enums;

namespace MedLab3.Models.Inspection
{
    public class InspectionEditModel
    {
        public string Anamnesis { get; set; }

        public string Complaints { get; set; }

        public string Treatment { get; set; }

        public Conclusion Conclusion { get; set; }

        public DateTime? NextVisitDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public List<DiagnosisCreateModel> Diagnoses { get; set; }
    }
}
