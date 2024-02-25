using Quartz;
using System;
using System.Threading.Tasks;

namespace MedLab3.Models
{
    public class MailTask:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Проверка пропущенных приемов и отправка уведомлений");
        }
    }
}
