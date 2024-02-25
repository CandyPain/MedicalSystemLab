using MedLab3.Models;
using MedLab3.Models.ICD;

namespace MedLab3.Services
{
    public interface IDictionaryService
    {
        Task<SpecialityPageModel> GetSpeciality(string? name, int page, int size);
        Task<Icd10SearchModel> Search(string? request, int page, int size);

        Task<List<Icd10RecordModel>> Roots();
    }
}
