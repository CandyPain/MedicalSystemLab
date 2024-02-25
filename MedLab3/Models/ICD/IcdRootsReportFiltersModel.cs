namespace MedLab3.Models.ICD
{
    public class IcdRootsReportFiltersModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> IcdRoots { get; set; }
    }

}
