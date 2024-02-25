using MedLab3.Models.Enums;

namespace MedLab3.Models.Doctor
{
    public class DoctorEditModel
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        public Gender Gender { get; set; }

        public string Phone { get; set; }
    }
}
