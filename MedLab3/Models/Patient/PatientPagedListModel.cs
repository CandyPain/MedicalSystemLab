namespace MedLab3.Models.Patient
{
    public class PatientPagedListModel
    {
        public List<PatientModel> Patients { get; set; }
        public PageInfoModel PageInfo { get; set; }
    }
}
