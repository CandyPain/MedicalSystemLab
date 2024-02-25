using MedLab3.Models;
using MedLab3.Models.Doctor;
using MedLab3.Models.ICD;
using MedLab3.Models.Inspection;
using MedLab3.Models.Patient;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MedLab3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<DoctorModel> Doctors { get; set; }

        public DbSet<InspectionModel> Inspections { get; set; }
        public DbSet<MkbRecord> MkbRecords { get; set; }
        public DbSet<TokenBan> TokensBan { get; set; }
        public DbSet<PatientModel> Patients { get; set; }
        public DbSet<SpecialityModel> Specialitys { get; set; }

        public DbSet<DiagnosisModel> DiagnosisModels { get; set; }
        public DbSet<InspectionConsultationModel> InspectionConsultationModel { get; set; }
        public DbSet<InspectionCommentModel> InspectionCommentModel { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InspectionModel>()
    .HasMany(i => i.Diagnoses)
    .WithOne()
    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}