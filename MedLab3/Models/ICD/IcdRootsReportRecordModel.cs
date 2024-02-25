using MedLab3.Models.Enums;
using System.Text.Json.Serialization;

namespace MedLab3.Models.ICD
{
    public class IcdRootsReportRecordModel
    {
        public string? PatientName { get; set; }
        public DateTime? patientBirthdate { get; set; }
        public Gender Gender { get; set; }
        public List<Tuple<Guid, int>> VisitsByRoot { get; set; }
        [JsonIgnore]
        public Guid? PatientId { get; set; }
    }
}
