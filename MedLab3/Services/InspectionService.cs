using MedLab3.Data;
using MedLab3.Models;
using MedLab3.Models.Enums;
using MedLab3.Models.ICD;
using MedLab3.Models.Inspection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MedLab3.Services
{
    public class InspectionService : IInspectionService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public InspectionService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        async Task<List<InspectionPreviewModel>> IInspectionService.ChainInspectionAsync(Guid Id)
        {
            IQueryable<InspectionModel> inspections = _context.Inspections.Include(d=>d.Doctor).Include(d=>d.Diagnoses).Include(p=>p.Patient);
            List<InspectionPreviewModel> res = new List<InspectionPreviewModel>();
            InspectionModel current = await inspections.SingleOrDefaultAsync(i => i.Id == Id);
            if(current == null)
            {
                throw new DirectoryNotFoundException("Inspection with this ID not found");
            }
            if(current.PreviousInspectionId != null)
            {
                throw new ArgumentException("Try to do chain with non-root inspection");
            }
            while(true)
            {
                current = await inspections.SingleOrDefaultAsync(i => i.PreviousInspectionId == current.Id);
                if(current == null)
                {
                    break;
                }
                IQueryable<DiagnosisModel> diagnoses = _context.DiagnosisModels.Where(d=>d.inspectionId == current.Id);
                res.Add(new InspectionPreviewModel
                {
                    Conclusion = current.Conclusion,
                    CreateTime = current.CreateTime,
                    Date = current.Date,
                    PreviousId = current.PreviousInspectionId,
                    Id = current.Id,
                    Doctor = current.Doctor.Name,
                    Patient = current.Patient.Name,
                    PatientId = current.Patient.Id,
                    DoctorId = current.Doctor.Id,
                    Diagnosis = diagnoses.SingleOrDefault(d => d.Type == DiagnosisType.Main),
                    HasChain = false,
                    HasNested = inspections.SingleOrDefault(i => i.PreviousInspectionId == current.Id) != null ? true : false
                });
                if (!res.Last().HasNested)
                {
                    break;
                }
            }
            return res;
        }

        async Task IInspectionService.EditInspectionAsync(Guid Id,InspectionEditModel editModel, Guid userId)
        {
            IQueryable<MkbRecord> mkbRecords = _context.MkbRecords;
            var inspection = await _context.Inspections.Include(i => i.Diagnoses).SingleOrDefaultAsync(i => i.Id == Id);
            if (inspection == null)
            {
                throw new DirectoryNotFoundException("Not Found");
            }
            if(userId != inspection.DoctorId)
            {
                throw new RankException();
            }
            if (!editModel.Diagnoses.Any(d => d.Type == DiagnosisType.Main))
            {
                throw new ArgumentException("No main diagnos");
            }
            if (editModel.Diagnoses.Where(d => d.Type == DiagnosisType.Main).Count() > 1)
            {
                throw new ArgumentException("More than 1 Main");
            }
            if (editModel.Conclusion == Conclusion.Recovery && editModel.NextVisitDate != null)
            {
                throw new ArgumentException("Попытка указать дату следующего посещения при Recovery");
            }
            if (editModel.NextVisitDate < inspection.Date || (editModel.Conclusion == Conclusion.Disease && editModel.NextVisitDate == null))
            {
                throw new ArgumentException("В случае болезни укажите корректную дату следующего визита");
            }
            if (editModel.Conclusion != Conclusion.Death)
            {
                if (editModel.DeathDate != null)
                {
                    throw new ArgumentException("No DateDeath");
                }
            }
            else
            {
                if (editModel.DeathDate == null)
                {
                    throw new ArgumentException("Укажите дату смерти");
                }
            }
            inspection.Anamnesis = editModel.Anamnesis;
            inspection.Complaints = editModel.Complaints;
            inspection.Treatment = editModel.Treatment;
            inspection.Conclusion = editModel.Conclusion;
            inspection.NextVisitDate = editModel.NextVisitDate;
            inspection.DeathDate = editModel.DeathDate;          
            foreach(var item in inspection.Diagnoses)
            {
                DiagnosisModel delete = item;
                _context.DiagnosisModels.Remove(delete);
            }
            _context.SaveChanges();
            inspection = await _context.Inspections.Include(i => i.Diagnoses).SingleOrDefaultAsync(i => i.Id == Id);
            foreach (var item in editModel.Diagnoses)
            {
                DiagnosisModel current = new DiagnosisModel();
                current.CreateTime = DateTime.Now;
                current.Description = item.Description;
                current.Type = item.Type;
                MkbRecord rec =  await mkbRecords.SingleOrDefaultAsync(rec => rec.ID == item.IcdDiagnosisId);
                if(rec == null || rec.ACTUAL == 0)
                {
                    throw new ArgumentException("MKB Not Actual");
                }
                current.Id = Guid.NewGuid();
                current.mkbId = rec.ID;
                current.Code = rec.REC_CODE;
                current.Name = rec.MKB_NAME;
                current.inspectionId = inspection.Id;
                _context.DiagnosisModels.Add(current);
            }
            await _context.SaveChangesAsync();
        }

        async Task<InspectionModel> IInspectionService.GetInspectionAsync(Guid Id)
        {
            var inspection = await _context.Inspections
        .Include(i => i.Doctor)
        .Include(i => i.Patient)
        .Include(i => i.Diagnoses)
        .Include(i => i.Consultations)
            .ThenInclude(c => c.Speciality)
        .Include(i => i.Consultations)
            .ThenInclude(c => c.RootComment)
        .SingleOrDefaultAsync(i => i.Id == Id);
            if (inspection == null)
            {
                throw new DirectoryNotFoundException("Not Found");
            }
            var inspectionModel = new InspectionModel
            {
                Id = inspection.Id,
                CreateTime = inspection.CreateTime,
                Date = inspection.Date,
                Anamnesis = inspection.Anamnesis,
                Complaints = inspection.Complaints,
                Treatment = inspection.Treatment,
                Conclusion = inspection.Conclusion,
                NextVisitDate = inspection.NextVisitDate,
                DeathDate = inspection.DeathDate,
                BaseInspectionId = inspection.BaseInspectionId,
                PreviousInspectionId = inspection.PreviousInspectionId,
                Consultations = inspection.Consultations,
                Diagnoses = _context.DiagnosisModels.Where(i=> i.inspectionId == inspection.Id).ToList(),
                Doctor = inspection.Doctor,
                Patient = inspection.Patient,
                PatientId = inspection.PatientId,
                DoctorId = inspection.DoctorId,
            };
            return inspectionModel;
        }

        async Task IInspectionService.HasMail(Guid Id)
        {
            InspectionModel cur = _context.Inspections.SingleOrDefault(i => i.Id == Id);
            if(cur== null)
            {
                throw new DirectoryNotFoundException("Not Found");
            }
            cur.HasMail = true;
            _context.SaveChanges();
        }

        async Task IInspectionService.MKBInit()
        {
            string jsonContent = File.ReadAllText("MKB.Json");
            var mkbData = JsonConvert.DeserializeObject<MkbData>(jsonContent);
            foreach (var mkb in mkbData.Records)
            {
                _context.MkbRecords.Add(mkb);
            }
            _context.SaveChanges();
        }
    }
}
