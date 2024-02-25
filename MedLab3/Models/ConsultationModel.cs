namespace MedLab3.Models
{
    public class ConsultationModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid inspectionId { get; set; }
        public SpecialityModel Speciality { get; set; }
        public List<CommentModel> Comments { get; set; }
    }
}
