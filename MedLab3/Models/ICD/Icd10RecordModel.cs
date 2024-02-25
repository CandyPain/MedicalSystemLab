using System.Text.Json.Serialization;

namespace MedLab3.Models.ICD
{
    public class Icd10RecordModel
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public DateTime? CreateTime { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

}
