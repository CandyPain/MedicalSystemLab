namespace MedLab3.Models.ICD
{
    public class MkbRecord
    {
        public Guid ID { get; set; }
        public int ACTUAL { get; set; }
        public string MKB_CODE { get; set; }
        public string MKB_NAME { get; set; }
        public string REC_CODE { get; set; }
        public Guid? ID_PARENT { get; set; }
    }
}
