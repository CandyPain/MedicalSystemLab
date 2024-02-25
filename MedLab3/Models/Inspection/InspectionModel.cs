using MedLab3.Models.Doctor;
using MedLab3.Models.Enums;
using MedLab3.Models.Patient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedLab3.Models.Inspection
{
    public class InspectionModel
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? Date { get; set; }

        public string? Anamnesis { get; set; }

        public string? Complaints { get; set; }

        public string? Treatment { get; set; }

        public Conclusion Conclusion { get; set; }

        public DateTime? NextVisitDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public Guid BaseInspectionId { get; set; }

        public Guid? PreviousInspectionId { get; set; }
        [JsonIgnore]
        public Guid PatientId { get; set; }
        [ForeignKey("PatientId")]
        public PatientModel Patient { get; set; }

        [JsonIgnore]
        public Guid DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public DoctorModel Doctor { get; set; }

        public List<DiagnosisModel> Diagnoses { get; set; }

        public List<InspectionConsultationModel>? Consultations { get; set; }
        [JsonIgnore]
        public bool HasMail { get; set; }
    }
}
