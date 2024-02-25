using MedLab3.Models.Enums;

namespace MedLab3.Models.Patient
{
    public class PatientCreateModel
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
