using MedLab3.Models.Enums;

namespace MedLab3.Models.Doctor
{
    public class DoctorRegisterModel
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTime? Birthday { get; set; }

        public Gender Gender { get; set; }

        public string Phone { get; set; }

        public Guid Speciality { get; set; }
    }
}
