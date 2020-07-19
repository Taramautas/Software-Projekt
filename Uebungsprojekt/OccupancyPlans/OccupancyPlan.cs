using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
            int unsatisfiable_bookings = 0;
            int satisfiable_bookings = 0;
            
            BookingDao booking_dao = new BookingDaoImpl(cache);
            List<Booking> bookings = booking_dao.GetAll(booking_dao_id);
            List<Role> employee_roles = new List<Role> {Role.Assistant, Role.Employee, Role.Planner};

            List<Booking> vip_bookings = bookings.FindAll(b => b.user.role == Role.VIP).OrderBy(b => Guid.NewGuid()).ToList();
            List<Booking> guest_bookings = bookings.FindAll(b => b.user.role == Role.Guest).OrderBy(b => Guid.NewGuid()).ToList();
            List<Booking> employee_bookings = bookings.FindAll(b => employee_roles.Contains(b.user.role)).OrderBy(b => Guid.NewGuid()).ToList();
            
            
            foreach (Booking booking in vip_bookings)
            {
                if (!TryAcceptBooking(booking))
                    unsatisfiable_bookings += 1;
                else
                    satisfiable_bookings += 1;
            }
            
            foreach (Booking booking in guest_bookings)
            {
                if (!TryAcceptBooking(booking))
                    unsatisfiable_bookings += 1;
                else
                    satisfiable_bookings += 1;
            }
            
            foreach (Booking booking in employee_bookings)
            {
                if (!TryAcceptBooking(booking))
                    unsatisfiable_bookings += 1;
                else
                    satisfiable_bookings += 1;
            }

            Console.Out.WriteLine("Satisfiable: " + satisfiable_bookings);
            Console.Out.WriteLine("Unsatisfiable: " + unsatisfiable_bookings);
            return unsatisfiable_bookings;
        }

        private bool TryAcceptBooking(Booking booking)
        {
            List<ChargingColumn> columns = GetCompatibleChargingColumnsForBooking(booking).OrderBy(b => Guid.NewGuid()).ToList();
            List<Booking> bookings = GetAllBookings().FindAll(b => b.accepted);

            foreach (ChargingColumn column in columns)
            {
                Tuple<DateTime, DateTime, ConnectorType> booking_time = CheckIfColumnIsAvaliableForBooking(booking, column, bookings);
                if (booking_time != null)
                {
                    booking.accepted = true;
                    booking.charging_column = column;
                    booking.start_time = booking_time.Item1;
                    booking.end_time = booking_time.Item2;
                    booking.connector_type = booking_time.Item3;
                    return true;
                }
            }
            return false;
        }

        private List<ChargingColumn> GetCompatibleChargingColumnsForBooking(Booking booking)
        {
            List<ChargingColumn> columns = GetAllChargingColumns();
            List<ConnectorType> connectors = booking.vehicle.connector_types;

            return columns.FindAll(
                c =>
                {
                    foreach (Tuple<ConnectorType, int> connector_type in c.charging_column_type_id.connectors)
                    {
                        if (connectors.Contains(connector_type.Item1))
                            return true;
                    }
                    return false;
                });
        }
        
        private Tuple<DateTime, DateTime, ConnectorType> CheckIfColumnIsAvaliableForBooking(Booking booking, ChargingColumn column, List<Booking> bookings)
        {
            List<Booking> column_bookings = bookings.FindAll(b => b.charging_column.id == column.id);
            List<Tuple<DateTime, DateTime, ConnectorType>> booking_table = new List<Tuple<DateTime, DateTime, ConnectorType>>();
            DateTime booking_date = booking.start_time.Date;

            foreach (Booking b in column_bookings)
                booking_table.Add(new Tuple<DateTime, DateTime, ConnectorType>(b.start_time, b.end_time, b.connector_type));
            booking_table = booking_table.FindAll(tuple => tuple.Item1.Date == booking_date);

            if (booking_table.Count == 0)
            {
                foreach (Tuple<ConnectorType, int> connector in column.charging_column_type_id.connectors)
                {
                    if (booking.vehicle.connector_types.Contains(connector.Item1))
                    {
                        TimeSpan charging_time = GetNeededChargingTime(booking, connector.Item2);
                        return new Tuple<DateTime, DateTime, ConnectorType>(booking.start_time, booking.start_time + charging_time, connector.Item1);
                    }
                }
            }

            foreach (Tuple<ConnectorType, int> connector in column.charging_column_type_id.connectors)
            {
                if (booking.vehicle.connector_types.Contains(connector.Item1))
                {
                    TimeSpan charging_time = GetNeededChargingTime(booking, connector.Item2);
                    DateTime current_date_time = booking.start_time;
                    List<Tuple<DateTime, DateTime, ConnectorType>> connector_type_booking_table =
                        booking_table.FindAll(b => b.Item3 == connector.Item1);
                    
                    if (connector_type_booking_table.Count == 0)
                    {
                        return new Tuple<DateTime, DateTime, ConnectorType>(booking.start_time, booking.start_time + charging_time, connector.Item1);
                    }
                    
                    for (int i = 0; i < connector_type_booking_table.Count; i++)
                    {
                        if (connector_type_booking_table[i].Item1 - current_date_time > charging_time + new TimeSpan(0, 30, 0))
                        {
                            current_date_time += new TimeSpan(0, 15, 0);
                            return new Tuple<DateTime, DateTime, ConnectorType>(current_date_time, current_date_time + charging_time, connector.Item1);
                        }
                        current_date_time = connector_type_booking_table[i].Item1;
                    }

                    if (new TimeSpan(18, 0, 0) - connector_type_booking_table[^1].Item2.TimeOfDay > charging_time + new TimeSpan(0, 15, 0))
                    {
                        current_date_time = connector_type_booking_table[^1].Item2 + new TimeSpan(0, 15, 0);
                        return new Tuple<DateTime, DateTime, ConnectorType>(current_date_time, current_date_time + charging_time, connector.Item1);
                    }
                }
            }
            return null;
        }
        
        
        private TimeSpan GetNeededChargingTime(Booking booking, int charging_power)
        {
            int capacity = booking.vehicle.capacity;
            double start_charge = (capacity / 100) * booking.start_state_of_charge;
            double end_charge = (capacity / 100) * booking.target_state_of_charge;
            double charging_time = (end_charge - start_charge) / charging_power;
            int hours = (int)Math.Floor(charging_time);
            int minutes = (int) ((charging_time - hours) * 60);
            
            TimeSpan charging_timespan = new TimeSpan(hours, minutes, 0);
            TimeSpan booking_timespan = booking.end_time - booking.start_time;

            if (charging_timespan > booking_timespan)
                return booking_timespan;
                    
            return charging_timespan;
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
        
        public List<Booking> GetAllBookings()
        {
            BookingDao booking_dao = new BookingDaoImpl(cache);
            return booking_dao.GetAll(booking_dao_id);
        }
    }
}
