using MedLab3.Models.ICD;

namespace MedLab3.Services
{
    public interface IReportService
    {
        Task<IcdRootsReportModel> GetRootsReportAsync(DateTime start, DateTime end, List<Guid>? icdRoots);
    }
}
