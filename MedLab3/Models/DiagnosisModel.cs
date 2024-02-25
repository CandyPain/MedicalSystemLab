using MedLab3.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedLab3.Models
{
    public class DiagnosisModel
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid mkbId { get; set; }

        public DateTime CreateTime { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public DiagnosisType Type { get; set; }
        [JsonIgnore]
        public Guid? inspectionId { get; set; }
    }
}
