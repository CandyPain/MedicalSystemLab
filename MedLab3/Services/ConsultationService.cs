using MedLab3.Data;
using MedLab3.Models;
using MedLab3.Models.Doctor;
using MedLab3.Models.Enums;
using MedLab3.Models.ICD;
using MedLab3.Models.Inspection;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MedLab3.Services
{
    public class ConsultationService:IConsultationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        public ConsultationService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }

        public async Task<Guid> CreateComment(Guid DoctorId,Guid Id, CommentCreateModel commentCreateModel)
        {
            DoctorModel doc = _context.Doctors.SingleOrDefault(d => d.Id == DoctorId);
            InspectionConsultationModel cons = await _context.InspectionConsultationModel.Include(s=>s.Speciality).SingleOrDefaultAsync(c => c.Id == Id);
            if(cons == null)
            {
                throw new DirectoryNotFoundException("Consultation Not Found");
            }
            if(doc.Speciality != cons.Speciality.Id && _context.Inspections.SingleOrDefault(i=>i.Id == cons.InspectionId).DoctorId != doc.Id)
            {
                throw new RankException("Forbidden");
            }
            if(_context.InspectionCommentModel.SingleOrDefault(c=>c.Id==commentCreateModel.ParentId && c.ConsultationId == cons.Id) == null)
            {
                throw new DirectoryNotFoundException("Parent comment Not Found");
            }
            Guid id = Guid.NewGuid();
            _context.InspectionCommentModel.Add(new InspectionCommentModel
            {
                Content = commentCreateModel.Content,
                ParentId = commentCreateModel.ParentId,
                CreateTime = DateTime.Now,
                Id = id,
                ModifyTime = null,
                Author = doc,
                ConsultationId = cons.Id,
                
            });
            cons.CommentsNumber++;
            _context.SaveChanges();
            return id;
        }

        async Task IConsultationService.EditComment(Guid DoctorId, Guid Id, InspectionCommentCreateModel content)
        {
            DoctorModel doc = _context.Doctors.SingleOrDefault(d => d.Id == DoctorId);
            InspectionCommentModel current = await  _context.InspectionCommentModel.Include(d=>d.Author).SingleOrDefaultAsync(c => c.Id == Id);
            if(current == null)
            {
                throw new DirectoryNotFoundException("Not Found");
            }
            if(doc.Id != current.Author.Id)
            {
                throw new RankException("Forbidden");
            }
            current.Content = content.Content;
            current.ModifyTime = DateTime.Now;
            _context.SaveChanges();
        }

        async Task<ConsultationModel> IConsultationService.GetConsultation(Guid Id)
        {
            IQueryable<InspectionCommentModel> comments = _context.InspectionCommentModel.Include(d=>d.Author);
            InspectionConsultationModel cons = await _context.InspectionConsultationModel.Include(i=>i.Speciality).Include(i=>i.RootComment).ThenInclude(d=>d.Author).SingleOrDefaultAsync(c => c.Id == Id);
            if(cons == null)
            {
                throw new DirectoryNotFoundException("Not Found");
            }
            List<CommentModel> CommentList = new List<CommentModel>();
            InspectionCommentModel current = cons.RootComment;
            Queue<InspectionCommentModel> queue = new Queue<InspectionCommentModel>();
            queue.Enqueue(current);
            while(queue.Count() > 0)
            {
                CommentList.Add(new CommentModel
                {
                    AuthorId = current.Id,
                    Author = current.Author.Name,
                    Content = current.Content,
                    CreateTime = current.CreateTime,
                    Id = current.Id,
                    ModifiedDate = current.ModifyTime,
                    ParentId = current.ParentId
                });
                List<InspectionCommentModel> next = new List<InspectionCommentModel>();
                next = comments.Where(c => c.ParentId == current.Id).ToList();
                foreach(var item in next)
                {
                    queue.Enqueue(item);
                }
                queue.Dequeue();
                if(queue.Count()>0)
                current = queue.Peek();
            }
            ConsultationModel model = new ConsultationModel
            {
                Id = cons.Id,
                CreateTime = cons.CreateTime,
                inspectionId = cons.InspectionId,
                Speciality = cons.Speciality,
                Comments = CommentList
            };
            return model;
        }

        async Task<InspectionPagedListModel> IConsultationService.GetInspections(Guid DoctorId, bool? grouped, List<Guid>? isdRoots, int page = 1, int size = 5)
        {
            if (page <= 0 || size <= 0)
            {
                throw new ArgumentException("Bad page format");
            }
            IQueryable<InspectionModel> inspections = _context.Inspections.Include(d=>d.Doctor).Include(c=>c.Consultations).ThenInclude(c=>c.Speciality);
            IQueryable<MkbRecord> records = _context.MkbRecords;
            if(!isdRoots.All(r=>records.SingleOrDefault(m=>m.ID==r)!= null))
            {
                throw new ArgumentException("Bad ICD10 Id");
            }
            DoctorModel doc = _context.Doctors.SingleOrDefault(d=>d.Id==DoctorId);
            IQueryable<DiagnosisModel> diagnosisModels = _context.DiagnosisModels;
            inspections = inspections.Where(i => i.Consultations.Any(c => c.Speciality.Id == doc.Speciality));
            if (isdRoots != null && isdRoots.Count != 0)
                inspections = inspections.Where(i => diagnosisModels.Where(m => m.inspectionId == i.Id).Any(d => d.Type == DiagnosisType.Main && isdRoots.Contains(d.mkbId)));
            if(grouped == true)
            {
                inspections = inspections.Where(i=>i.PreviousInspectionId == null);
            }
            int skip = (page - 1) * size;
            int Count1 = inspections.Count();
            inspections = inspections.Skip(skip).Take(size);
            InspectionPagedListModel model = new InspectionPagedListModel
            {
                inspections = inspections.Select(item => new InspectionPreviewModel
                {
                    Conclusion = item.Conclusion,
                    CreateTime = item.CreateTime,
                    Date = item.Date,
                    Diagnosis = diagnosisModels.SingleOrDefault(d => d.inspectionId == item.Id && d.Type == DiagnosisType.Main),
                    Doctor = item.Doctor.Name,
                    DoctorId = item.DoctorId,
                    Id = item.Id,
                    Patient = item.Patient.Name,
                    PatientId = item.PatientId,
                    PreviousId = item.PreviousInspectionId,
                    HasChain = item.PreviousInspectionId == null ? true : false,
                    HasNested = inspections.SingleOrDefault(i => i.PreviousInspectionId == item.Id) != null ? true : false
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
    }
}
