using MedLab3.Data;
using MedLab3.Services;
using Quartz;

namespace MedLab3.Jobs
{
    public class EmailSenderFactory : IEmailSenderFactory
    {
        IJob IEmailSenderFactory.Create(AppDbContext context)
        {
            return null;
            //return new EmailSender(context);
        }
    }
}
