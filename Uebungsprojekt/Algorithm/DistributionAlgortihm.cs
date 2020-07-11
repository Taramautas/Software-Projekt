using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Algorithm;
using MathNet.Numerics.Optimization;

namespace Uebungsprojekt
{
    public class DistributionAlgorithm
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chargingcolumndao"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        public static void DistributionAlg(ChargingColumnDaoImpl chargingcolumndao, LocationDaoImpl locationDaoImpl, BookingDaoImpl bookingdao, Location location)
        {
            /// generate several list which are needed to run the Algorithm and eliminate the candidates which arent needed
            List<Booking> bookings = bookingdao.GetAll(0);
            List<Booking> unacceptedBookings = bookings.FindAll(HelpFunctions.FindUnacceptetBookings);

            List<Location> listofBookingLocations = unacceptedBookings.OfType<Location>().ToList();
            var connectorTypes = Enum.GetValues(typeof(ConnectorType)).Cast<ConnectorType>().ToList();
            List<ChargingColumn> result = new List<ChargingColumn>();
            List<ChargingColumn> overallresult = new List<ChargingColumn>();
            List<ChargingColumn> listofAllChargingColumn = chargingcolumndao.GetAll(0);

            List<ChargingColumn> listofChargingColumn = listofAllChargingColumn.FindAll(delegate (ChargingColumn cc)
            {
                foreach (Location loc in listofBookingLocations)
                {
                    if (cc.charging_zone.location.id == loc.id)
                    {
                        return true;
                    }
                
                }
                // Muss getestet werden keine Ahnung ob es passt
                return false;
            });
            List<ChargingColumn> listofBookingChargingColumn = listofChargingColumn.FindAll(delegate (ChargingColumn cc)
            {
                foreach (Booking b in unacceptedBookings)
                {
                    foreach (var columnconnector in cc.charging_column_type_id.connectors)
                    {
                        foreach (var connector in b.vehicle.connector_types)
                        {
                            if (columnconnector == connector)
                            {
                                return true;
                                

                            } 
                        }
                      
                    }

                }
                return false;
            });

            // all needed charging columns together in one list
            foreach (var connectorType in connectorTypes)
            {
                if (connectorType == ConnectorType.Schuko_Socket)
                {
                    result = listofChargingColumn.FindAll(HelpFunctions.FindSchuko_Socket);
                    overallresult.AddRange(result);
                }
                if (connectorType == ConnectorType.Type_1_Plug)
                {
                    result = listofChargingColumn.FindAll(HelpFunctions.FindType_1_Plug);
                    overallresult.AddRange(result);
                }
                if (connectorType == ConnectorType.Type_2_Plug)
                {
                    result = listofChargingColumn.FindAll(HelpFunctions.FindType_2_Plug);
                    overallresult.AddRange(result);
                }
                if (connectorType == ConnectorType.CHAdeMO_Plug)
                {
                    result = listofChargingColumn.FindAll(HelpFunctions.FindCHAdeMO_Plug);
                    overallresult.AddRange(result);
                }
                if (connectorType == ConnectorType.Tesla_Supercharger)
                {
                    result = listofChargingColumn.FindAll(HelpFunctions.FindTesla_Supercharger);
                    overallresult.AddRange(result);
                }
                if (connectorType == ConnectorType.CCS_Combo_2_Plug)
                {
                    result = listofChargingColumn.FindAll(HelpFunctions.FindCCS_Combo_2_Plug);
                    overallresult.AddRange(result);
                }
            }
            //the Algorithm which calculate if between all bookings is enough space or if they are in a row
            //Fehlende if-Abfragen ob die Zeit nicht zu weit ausseinder liegt und das für die jeweiligen ChargingColumns
            foreach (ChargingColumn cc in overallresult)
            {
                foreach (Booking b in unacceptedBookings)
                {
                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                    {

                        Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                        DateTime currentStartTime = tuple.Item1;
                        DateTime currentEndTime = tuple.Item2 + new TimeSpan(0, 15, 0);

                        DateTime nextStartTime = next.Item1;
                        DateTime nextEndTime = next.Item2 + new TimeSpan(0, 15, 0);

                        DateTime bookingStartTime = b.start_time;
                        DateTime bookingEndTime = b.end_time + new TimeSpan(0, 15, 0);

                        TimeSpan bookingRealTimeSpan = ChargingTime.RealChargingTime(cc.charging_column_type_id, b);

                        if (currentEndTime <= bookingStartTime && bookingEndTime < nextStartTime)
                        {
                            if(bookingStartTime - currentEndTime >= new TimeSpan(1,0,0) && cc.charging_column_type_id.max_concurrent_charging > 50) 
                            {
                                if (nextStartTime - bookingEndTime >= new TimeSpan(1, 0, 0))
                                {
                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + new TimeSpan(0, 15, 0)));
                                    b.Accept();
                                    goto Exit;
                                }
                                else if(nextStartTime - bookingEndTime < new TimeSpan(1, 0, 0))
                                {
                                    bookingEndTime = nextStartTime;
                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                    b.Accept();
                                    goto Exit;
                                }
                                
                            }else if(bookingStartTime - currentEndTime < new TimeSpan(1,0,0) )
                            if(bookingStartTime - currentEndTime < new TimeSpan(1,0,0) && cc.charging_column_type_id.max_concurrent_charging < 50)
                            {
                                bookingStartTime = currentEndTime;
                                cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan));
                                b.Accept();
                                goto Exit;
                            }
                            
                        }
                        else if (currentEndTime <= bookingStartTime && bookingEndTime >= nextStartTime)
                        {
                            bookingEndTime = bookingEndTime - (bookingEndTime - nextStartTime) - new TimeSpan(0, 15, 0);
                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + new TimeSpan(0, 15, 0)));
                            break;
                        }
                        else if (currentEndTime > bookingStartTime && bookingEndTime < nextStartTime)
                        {
                            bookingStartTime = bookingStartTime + (currentEndTime - bookingStartTime);
                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + new TimeSpan(0, 15, 0)));
                            break;
                        }
                        else if (currentEndTime > bookingStartTime && bookingEndTime >= nextStartTime)
                        {
                            bookingStartTime = bookingStartTime + (currentEndTime - bookingStartTime);
                            bookingEndTime = bookingEndTime - (bookingEndTime - nextStartTime) - new TimeSpan(0, 15, 0);
                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + new TimeSpan(0, 15, 0)));
                            break;
                        }
                    Exit:;

                    }
                }
            }



        }

    }
}

