using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Service
{
    public class CronTest : CronJobService
    {
        private readonly ILogger<CronTest> _logger;
        private IMemoryCache _cache;
        private List<int> notifcationList;

        public CronTest(IScheduleConfig<CronTest> config, ILogger<CronTest> logger, IMemoryCache cache)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
             _cache = cache;
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
            //TODO: Necessary??
            notifcationList = new List<int>();
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
            //TODO: 0 or 1 as productive BookingDao?
            // TODO: daoImpl
            BookingDaoImpl tmp1 = new BookingDaoImpl(_cache);
            tmp1.GetAll(0);
            
            if(_cache.TryGetValue("0" + "CreateBooking", out List<Booking> createdBookings))
            {
                DateTime tmp = new DateTime();
                NotificationService mail_notification = new NotificationService();
                foreach (Booking book in createdBookings)
                {
                    tmp = book.start_time.Subtract(new TimeSpan(0,15,0));
                    if (tmp == DateTime.Now)
                    {
                        //mail_notification.SendEmail(book.user.email, book.user.name);
                    }
                }
                //E-mail Notification Tes
                //is working but don't activate it.. mail flooding ;)
                //NotificationService mail_noti = new NotificationService();
                //mail_noti.SendEmail();
               
            
                //DO WHATEVERY YOU WANT HERE
            }
            
            NotificationService mail_noti = new NotificationService();
           // mail_noti.SendEmail("crapspams@icloud.com", "Dominik");
           // mail_noti.SendEmail("sebastian.rossi@student.uni-augsburg.de", "Rossi");

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob is stopping.");
            return base.StopAsync(cancellationToken);
        }
        
        
    }
}