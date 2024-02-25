using Quartz.Impl;
using Quartz;
using MedLab3.Data;
using MedLab3.Services;

namespace MedLab3.Jobs
{
        public class EmailScheduler
        {
        /*
            public static async Task Start()
            {
            try
            {
                Console.WriteLine("Start");
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();

                IJobDetail job = JobBuilder.Create<EmailSender>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                    .StartNow()                            // запуск сразу после начала выполнения
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInSeconds(10)          // через 1 минуту
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
                Console.WriteLine("Start2");
                await scheduler.Start();
                var executingJobs = await scheduler.GetCurrentlyExecutingJobs();
                Console.WriteLine(executingJobs.Count);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        */
        }
}
