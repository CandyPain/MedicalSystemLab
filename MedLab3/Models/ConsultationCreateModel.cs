using MedLab3.Models.Inspection;

namespace MedLab3.Models
{
    public class ConsultationCreateModel
    {
        public Guid SpecialityId { get; set; }
        public InspectionCommentCreateModel Comment { get; set; }
    }
}
