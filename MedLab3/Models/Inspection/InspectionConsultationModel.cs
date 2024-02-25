namespace MedLab3.Models.Inspection
{
    public class InspectionConsultationModel
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; }

        public Guid InspectionId { get; set; }

        public SpecialityModel Speciality { get; set; }

        public InspectionCommentModel RootComment { get; set; }

        public int CommentsNumber { get; set; }
    }
}
