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
        public static void DistributionAlg(ChargingColumnDaoImpl chargingcolumndao, BookingDaoImpl bookingdao)
        {
            /// generate several list which are needed to run the Algorithm and eliminate the candidates which arent needed
            List<Booking> bookings = bookingdao.GetAll(0);

            //list of all unaccepted Bookings
            List<Booking> unacceptedBookings = bookings.FindAll(HelpFunctions.FindUnacceptedBookings).FindAll(delegate (Booking b)
            {
                return b.user.role == Role.VIP;
            });

            unacceptedBookings.AddRange(bookings.FindAll(HelpFunctions.FindUnacceptedBookings).FindAll(delegate (Booking b)
            {
                return b.user.role == Role.Employee;
            }));

            unacceptedBookings.AddRange(bookings.FindAll(HelpFunctions.FindUnacceptedBookings).FindAll(delegate (Booking b)
            {
                return b.user.role == Role.Guest;
            }));

            //list of all needed locations which are extracted by bookings
            List<Location> listofBookingLocations = new List<Location>();
            foreach(Booking b in unacceptedBookings)
            {
                if (listofBookingLocations.Find(x => x.id == b.location.id) == null)
                {
                    listofBookingLocations.Add(b.location);
                }
                
            }
            

            //list of all connector typs
            var connectorTypes = Enum.GetValues(typeof(ConnectorType)).Cast<ConnectorType>().ToList();

            //help list
            List<ChargingColumn> result = new List<ChargingColumn>();

            //
            List<ChargingColumn> overallresult = new List<ChargingColumn>();

            //list of all chargingcolumns
            List<ChargingColumn> listofAllChargingColumn = chargingcolumndao.GetAll(0);

            //list of needed charging columns filtered by Location
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

            //list of all charging columns filtered by location and needed connectortyps
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

            //alternative lösung zu oben
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

            TimeSpan pufferhigh = new TimeSpan(0, 45, 0);
            TimeSpan pufferlow = new TimeSpan(1, 45, 0);
            TimeSpan pufferbetween = new TimeSpan(0, 15, 0);

            //the Algorithm which calculate if between all bookings is enough space or if they are in a row
            foreach (Booking b in unacceptedBookings)
            {

                DateTime bookingStartTime = b.start_time;
                DateTime bookingEndTime = b.end_time + pufferbetween;

                foreach (ChargingColumn cc in listofBookingChargingColumn)
                {
                    TimeSpan bookingRealTimeSpan = ChargingTime.RealChargingTime(cc.charging_column_type_id, b);

                    if (HelpFunctions.ConnectorCompare(cc, b))
                    {
                        if (cc.list.Count == 0)
                        {
                            cc.list.Insert(0, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                            b.charging_column = cc;
                            b.Accept();
                            goto Exit;
                        }

                        if (cc.list.Count == 1)
                        {
                            Tuple<DateTime, DateTime> tuple1 = cc.list[0];
                            DateTime currentStartTime1 = tuple1.Item1;
                            DateTime currentEndTime1 = tuple1.Item2 + pufferbetween;

                            if (cc.charging_column_type_id.max_concurrent_charging >= 50)
                            {
                                // Booking is between the last booking and the next booking without conflicts
                                if (currentEndTime1 <= bookingStartTime)
                                {
                                    if (bookingStartTime - currentEndTime1 >= pufferhigh)
                                    {

                                        cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.charging_column = cc;
                                        b.Accept();
                                        goto Exit;


                                    }
                                    else if (bookingStartTime - currentEndTime1 < pufferhigh)
                                    {

                                        bookingStartTime = currentEndTime1;
                                        cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.Accept();
                                        goto Exit;

                                    }


                                }

                                else if (currentStartTime1 >= bookingEndTime)
                                {
                                    if (currentStartTime1 - bookingEndTime >= pufferhigh)
                                    {

                                        cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.charging_column = cc;
                                        b.Accept();
                                        goto Exit;
                                    }

                                    else if (currentStartTime1 - bookingEndTime < pufferhigh)
                                    {

                                        bookingEndTime = currentStartTime1;
                                        cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime, bookingEndTime - bookingRealTimeSpan + pufferbetween));
                                        b.charging_column = cc;
                                        b.Accept();
                                        goto Exit;

                                    }

                                }

                                else
                                {
                                    if (bookingStartTime < currentStartTime1)
                                    {
                                        if (currentStartTime1 - bookingStartTime < bookingRealTimeSpan)
                                        {
                                            goto Exit;
                                        }
                                        else
                                        {
                                            bookingEndTime = currentStartTime1;
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                            b.charging_column = cc;
                                            b.Accept();
                                            goto Exit;
                                        }
                                    }

                                    else if (bookingEndTime > currentEndTime1)
                                    {
                                        if (bookingEndTime - currentEndTime1 < bookingRealTimeSpan)
                                        {
                                            goto Exit;
                                        }

                                        else
                                        {
                                            bookingStartTime = currentEndTime1;
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingEndTime + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            goto Exit;
                                        }
                                    }
                                }
                            }
                            else if (cc.charging_column_type_id.max_concurrent_charging < 50)
                            {
                                // Booking is between the last booking and the next booking without conflicts
                                if (currentEndTime1 <= bookingStartTime)
                                {
                                    if (bookingStartTime - currentEndTime1 >= pufferhigh)
                                    {
                                        cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.charging_column = cc;
                                        b.Accept();
                                        goto Exit;
                                    }
                                    else if (bookingStartTime - currentEndTime1 < pufferhigh)
                                    {
                                        bookingStartTime = currentEndTime1;
                                        cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.charging_column = cc;
                                        b.Accept();
                                        goto Exit;
                                    }


                                }

                                else if (currentStartTime1 >= bookingEndTime)
                                {
                                    if (currentStartTime1 - bookingEndTime >= pufferhigh)
                                    {
                                        cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                        b.charging_column = cc;
                                        b.charging_column = cc;
                                        b.Accept();
                                        goto Exit;
                                    }
                                    else if (currentStartTime1 - bookingEndTime < pufferhigh)
                                    {
                                        bookingEndTime = currentStartTime1;
                                        cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime, bookingEndTime - bookingRealTimeSpan + pufferbetween));
                                        b.charging_column = cc;
                                        b.Accept();
                                        goto Exit;

                                    }

                                }

                                else
                                {
                                    if (bookingStartTime < currentStartTime1)
                                    {
                                        if (currentStartTime1 - bookingStartTime < bookingRealTimeSpan)
                                        {
                                            goto Exit;
                                        }

                                        else
                                        {
                                            bookingEndTime = currentStartTime1;
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                            b.charging_column = cc;
                                            b.Accept();
                                            goto Exit;
                                        }
                                    }

                                    else if (bookingEndTime > currentEndTime1)
                                    {
                                        if (bookingEndTime - currentEndTime1 < bookingRealTimeSpan)
                                        {
                                            goto Exit;
                                        }

                                        else
                                        {
                                            bookingStartTime = currentEndTime1;
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingEndTime + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            goto Exit;
                                        }
                                    }
                                }
                            }
                        }

                        else
                        {
                            foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                            {

                                Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                DateTime currentStartTime = tuple.Item1;
                                DateTime currentEndTime = tuple.Item2 + pufferbetween;

                                DateTime nextStartTime = next.Item1;
                                DateTime nextEndTime = next.Item2 + pufferbetween;

                                // Charging column with more than 50kWh chargingpower
                                if (cc.charging_column_type_id.max_concurrent_charging >= 50)
                                {


                                    // Booking is between the last booking and the next booking without conflicts
                                    if (currentEndTime <= bookingStartTime && bookingEndTime <= nextStartTime)
                                    {
                                        if (bookingStartTime - currentEndTime >= pufferhigh)
                                        {
                                            if (nextStartTime - bookingEndTime >= pufferhigh)
                                            {
                                                cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                b.charging_column = cc;
                                                b.Accept();
                                                goto Exit;
                                            }

                                            if (nextStartTime - bookingEndTime <= pufferhigh)
                                            {
                                                bookingEndTime = nextStartTime;
                                                cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                b.charging_column = cc;
                                                b.Accept();
                                                goto Exit;
                                            }

                                        }

                                        else if (bookingStartTime - currentEndTime < pufferhigh)
                                        {
                                            if (nextStartTime - bookingEndTime >= pufferhigh)
                                            {
                                                bookingStartTime = currentEndTime;
                                                cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                b.charging_column = cc;
                                                b.Accept();
                                                goto Exit;

                                            }
                                            else if (nextStartTime - bookingEndTime < pufferhigh)
                                            {
                                                if (nextStartTime - bookingEndTime > bookingStartTime - currentEndTime)
                                                {
                                                    bookingStartTime = currentEndTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    goto Exit;
                                                }
                                                else
                                                {
                                                    bookingEndTime = nextStartTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                    b.charging_column = cc;
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
                                            b.charging_column = cc;
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
                                            b.charging_column = cc;
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
                                            b.charging_column = cc;
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
                                                b.charging_column = cc;
                                                b.Accept();
                                                goto Exit;
                                            }

                                            if (nextStartTime - bookingEndTime <= pufferlow)
                                            {
                                                bookingEndTime = nextStartTime;
                                                cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                b.charging_column = cc;
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
                                                b.charging_column = cc;
                                                b.Accept();
                                                goto Exit;

                                            }

                                            else if (nextStartTime - bookingEndTime < pufferlow)
                                            {
                                                if (nextStartTime - bookingEndTime > bookingStartTime - currentEndTime)
                                                {
                                                    bookingStartTime = currentEndTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    goto Exit;
                                                }

                                                else
                                                {
                                                    bookingEndTime = nextStartTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                    b.charging_column = cc;
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
                                            b.charging_column = cc;
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
                                            b.charging_column = cc;

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
                                            b.charging_column = cc;
                                            b.Accept();
                                            goto Exit;
                                        }
                                    }
                                }
                            }
                        }
                    }
                
                }
                Exit:;
            }
        }

    }
}


