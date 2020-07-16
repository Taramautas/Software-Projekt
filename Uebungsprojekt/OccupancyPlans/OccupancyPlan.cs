using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.OccupancyPlans
{
    public class OccupancyPlan
    {

        private int location_dao_id;
        private int charging_zone_dao_id;
        private int charging_column_dao_id;
        private int booking_dao_id;
        private IMemoryCache cache;

        public OccupancyPlan(int location_dao_id, int charging_zone_dao_id, int charging_column_dao_id, int booking_dao_id, IMemoryCache cache)
        {
            this.location_dao_id = location_dao_id;
            this.charging_zone_dao_id = charging_zone_dao_id;
            this.charging_column_dao_id = charging_column_dao_id;
            this.booking_dao_id = booking_dao_id;
            this.cache = cache;
        }
        
        public bool AcceptBooking(Booking booking)
        {
            return true;
        }


        /// <summary>
        /// Calculates current workload for each charging zone
        /// </summary>
        /// <returns>Dictionary<int, double> with Charging Zone ID as index and workload as value</returns>
        public Dictionary<int, double> GetCurrentWorkload(DateTime time)
        {
            ChargingZoneDao zone_dao = new ChargingZoneDaoImpl(cache);
            ChargingColumnDao column_dao = new ChargingColumnDaoImpl(cache);

            Dictionary<int, double> map_workload = new Dictionary<int, double>();
            Dictionary<int, int> map_available_columns = new Dictionary<int, int>();

            // Initiate the dictionary with zeros
            foreach (ChargingZone zone in zone_dao.GetAll(charging_zone_dao_id))
            {
                map_workload[zone.id] = 0;
                map_available_columns[zone.id] = 0;
            }

            // Count up variables
            foreach (ChargingColumn column in column_dao.GetAll(charging_column_dao_id))
            {
                if (column.busy)
                    map_workload[column.charging_zone.id] += 1;
                map_available_columns[column.charging_zone.id] += 1;
            }

            // Divide busy columns by total columns in order to get workload
            foreach(ChargingZone zone in zone_dao.GetAll(charging_zone_dao_id))
            {
                map_workload[zone.id] /= map_available_columns[zone.id];
            }

            return map_workload;
        }

        /// <summary>
        /// Adds an unaccepted booking to DAO
        /// </summary>
        /// <param name="booking">Unregistered booking wish</param>
        public void AddNewBooking(Booking booking)
        {
            BookingDao booking_dao = new BookingDaoImpl(cache);
            booking_dao.Create(
                booking.start_state_of_charge,
                booking.target_state_of_charge,
                booking.start_time,
                booking.end_time,
                booking.vehicle,
                booking.user,
                booking.location,
                booking_dao_id
                );
        }

        public int Distribute()
        {
            return 1;
        }

        public List<Location> GetAllLocations()
        {
            LocationDao location_dao = new LocationDaoImpl(cache);
            return location_dao.GetAll(location_dao_id);
        }

        public List<ChargingZone> GetAllChargingZones()
        {
            ChargingZoneDao charging_zone_dao = new ChargingZoneDaoImpl(cache);
            return charging_zone_dao.GetAll(charging_zone_dao_id);
        }

        public List<ChargingColumn> GetAllChargingColumns()
        {
            ChargingColumnDao charging_column_dao = new ChargingColumnDaoImpl(cache);
            return charging_column_dao.GetAll(charging_column_dao_id);
        }
    }
}
