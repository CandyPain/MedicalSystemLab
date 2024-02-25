using MedLab3.Models;
using MedLab3.Models.Inspection;

namespace MedLab3.Services
{
    public interface IConsultationService
    {
        Task<InspectionPagedListModel> GetInspections(Guid DoctorId, bool? grouped, List<Guid>? isdRoots, int page, int size);
        Task<ConsultationModel> GetConsultation(Guid Id);
        Task<Guid> CreateComment(Guid DoctorId,Guid Id, CommentCreateModel commentCreateModel);
        Task EditComment(Guid DoctorId, Guid Id, InspectionCommentCreateModel content);
    }
}
