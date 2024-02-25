using MedLab3.Models.Doctor;
using Newtonsoft.Json;

namespace MedLab3.Models.Inspection
{
    public class InspectionCommentModel
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; }

        public Guid? ParentId { get; set; }

        public string Content { get; set; }

        public DoctorModel Author { get; set; }

        public DateTime? ModifyTime { get; set; }

        [JsonIgnore]
        public Guid ConsultationId { get; set; }
    }

}
