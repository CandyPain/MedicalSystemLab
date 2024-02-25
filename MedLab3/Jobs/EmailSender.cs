using Quartz;
using System.Net.Mail;
using System.Net;
using MedLab3.Data;
using MedLab3.Services;
using MedLab3.Models;
using Microsoft.EntityFrameworkCore;
using MedLab3.Models.Inspection;

namespace MedLab3.Jobs
{
    public class EmailSender : IEmailSender
    {
        private readonly AppDbContext _context;
        public EmailSender(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendEmailAsync()
        {
            Console.WriteLine("SendMail");
            IQueryable<InspectionModel> inspections = _context.Inspections;
            var doctors = _context.Doctors.ToList();
            var patients = _context.Patients.ToList();
            inspections = inspections.Where(i=>i.NextVisitDate != null);
            foreach(var item in inspections)
            {
                if (item.NextVisitDate != null && item.NextVisitDate.Value.AddMinutes(3) <DateTime.Now && item.HasMail == false)
                {
                    item.HasMail = true;
                    var from = "candypainmail@mail.ru";
                    var pass = "mailpassword7645";
                    var email = doctors.SingleOrDefault(d => d.Id == item.DoctorId)?.Email;

                    if (email != null)
                    {
                        SmtpClient client = new SmtpClient("localhost", 1025);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential(from, pass);
                        client.EnableSsl = false;

                        var mail = new MailMessage(from, email);
                        mail.Subject = "Уведомление";
                        mail.Body = $"Пациент {patients.SingleOrDefault(p => p.Id == item.PatientId)?.Name} Пропустил прием";
                        mail.IsBodyHtml = true;

                        await client.SendMailAsync(mail);
                    }
                }
            }
            _context.SaveChanges();
        }
    }
}
