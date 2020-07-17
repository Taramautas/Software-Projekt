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
    public class DistributionService : CronJobService
     {
        private readonly ILogger<CronTest> _logger;
        private IMemoryCache _cache;

        public DistributionService(IScheduleConfig<CronTest> config, ILogger<CronTest> logger, IMemoryCache cache)
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
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// When called after the given time called every x time (Defined when cronjob is connected)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task DoWork(CancellationToken cancellationToken)
        {
           Console.WriteLine("JUHU");
           ChargingColumnDaoImpl _chargingcolumndao = new ChargingColumnDaoImpl(_cache);
           BookingDaoImpl _bookingDao = new BookingDaoImpl(_cache);
           int bookingdao_id = 0;
           int charcoldao_id = 0;
           
           Algorithm.DistributionAlgorithm.DistributionAlg(_chargingcolumndao, charcoldao_id, _bookingDao, new DateTime(2020,07,18), bookingdao_id);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob is stopping.");
            return base.StopAsync(cancellationToken);
        }
        
        
    }
}