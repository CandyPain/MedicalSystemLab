namespace MedLab3.Models.Inspection
{
    public class InspectionPagedListModel
    {
        public List<InspectionPreviewModel> inspections { get; set; }
        public PageInfoModel pageInfo { get; set; }
    }
}
