using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Uebungsprojekt.Service
{
    public class CronTest : CronJobService
    {
        private readonly ILogger<CronTest> _logger;
        
        public CronTest(IScheduleConfig<CronTest> config, ILogger<CronTest> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
        }
        /// <summary>
        /// On Registering the CronJob
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Task</returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob starts.");
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// When called after the given time called every x time (Defined when cronjob is connected)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob is working.");
            
            //E-mail Notification Tes
            //is working but don't activate it.. mail flooding ;)
                //NotificationService mail_noti = new NotificationService();
                //mail_noti.SendEmail();
            
            
            //DO WHATEVERY YOU WANT HERE

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob is stopping.");
            return base.StopAsync(cancellationToken);
        }
        
        
    }
}