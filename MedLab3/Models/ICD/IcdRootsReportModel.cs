namespace MedLab3.Models.ICD
{
    public class IcdRootsReportModel
    {
        public IcdRootsReportFiltersModel filters { get; set; }
        public List<IcdRootsReportRecordModel> records { get; set; }
        public List<Tuple<Guid, int>> visitsByRoot { get; set; }
    }
}
