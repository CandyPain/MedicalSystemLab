using MedLab3.Data;
using MedLab3.Models;
using MedLab3.Models.Enums;
using MedLab3.Models.ICD;
using MedLab3.Models.Inspection;
using MedLab3.Models.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MedLab3.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        private readonly string RegexBirthDate = @"^\d{2}.\d{2}.\d{4} \d{1,2}:\d{2}:\d{2}$";
        public PatientService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }

        async Task<PatientPagedListModel> IPatientService.GetPatientsList(Guid DoctorId, string? name, Conclusion[]? conclusions, Sorting sort = 0, bool scheduledVisits = false, bool onlyMine = false, int page = 1, int size = 5)
        {
            IQueryable<PatientModel> patients = _context.Patients;
            IQueryable<InspectionModel> inspections = _context.Inspections;
            if (name != null)
            {
                patients = patients.Where(p => p.Name.Contains(name));
            }
            if(sort == null)
            {
                throw new ArgumentException("Ошибка аргументов фильтрации");
            }
            if (page <= 0 || size <= 0)
            {
                throw new ArgumentException("Некорректный номер/размер страницы.");
            }
            if(conclusions == null)
            {
                conclusions = new Conclusion[3];
                conclusions[0] = Conclusion.Disease;
                conclusions[1] = Conclusion.Recovery;
                conclusions[2] = Conclusion.Death;
            }
            if (conclusions.Length != 0)
            {
                patients = patients
                .Select(patient => new
                {
                    Patient = patient,
                    LatestInspection = inspections
                        .Where(inspection => inspection.Patient.Id == patient.Id)
                        .OrderByDescending(inspection => inspection.Date)
                        .FirstOrDefault()
                })
                .Where(data => data.LatestInspection != null && conclusions.Contains(data.LatestInspection.Conclusion))
                .Select(data => data.Patient);
            }
            switch (sort.ToString())
            {
                case "NameAsc":
                    patients = patients.OrderBy(q => q.Name);
                    break;
                case "CreateAsc":
                    patients = patients.OrderBy(q => q.CreateTime);
                    break;
                case "InspectionAsc":
                    patients = patients.OrderBy(q => q.LatestInspectionTime);
                    break;
                case "NameDesc":
                    patients = patients.OrderByDescending(q => q.Name);
                    break;
                case "CreateDesc":
                    patients = patients.OrderByDescending(q => q.CreateTime);
                    break;
                case "InspectionDesc":
                    patients = patients.OrderByDescending(q => q.LatestInspectionTime);
                    break;
            }
            if(scheduledVisits)
            {
                patients = patients
                .Select(patient => new
                {
                     Patient = patient,
                     LatestInspection = inspections
                      .Where(inspection => inspection.Patient.Id == patient.Id)
                      .OrderByDescending(inspection => inspection.Date)
                      .FirstOrDefault()
                })
                .Where(data => data.LatestInspection != null && data.LatestInspection.NextVisitDate != null)
                .Select(data => data.Patient);
            }
            if(onlyMine)
            {
                patients = patients
                .Where(patient => inspections.Any(inspection => inspection.Patient.Id == patient.Id && inspection.Doctor.Id ==DoctorId));
            }
            int skip = (page - 1) * size;
            int Count1 = patients.Count();
            patients= patients.Skip(skip).Take(size);
            List<PatientModel> patientList = patients.ToList();

            PatientPagedListModel model = new PatientPagedListModel
            {
                Patients = patientList,
                PageInfo = new PageInfoModel
                {
                    Size = size,
                    Count = (Count1 % size == 0 ? Count1 / size : Count1 / size + 1),
                    Current = page
                }
            };
            return model;

        }

        async Task<Guid> IPatientService.CreatePatient(PatientCreateModel patient)
        {
            if (patient == null)
            {
                throw new ArgumentException("Bad Data");
            }
            if (!Regex.IsMatch(patient.BirthDate.ToString(), RegexBirthDate) || patient.BirthDate.Value.Year >= DateTime.Now.Year)
            {
                throw new ArgumentException("Bad Data");
            }
            PatientModel patientModel = new PatientModel();
            patientModel.CreateTime = DateTime.Now;
            patientModel.Id = Guid.NewGuid();
            patientModel.Name = patient.Name;
            patientModel.Gender = patient.Gender;
            patientModel.Birthday = patient.BirthDate;
            _context.Patients.Add(patientModel);
            _context.SaveChanges();
            return patientModel.Id;
        }

        async Task<PatientModel> IPatientService.GetPatientId(Guid Id)
        {
            var patient = await _context.Patients.SingleOrDefaultAsync(p => p.Id == Id);
            if(patient == null)
            {
                throw new DirectoryNotFoundException();
            }
            return patient;
        }

        async Task<Guid> IPatientService.CreateInspection(Guid DoctorId,Guid PatientId, InspectionCreateModel inspection)
        {
            IQueryable<InspectionModel> queryInspections = _context.Inspections;
            IQueryable<SpecialityModel> specialites = _context.Specialitys; 
            if(!inspection.Consultations.All(c=> specialites.SingleOrDefault(s=>s.Id == c.SpecialityId) != null))
            {
                throw new ArgumentException("wrong Speciality ID");
            }
            if(inspection.PreviousInspectionId != null && queryInspections.SingleOrDefault(i=>i.Id==inspection.PreviousInspectionId) == null)
            {
                throw new ArgumentException("Privious inspectiont doest not exist");
            }
            if((inspection.PreviousInspectionId != null && inspection.Date< queryInspections.SingleOrDefault(i=>i.Id==inspection.PreviousInspectionId).Date)||inspection.Date > DateTime.Now.AddMinutes(10))
            {
                throw new ArgumentException("дата создания осмотра не должна быть больше текущего времени, осмотр не может быть сделан ранее предыдущего осмотра в цепочке");
            }
            if(!inspection.Diagnoses.Any(d=>d.Type==DiagnosisType.Main))
            {
                throw new ArgumentException("No main diagnos");
            }
            if(inspection.Diagnoses.Where(d=>d.Type == DiagnosisType.Main).Count()>1)
            {
                throw new ArgumentException("More than 1 Main");
            }
            if(inspection.Conclusion == Conclusion.Recovery && inspection.NextVisitDate != null)
            {
                throw new ArgumentException("Попытка указать дату следующего посещения при Recovery");
            }
            if(inspection.NextVisitDate < inspection.Date ||(inspection.Conclusion ==Conclusion.Disease && inspection.NextVisitDate == null))
            {
                throw new ArgumentException("В случае болезни укажите корректную дату следующего визита");
            }
            if(inspection.Conclusion != Conclusion.Death)
            {
                if(inspection.DeathDate != null)
                {
                    throw new ArgumentException("No DateDeath");
                }
            }
            else
            {
                if(inspection.DeathDate == null)
                {
                    throw new ArgumentException("Укажите дату смерти");
                }
            }
            InspectionModel inspectionModel = new InspectionModel();
            inspectionModel.Id = Guid.NewGuid();
            inspectionModel.PatientId = PatientId;
            inspectionModel.DoctorId = DoctorId;
            inspectionModel.Doctor = await _context.Doctors.SingleOrDefaultAsync(p => p.Id == DoctorId);
            inspectionModel.Patient = await _context.Patients.SingleOrDefaultAsync(p => p.Id == PatientId);
            inspectionModel.NextVisitDate = inspection.NextVisitDate;
            inspectionModel.DeathDate = inspection.DeathDate;
            inspectionModel.Date = inspection.Date;
            inspectionModel.Treatment = inspection.Treatment;
            inspectionModel.CreateTime = DateTime.Now;
            inspectionModel.Anamnesis = inspection.Anamnesis;
            inspectionModel.Conclusion = inspection.Conclusion;
            inspectionModel.Complaints = inspection.Complaints;
            inspectionModel.PreviousInspectionId = inspection.PreviousInspectionId;
            inspectionModel.HasMail = false;
            inspectionModel.Diagnoses = new List<DiagnosisModel>();
            inspectionModel.Consultations = new List<InspectionConsultationModel>();
            if(inspection.PreviousInspectionId == null)
            {
                inspectionModel.BaseInspectionId = Guid.NewGuid();
            }
            else
            {
                InspectionModel FindBase = new InspectionModel();
                FindBase.PreviousInspectionId = inspection.PreviousInspectionId;
                while(true)
                {
                    FindBase = await queryInspections.SingleOrDefaultAsync(i => i.Id == FindBase.PreviousInspectionId);
                    if(FindBase.PreviousInspectionId == null)
                    {
                        inspectionModel.BaseInspectionId = FindBase.Id; break;
                    }
                }
            }
            foreach (var item in inspection.Diagnoses)
            {
                DiagnosisModel current = new DiagnosisModel();
                current.CreateTime = DateTime.Now;
                current.Description = item.Description;
                current.Type = item.Type;
                MkbRecord rec = await _context.MkbRecords.SingleOrDefaultAsync(rec => rec.ID == item.IcdDiagnosisId);
                if (rec == null|| rec.ACTUAL == 0)
                {
                    throw new ArgumentException("MKB Not Actual");
                }
                current.Id = Guid.NewGuid();
                current.Code = rec.REC_CODE;
                current.Name = rec.MKB_NAME;
                current.mkbId = rec.ID;
                current.inspectionId = inspectionModel.Id;
                inspectionModel.Diagnoses.Add(current);
            }
            foreach(var item in inspection.Consultations)
            {
                SpecialityModel currentSpec = await specialites.SingleOrDefaultAsync(s => s.Id == item.SpecialityId);
                if (item.Comment == null || item.SpecialityId == null || currentSpec == null)
                {
                    throw new ArgumentException("Bad Consultation Data");
                }
                Guid newId = Guid.NewGuid();
                InspectionConsultationModel current = new InspectionConsultationModel
                {
                    Id = newId,
                    InspectionId = inspectionModel.Id,
                    Speciality = currentSpec,
                    CreateTime = DateTime.Now,
                    CommentsNumber = 1,
                    RootComment = new InspectionCommentModel
                    {
                        Author = inspectionModel.Doctor,
                        CreateTime = DateTime.Now,
                        ModifyTime = null,
                        Content = item.Comment.Content,
                        Id = Guid.NewGuid(),
                        ParentId = null,
                        ConsultationId = newId
                    }
                };
                inspectionModel.Consultations.Add(current);
            }
            _context.Inspections.Add(inspectionModel);
            _context.Patients.SingleOrDefault(p => p.Id == PatientId).LatestInspectionTime = inspectionModel.Date;
            _context.SaveChanges();
            return inspectionModel.Id;

        }

        async Task<InspectionPagedListModel> IPatientService.GetInspections(Guid PatientId, bool? grouped = false, List<Guid>? icdRoots = null, int page = 1, int size = 5)
        {
            IQueryable<InspectionModel> inspectionsQuery = _context.Inspections
                .Include(d=>d.Doctor).Include(p=>p.Patient).Include(p=>p.Diagnoses);
            IQueryable<DiagnosisModel> diagnosisModels = _context.DiagnosisModels;
            if(_context.Patients.SingleOrDefault(p=>p.Id== PatientId) == null)
            {
                throw new DirectoryNotFoundException("patient not found");
            }
            if(page<=0 || size<=0)
            {
                throw new ArgumentException("Bad page format");
            }
            IQueryable<MkbRecord> records = _context.MkbRecords;
            if (!icdRoots.All(r => records.SingleOrDefault(m => m.ID == r) != null))
            {
                throw new ArgumentException("Bad ICD10 Id");
            }
            inspectionsQuery = inspectionsQuery.Where(i=> i.PatientId == PatientId);
            if(grouped == true)
            {
                inspectionsQuery = inspectionsQuery.Where(i=>i.PreviousInspectionId == null);
            }
            int skip = (page - 1) * size;
            int Count1 = inspectionsQuery.Count();
            if(icdRoots!= null && icdRoots.Count != 0)
            inspectionsQuery = inspectionsQuery.Where(i=> diagnosisModels.Where(m=>m.inspectionId==i.Id).Any(d=>d.Type == DiagnosisType.Main && icdRoots.Contains(d.mkbId))); //добавить grouped
            InspectionPagedListModel model = new InspectionPagedListModel
            {
                inspections = inspectionsQuery.Select(item => new InspectionPreviewModel
                {
                    Conclusion = item.Conclusion,
                    CreateTime = item.CreateTime,
                    Date = item.Date,
                    Diagnosis = diagnosisModels.SingleOrDefault(d =>d.inspectionId==item.Id && d.Type == DiagnosisType.Main),
                    Doctor = item.Doctor.Name,
                    DoctorId = item.DoctorId,
                    Id = item.Id,
                    Patient = item.Patient.Name,
                    PatientId = item.PatientId,
                    PreviousId = item.PreviousInspectionId,
                    HasChain = item.PreviousInspectionId == null ? true : false,
                    HasNested = inspectionsQuery.SingleOrDefault(i => i.PreviousInspectionId == item.Id) != null ? true : false
                }).ToList(),
                pageInfo = new PageInfoModel
                {
                    Size = size,
                    Count = (Count1 % size == 0 ? Count1 / size : Count1 / size + 1),
                    Current = page
                }
            };
            model.inspections = model.inspections.Skip(skip).Take(size).ToList();
            return model;
        }

        async Task<List<InspectionShortModel>> IPatientService.InspectionSearch(Guid patientId, string request)
        {
            if(_context.Patients.SingleOrDefault(p=>p.Id == patientId) == null)
            {
                throw new DirectoryNotFoundException("PatientNotFound");
            }
            IQueryable<InspectionModel> inspections = _context.Inspections;
            IQueryable<DiagnosisModel> diagnosisModels = _context.DiagnosisModels;
            inspections = inspections.Where(i=> i.PatientId == patientId);
            if (request != null)
            {
                inspections = inspections.Where(i => i.Diagnoses.Any(d => d.Name.Contains(request) || d.Code.Contains(request)));
            }
            List<InspectionShortModel> result = inspections.Select(item => new InspectionShortModel
            {
                CreateTime = item.CreateTime,
                Date = item.Date, 
                id = item.Id,
                Diagnosis = diagnosisModels.SingleOrDefault(d =>d.inspectionId == item.Id &&  d.Type == DiagnosisType.Main)
            }).ToList();
            return result;
        }
    }
}
