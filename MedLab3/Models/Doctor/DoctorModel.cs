using MedLab3.Models.Enums;
using System.Text.Json.Serialization;

namespace MedLab3.Models.Doctor
{
    public class DoctorModel
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public Guid Speciality { get; set; }

        public DateTime CreateTime { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        public Gender Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

    }
}
