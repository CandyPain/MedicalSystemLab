using MedLab3.Data;
using MedLab3.Models.ICD;
using MedLab3.Models.Inspection;
using MedLab3.Models.Patient;
using Microsoft.EntityFrameworkCore;

namespace MedLab3.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        public ReportService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }

        async Task<IcdRootsReportModel> IReportService.GetRootsReportAsync(DateTime start, DateTime end, List<Guid>? icdRoots)
        {
            if(end<start)
            {
                throw new ArgumentException("Bad Time");
            }
            IQueryable<MkbRecord> recordsq = _context.MkbRecords;
            if(icdRoots == null ||  icdRoots.Count != 0)
            {
                recordsq = recordsq.Where(r=>icdRoots.Contains(r.ID));
                if(recordsq.Any(r=>r.ID_PARENT != null))
                {
                    throw new ArgumentException("Non Root element");
                }
            }
            else
            {
                recordsq = recordsq.Where(r => r.ID_PARENT == null);
            }
            var records = recordsq.ToList();
            IcdRootsReportModel model = new IcdRootsReportModel();
            model.visitsByRoot = new List<Tuple<Guid, int>>();
            model.filters = new IcdRootsReportFiltersModel
            {
                Start = start,
                End = end,
                IcdRoots = records.Select(r => r.MKB_NAME).ToList()
            };
            foreach(var item in records)
            {
                model.visitsByRoot.Add(new Tuple<Guid, int>(item.ID, 0));
            }
            List<IcdRootsReportRecordModel> recordModels = new List<IcdRootsReportRecordModel>();
            IQueryable<InspectionModel> inspections = _context.Inspections.Include(d=>d.Diagnoses);
            inspections = inspections.Where(i => i.Date >= start && i.Date <= end);
            List<Guid> recordGuidList = records.Select(r => r.ID).ToList();
            inspections = inspections.Where(i => recordGuidList.Contains(i.Diagnoses.SingleOrDefault(d => d.Type == Models.Enums.DiagnosisType.Main).mkbId));
            List<PatientModel> patients = _context.Patients.ToList();
            foreach(var item in inspections)
            {
                Guid MkbDiagnosesId = item.Diagnoses.SingleOrDefault(d => d.Type == Models.Enums.DiagnosisType.Main).mkbId;
                var current = recordModels.SingleOrDefault(r => r.PatientId == item.PatientId);
                if(current != null)
                {
                    var curTuple = current.VisitsByRoot.SingleOrDefault(t => t.Item1 == MkbDiagnosesId);
                    if (curTuple != null)
                    {
                        current.VisitsByRoot[current.VisitsByRoot.IndexOf(curTuple)] = Tuple.Create(MkbDiagnosesId, curTuple.Item2 + 1);
                        var ModelVisit = model.visitsByRoot.SingleOrDefault(t => t.Item1 == MkbDiagnosesId);
                        model.visitsByRoot[model.visitsByRoot.IndexOf(ModelVisit)] = Tuple.Create(ModelVisit.Item1,ModelVisit.Item2+1);
                    }
                    else
                    {
                        current.VisitsByRoot.Add(new Tuple<Guid, int>(MkbDiagnosesId, 1));
                    }
                }
                else
                {
                    var curPatient = patients.SingleOrDefault(p => p.Id == item.PatientId);
                    IcdRootsReportRecordModel newModel = new IcdRootsReportRecordModel();
                    newModel.patientBirthdate = curPatient.Birthday;
                    newModel.PatientId = curPatient.Id;
                    newModel.PatientName = curPatient.Name;
                    newModel.Gender = curPatient.Gender;
                    newModel.VisitsByRoot = new List<Tuple<Guid, int>>();
                    foreach(var icd in records.ToList())
                    {
                        if (icd.ID == MkbDiagnosesId)
                        {
                            newModel.VisitsByRoot.Add(new Tuple<Guid, int>(icd.ID, 1));
                            var ModelVisit = model.visitsByRoot.SingleOrDefault(t => t.Item1 == MkbDiagnosesId);
                            model.visitsByRoot[model.visitsByRoot.IndexOf(ModelVisit)] = Tuple.Create(ModelVisit.Item1, ModelVisit.Item2 + 1);
                        }
                        else
                        {
                            newModel.VisitsByRoot.Add(new Tuple<Guid, int>(icd.ID, 0));
                        }
                    }
                    recordModels.Add(newModel);
                }
            }
            model.records = recordModels;
            return model;
        }
    }
}
