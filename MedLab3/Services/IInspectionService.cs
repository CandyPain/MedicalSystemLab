using MedLab3.Models.Inspection;

namespace MedLab3.Services
{
    public interface IInspectionService
    {
        Task<InspectionModel> GetInspectionAsync(Guid Id);
        Task EditInspectionAsync(Guid Id,InspectionEditModel editModel, Guid userId);
        Task<List<InspectionPreviewModel>> ChainInspectionAsync(Guid Id);
        Task MKBInit();
        Task HasMail(Guid Id);
    }
}
