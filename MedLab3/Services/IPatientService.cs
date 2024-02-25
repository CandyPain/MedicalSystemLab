using MedLab3.Models.Enums;
using MedLab3.Models.Inspection;
using MedLab3.Models.Patient;

namespace MedLab3.Services
{
    public interface IPatientService
    {
        Task<Guid> CreatePatient(PatientCreateModel patient);
        Task<PatientModel> GetPatientId(Guid Id);
        Task<PatientPagedListModel> GetPatientsList(Guid DoctorId, string? name, Conclusion[]? conclusions, Sorting sort = 0,bool scheduledVisits = false,bool onlyMine = false, int page = 1, int size = 5);
        Task<Guid> CreateInspection(Guid DoctorId,Guid PatientId, InspectionCreateModel inspection);
        Task<InspectionPagedListModel> GetInspections(Guid PatientId, bool? grouped = false, List<Guid>? icdRoots = null, int page = 1, int size = 5);
        Task<List<InspectionShortModel>> InspectionSearch(Guid patientId, string request);
    }
}
