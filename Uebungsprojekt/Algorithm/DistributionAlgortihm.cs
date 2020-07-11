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

                        TimeSpan pufferhigh = new TimeSpan(0, 45, 0);
                        TimeSpan pufferlow = new TimeSpan(1, 45, 0);
                        TimeSpan pufferbetween = new TimeSpan(0, 15, 0);
                        TimeSpan bookingRealTimeSpan = ChargingTime.RealChargingTime(cc.charging_column_type_id, b);
                        // Charging column with more than 50kWh chargingpower
                        if (cc.charging_column_type_id.max_concurrent_charging >= 50)
                        {


                            // Booking is between the last booking and the next booking without conflicts
                            if (currentEndTime <= bookingStartTime && bookingEndTime <= nextStartTime) 
                            {
                                if (bookingStartTime - currentEndTime>= pufferhigh)
                                {
                                    if(nextStartTime - bookingEndTime >= pufferhigh)
                                    {
                                        cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.Accept();
                                        goto Exit;
                                    }

                                    if (nextStartTime - bookingEndTime <= pufferhigh)
                                    {
                                        bookingEndTime = nextStartTime;
                                        cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                        b.Accept();
                                        goto Exit;
                                    }

                                }
                                else if(bookingStartTime - currentEndTime < pufferhigh)
                                {
                                    if(nextStartTime - bookingEndTime >= pufferhigh)
                                    {
                                        bookingStartTime = currentEndTime;
                                        cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.Accept();
                                        goto Exit;

                                    }
                                    else if(nextStartTime - bookingEndTime < pufferhigh)
                                    {
                                        if(nextStartTime - bookingEndTime > bookingStartTime - currentEndTime)
                                        {
                                            bookingStartTime = currentEndTime;
                                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.Accept();
                                            goto Exit;
                                        }
                                        else
                                        {
                                            bookingEndTime = nextStartTime;
                                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                            b.Accept();
                                            goto Exit;
                                        }
                                    }

                                }
                                
                                
                            }
                            else if(currentEndTime > bookingStartTime && bookingEndTime <= nextStartTime)
                            {
                                if (bookingEndTime - currentEndTime < bookingRealTimeSpan)
                                {
                                    goto Exit;
                                }
                                else
                                {
                                    bookingStartTime = currentEndTime;
                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                    b.Accept();
                                    goto Exit;
                                }

                            }
                            else if(currentEndTime <= bookingStartTime && bookingEndTime > nextStartTime)
                            {
                                if(nextStartTime - bookingStartTime < bookingRealTimeSpan)
                                {
                                    goto Exit;
                                }
                                else
                                {
                                    bookingEndTime = nextStartTime;
                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime ));
                                    b.Accept();
                                    goto Exit;
                                }
                            }
                            else if(currentEndTime > bookingStartTime && bookingEndTime > nextStartTime)
                            {
                                if(nextStartTime - currentEndTime < bookingRealTimeSpan)
                                {
                                    goto Exit;
                                }
                                else
                                {
                                    bookingStartTime = currentEndTime;
                                    bookingEndTime = nextStartTime;
                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                    b.Accept();
                                    goto Exit;
                                }
                            }

                           
                            
                        }
                        else if (cc.charging_column_type_id.max_concurrent_charging < 50)
                        {
                            // Booking is between the last booking and the next booking without conflicts
                            if (currentEndTime <= bookingStartTime && bookingEndTime <= nextStartTime)
                            {
                                if (bookingStartTime - currentEndTime >= pufferlow)
                                {
                                    if (nextStartTime - bookingEndTime >= pufferlow)
                                    {
                                        cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.Accept();
                                        goto Exit;
                                    }

                                    if (nextStartTime - bookingEndTime <= pufferlow)
                                    {
                                        bookingEndTime = nextStartTime;
                                        cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                        b.Accept();
                                        goto Exit;
                                    }

                                }
                                else if (bookingStartTime - currentEndTime < pufferlow)
                                {
                                    if (nextStartTime - bookingEndTime >= pufferlow)
                                    {
                                        bookingStartTime = currentEndTime;
                                        cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.Accept();
                                        goto Exit;

                                    }
                                    else if (nextStartTime - bookingEndTime < pufferlow)
                                    {
                                        if (nextStartTime - bookingEndTime > bookingStartTime - currentEndTime)
                                        {
                                            bookingStartTime = currentEndTime;
                                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.Accept();
                                            goto Exit;
                                        }
                                        else
                                        {
                                            bookingEndTime = nextStartTime;
                                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                            b.Accept();
                                            goto Exit;
                                        }
                                    }

                                }


                            }
                            else if (currentEndTime > bookingStartTime && bookingEndTime <= nextStartTime)
                            {
                                if (bookingEndTime - currentEndTime < bookingRealTimeSpan)
                                {
                                    goto Exit;
                                }
                                else
                                {
                                    bookingStartTime = currentEndTime;
                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                    b.Accept();
                                    goto Exit;
                                }

                            }
                            else if (currentEndTime <= bookingStartTime && bookingEndTime > nextStartTime)
                            {
                                if (nextStartTime - bookingStartTime < bookingRealTimeSpan)
                                {
                                    goto Exit;
                                }
                                else
                                {
                                    bookingEndTime = nextStartTime;
                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                    b.Accept();
                                    goto Exit;
                                }
                            }
                            else if (currentEndTime > bookingStartTime && bookingEndTime > nextStartTime)
                            {
                                if (nextStartTime - currentEndTime < bookingRealTimeSpan)
                                {
                                    goto Exit;
                                }
                                else
                                {
                                    bookingStartTime = currentEndTime;
                                    bookingEndTime = nextStartTime;
                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                    b.Accept();
                                    goto Exit;
                                }
                            }
                        }
                       
                    Exit:;

                    }
                }
            }



        }

    }
}

