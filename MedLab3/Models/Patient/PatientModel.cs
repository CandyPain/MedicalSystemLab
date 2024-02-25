using MedLab3.Models.Enums;
using System.Text.Json.Serialization;

namespace MedLab3.Models.Patient
{
    public class PatientModel
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        public Gender Gender { get; set; }
        [JsonIgnore]
        public DateTime? LatestInspectionTime { get; set; }
    }
}
