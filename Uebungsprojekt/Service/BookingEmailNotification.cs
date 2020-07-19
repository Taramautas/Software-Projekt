using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Service
{
    public class BookingEmailNotification : CronJobService
    {
        private readonly ILogger<BookingEmailNotification> _logger;
        private IMemoryCache _cache;

        public BookingEmailNotification(IScheduleConfig<BookingEmailNotification> config, ILogger<BookingEmailNotification> logger, IMemoryCache cache)
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
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// When called after the given time called every x time (Defined when cronjob is connected)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task DoWork(CancellationToken cancellationToken)
        {
           // _logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob is working.");
            //TODO: 0 or 1 as productive BookingDao?
            // TODO: daoImpl
            BookingDaoImpl tmp1 = new BookingDaoImpl(_cache);
            tmp1.GetAll(0);
            
            if(tmp1.GetAll(0).Count > 0)
            {
                Console.WriteLine("BookingCount: "+ tmp1.GetAll(0).Count);
                DateTime tmp = new DateTime();
                NotificationService mail_notification = new NotificationService();
                foreach (Booking book in tmp1.GetAll(0))
                {
                    try
                    {
                        tmp = book.start_time.Subtract(new TimeSpan(0,15,0));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                    //Console.WriteLine(tmp);
                    if (tmp.Year == DateTime.Now.Year && tmp.Month == DateTime.Now.Month && tmp.Day == DateTime.Now.Day && tmp.Hour == DateTime.Now.Hour && tmp.Minute == DateTime.Now.Minute)
                    {
                        String[] ignored_email_names = new[] {"admin@admin.de", "assistant@assistant.de", "vip@vip.de", "guest@guest.de", "user@user.de"};
                        if (!ignored_email_names.Contains(book.user.email))
                            mail_notification.SendEmail(book.user.email, book.user.name, book.location.city);

                        //ONly if accepted
                        //mail_notification.SendEmail(book.user.email, book.user.name, book.location.city, book.charging_column.charging_zone.name);
                    }
                }
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