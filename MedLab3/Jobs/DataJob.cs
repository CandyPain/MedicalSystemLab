using MedLab3.Services;
using Quartz;

namespace MedLab3.Jobs
{
    public class DataJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public DataJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var emailsender = scope.ServiceProvider.GetService<IEmailSender>();

                await emailsender.SendEmailAsync();
            }
        }
    }
}
