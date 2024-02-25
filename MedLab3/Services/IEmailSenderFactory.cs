using MedLab3.Data;
using Quartz;

namespace MedLab3.Services
{
    public interface IEmailSenderFactory
    {
        IJob Create(AppDbContext context);
    }
}
