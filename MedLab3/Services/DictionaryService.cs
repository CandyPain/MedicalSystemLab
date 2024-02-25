using MedLab3.Data;
//using MedLab3.Migrations;
using MedLab3.Models;
using MedLab3.Models.ICD;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MedLab3.Services
{
    public class DictionaryService : IDictionaryService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        public DictionaryService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }

        async Task<Icd10SearchModel> IDictionaryService.Search(string? request, int page, int size)
        {
            IQueryable<MkbRecord> RecodrsQeury = _context.MkbRecords;
            RecodrsQeury = RecodrsQeury.Where(r=> r.ACTUAL == 1);
            if (page <= 0 || size <= 0)
            {
                throw new ArgumentException("Bad Page or Size argument");
            }
            if (request != null)
            {
                    RecodrsQeury = RecodrsQeury.Where(r => r.MKB_NAME.Contains(request) || r.MKB_CODE.Contains(request));
            }
            int skip = (page - 1) * size;
            int Count1 = RecodrsQeury.Count();
            RecodrsQeury = RecodrsQeury.Skip(skip).Take(size);
            Icd10SearchModel result = new Icd10SearchModel
            {
                Records = RecodrsQeury.Select(item => new Icd10RecordModel
                {
                    Code = item.MKB_CODE,
                    CreateTime = DateTime.Now,
                    Id = item.ID,
                    Name = item.MKB_NAME
                }).ToList(),
                Pagination = new PageInfoModel
                {
                    Size = size,
                    Count = (Count1 % size == 0 ? Count1 / size : Count1 / size + 1),
                    Current = page
                }
            };
            return result;
        }
        async Task<SpecialityPageModel> IDictionaryService.GetSpeciality(string? name, int page, int size)
        {
            /*
            var specialtiesJson = File.ReadAllText("Spec.json");
            var specialties = JsonConvert.DeserializeObject<List<SpecialityModel>>(specialtiesJson);

            _context.Specialitys.AddRange(specialties);
            _context.SaveChanges();
            */
            IQueryable<SpecialityModel> specialityModels = _context.Specialitys;
            if (name != null)
            {
                specialityModels = specialityModels.Where(s => s.Name.Contains(name));
            }
            if (page <= 0 || size <= 0)
            {
                throw new ArgumentException("Bad page or size data");
            }
            int skip = (page - 1) * size;
            int Count1 = specialityModels.Count();
            specialityModels = specialityModels.Skip(skip).Take(size);
            var specModList = specialityModels.ToList();
            SpecialityPageModel model = new SpecialityPageModel
            {
                Speciality = specModList,
                PageInfo = new PageInfoModel
                {
                    Size = size,
                    Count = (Count1 % size == 0 ? Count1 / size : Count1 / size + 1),
                    Current = page
                }
            };
            return model;
        }

        async Task<List<Icd10RecordModel>> IDictionaryService.Roots()
        {
            IQueryable<MkbRecord> mkbRecords = _context.MkbRecords;
            mkbRecords = mkbRecords.Where(m => m.ACTUAL == 1 && m.ID_PARENT == null);
            List<Icd10RecordModel> result = mkbRecords.Select(m => new Icd10RecordModel
            {
                Id = m.ID,
                Code = m.REC_CODE,
                CreateTime = DateTime.Now,
                Name = m.MKB_NAME,
            }).ToList();
            return result;
        }
    }

}

