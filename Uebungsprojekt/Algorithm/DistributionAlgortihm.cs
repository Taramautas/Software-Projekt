using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Algorithm;
using MathNet.Numerics.Optimization;
/*
namespace Uebungsprojekt.Algorithm
{
    public class DistributionAlgorithm
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chargingcolumndao"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        public static void DistributionAlg(ChargingColumnDaoImpl chargingcolumndao, int chargingcolumndaoID, BookingDaoImpl bookingdao, int bookindaoID)
        {
            /// generate several list which are needed to run the Algorithm and eliminate the candidates which arent needed
            List<Booking> bookings = bookingdao.GetAll(bookindaoID);
            Console.WriteLine("allbookings");
            foreach (Booking b in bookings)
            {
                Console.WriteLine(b.id);
            }
            Console.WriteLine();

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
            Console.WriteLine("unaccapted");
            foreach (Booking b in unacceptedBookings)
            {
                Console.WriteLine(b.id);
            }
            Console.WriteLine();
            //list of all needed locations which are extracted by bookings
            List<Location> listofBookingLocations = new List<Location>();
            foreach (Booking b in unacceptedBookings)
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
            List<ChargingColumn> listofAllChargingColumn = chargingcolumndao.GetAll(chargingcolumndaoID);

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

            foreach (ChargingColumn cc in listofBookingChargingColumn)
            {
                Console.WriteLine(cc.id + "\n");
            }

            TimeSpan pufferhigh = new TimeSpan(0, 45, 0);
            TimeSpan pufferlow = new TimeSpan(1, 45, 0);
            TimeSpan pufferbetween = new TimeSpan(0, 15, 0);

            //the Algorithm which calculate if between all bookings is enough space or if they are in a row
            int bookingindex = 0;
            foreach (Booking b in unacceptedBookings)
            {

                DateTime bookingStartTime = b.start_time;
                DateTime bookingEndTime = b.end_time + pufferbetween;

                ++bookingindex;
                foreach (ChargingColumn cc in listofBookingChargingColumn)
                {

                    TimeSpan bookingRealTimeSpan = ChargingTime.RealChargingTime(cc.charging_column_type_id, b);

                    Console.WriteLine(bookingindex);
                    if (cc.charging_zone.location == b.location)
                    {
                        if (HelpFunctions.ConnectorCompare(cc, b))
                        {
                            Console.WriteLine("==0");

                            if (cc.list.Count == 0)
                            {
                                cc.list.Insert(0, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                b.charging_column = cc;
                                b.Accept();
                                b.start_time = bookingStartTime;
                                b.end_time = bookingStartTime + bookingRealTimeSpan;
                                goto Exit;
                            }

                            Console.WriteLine("==1");

                            if (cc.list.Count == 1)
                            {
                                Tuple<DateTime, DateTime> tuple1 = cc.list[0];
                                DateTime currentStartTime1 = tuple1.Item1;
                                DateTime currentEndTime1 = tuple1.Item2;

                                if (cc.charging_column_type_id.max_concurrent_charging >= 50)
                                {
                                    // Booking is between the last booking and the next booking without conflicts
                                    if (currentEndTime1 <= bookingStartTime)
                                    {
                                        Console.WriteLine(01.1111);
                                        if (bookingStartTime - currentEndTime1 >= pufferhigh)
                                        {

                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;


                                        }
                                        else if (bookingStartTime - currentEndTime1 < pufferhigh)
                                        {

                                            bookingStartTime = currentEndTime1;
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            b.charging_column = cc;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                            goto Exit;

                                        }


                                    }

                                    if (currentStartTime1 >= bookingEndTime)
                                    {
                                        Console.WriteLine(01.2111);
                                        // Booking is between the last booking and the next booking without conflicts



                                        if (currentStartTime1 - bookingEndTime >= pufferhigh)
                                        {
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }
                                        else if (currentStartTime1 - bookingEndTime < pufferhigh)
                                        {
                                            bookingEndTime = currentStartTime1;
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                            b.end_time = bookingEndTime - pufferbetween;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;

                                        }

                                    }

                                    else
                                    {
                                        Console.WriteLine("meepmeep");
                                        if (bookingStartTime < currentStartTime1)
                                        {
                                            if (currentStartTime1 - bookingStartTime < bookingRealTimeSpan)
                                            {
                                                Console.WriteLine("Exit");
                                                goto Exit;
                                            }

                                            else
                                            {
                                                Console.WriteLine("here");
                                                bookingEndTime = currentStartTime1;
                                                cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                b.charging_column = cc;
                                                b.Accept();
                                                b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                b.end_time = bookingEndTime - pufferbetween;
                                                cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
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
                                                cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                b.charging_column = cc;
                                                b.Accept();
                                                b.start_time = bookingStartTime;
                                                b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                                goto Exit;
                                            }
                                        }
                                    }


                                    if (currentEndTime1 > bookingStartTime)
                                    {
                                        bookingStartTime = currentEndTime1;
                                        if (bookingEndTime - bookingStartTime > bookingRealTimeSpan)
                                        {
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }


                                    }


                                }
                                else if (cc.charging_column_type_id.max_concurrent_charging < 50)
                                {
                                    // Booking is between the last booking and the next booking without conflicts
                                    if (currentEndTime1 <= bookingStartTime)
                                    {
                                        if (bookingStartTime - currentEndTime1 >= pufferlow)
                                        {
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }
                                        else if (bookingStartTime - currentEndTime1 < pufferlow)
                                        {
                                            bookingStartTime = currentEndTime1;
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }


                                    }

                                    else if (currentStartTime1 >= bookingEndTime)
                                    {
                                        if (currentStartTime1 - bookingEndTime >= pufferlow)
                                        {
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }
                                        else if (currentStartTime1 - bookingEndTime < pufferlow)
                                        {
                                            bookingEndTime = currentStartTime1;
                                            cc.list.Insert(cc.list.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime, bookingEndTime - bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                            b.end_time = bookingEndTime - pufferbetween;
                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
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
                                                b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                b.end_time = bookingEndTime - pufferbetween;
                                                cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
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
                                                b.start_time = bookingStartTime;
                                                b.end_time = bookingEndTime;
                                                cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                                goto Exit;
                                            }
                                        }
                                    }
                                }



                            }
                            
                            else
                            {
                                Console.WriteLine("meep");

                                if (cc.charging_column_type_id.max_concurrent_charging >= 50)
                                {

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) == 0)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;
                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.1);
                                            if (bookingEndTime < currentStartTime && cc.list.IndexOf(tuple) == 1)
                                            {
                                                if (currentStartTime - bookingEndTime >= pufferhigh)
                                                {
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (currentStartTime - bookingEndTime < pufferhigh)
                                                {
                                                    bookingEndTime = currentStartTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }
                                        }
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) < cc.list.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;
                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;

                                            Console.WriteLine(1.2);

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
                                                        b.start_time = bookingStartTime;
                                                        b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                        cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                        goto Exit;
                                                    }

                                                    if (nextStartTime - bookingEndTime <= pufferhigh)
                                                    {
                                                        bookingEndTime = nextStartTime;
                                                        cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                        b.charging_column = cc;
                                                        b.Accept();
                                                        b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                        b.end_time = bookingEndTime - pufferbetween;
                                                        cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

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
                                                        b.start_time = bookingStartTime;
                                                        b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                        cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

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
                                                            b.start_time = bookingStartTime;
                                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                            goto Exit;
                                                        }
                                                        else
                                                        {
                                                            bookingEndTime = nextStartTime;
                                                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                            b.charging_column = cc;
                                                            b.Accept();
                                                            b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                            b.end_time = bookingEndTime - pufferbetween;
                                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                            goto Exit;
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) < cc.list.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.3);

                                            if (currentEndTime > bookingStartTime && bookingEndTime <= nextStartTime)
                                            {
                                                if (bookingEndTime - currentEndTime < bookingRealTimeSpan)
                                                {
                                                    goto ExitTuple;
                                                }

                                                else
                                                {
                                                    bookingStartTime = currentEndTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }
                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) < cc.list.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.4);
                                            if (currentEndTime > bookingStartTime && bookingEndTime > nextStartTime)
                                            {
                                                if (nextStartTime - pufferbetween - currentEndTime < bookingRealTimeSpan)
                                                {
                                                    goto ExitTuple;
                                                }

                                                else
                                                {
                                                    bookingStartTime = currentEndTime;

                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }

                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) < cc.list.Count  -1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.5);
                                            if (currentEndTime <= bookingStartTime && bookingEndTime > nextStartTime)
                                            {
                                                if ((nextStartTime > bookingStartTime && nextStartTime - bookingStartTime < bookingRealTimeSpan) && (bookingEndTime > nextEndTime && bookingEndTime - nextEndTime < bookingRealTimeSpan))
                                                {
                                                    Console.WriteLine("Exit");
                                                    goto ExitTuple;
                                                }
                                                if (nextStartTime > bookingStartTime && nextStartTime - bookingStartTime >= bookingRealTimeSpan) {

                                                    Console.WriteLine(1);
                                                    bookingEndTime = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 2, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingEndTime;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (bookingStartTime >= nextStartTime && bookingStartTime < nextEndTime && bookingEndTime > nextEndTime && cc.list.IndexOf(tuple) + 1 == cc.list.Count - 1)
                                                {

                                                    Console.WriteLine(2);
                                                    bookingStartTime = nextEndTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 2, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }
                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) == cc.list.Count - 2)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.6);
                                            if (bookingStartTime >= nextEndTime && cc.list.IndexOf(tuple) == cc.list.Count - 2)
                                            {
                                                if (bookingStartTime - nextEndTime >= pufferhigh)
                                                {
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (bookingStartTime - nextEndTime < pufferhigh)
                                                {
                                                    bookingStartTime = nextEndTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (cc.charging_column_type_id.max_concurrent_charging < 50)
                                {
                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) == 0)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.1);
                                            if (bookingEndTime < currentStartTime && cc.list.IndexOf(tuple) == 1)
                                            {
                                                if (currentStartTime - bookingEndTime >= pufferlow)
                                                {
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (currentStartTime - bookingEndTime < pufferlow)
                                                {
                                                    bookingEndTime = currentStartTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }

                                            }
                                        }
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) < cc.list.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;

                                            Console.WriteLine(1.2);

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
                                                        b.start_time = bookingStartTime;
                                                        b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                        cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                        goto Exit;
                                                    }

                                                    if (nextStartTime - bookingEndTime <= pufferlow)
                                                    {
                                                        bookingEndTime = nextStartTime;
                                                        cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                        b.charging_column = cc;
                                                        b.Accept();
                                                        b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                        b.end_time = bookingEndTime - pufferbetween;
                                                        cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));


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
                                                        b.start_time = bookingStartTime;
                                                        b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                        b.Accept();
                                                        cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

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
                                                            b.start_time = bookingStartTime;
                                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                            goto Exit;
                                                        }
                                                        else
                                                        {
                                                            bookingEndTime = nextStartTime;
                                                            cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                            b.charging_column = cc;
                                                            b.Accept();
                                                            b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                            b.end_time = bookingEndTime - pufferbetween;
                                                            cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                            goto Exit;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) < cc.list.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.3);

                                            if (currentEndTime > bookingStartTime && bookingEndTime <= nextStartTime)
                                            {
                                                if (bookingEndTime - currentEndTime < bookingRealTimeSpan)
                                                {
                                                    goto ExitTuple;
                                                }

                                                else
                                                {
                                                    bookingStartTime = currentEndTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }

                                            }

                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) < cc.list.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.4);
                                            if (currentEndTime > bookingStartTime && bookingEndTime > nextStartTime)
                                            {
                                                if (nextStartTime - pufferbetween - currentEndTime < bookingRealTimeSpan)
                                                {
                                                    goto ExitTuple;
                                                }

                                                else
                                                {
                                                    bookingStartTime = currentEndTime;

                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }

                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) < cc.list.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.5);
                                            if (currentEndTime <= bookingStartTime && bookingEndTime > nextStartTime)
                                            {
                                                if (nextStartTime - bookingStartTime < bookingRealTimeSpan || bookingEndTime - nextEndTime < bookingRealTimeSpan)
                                                {
                                                    goto ExitTuple;
                                                }
                                                else if (nextStartTime - bookingStartTime > bookingRealTimeSpan)
                                                {
                                                    bookingEndTime = nextStartTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (bookingEndTime - nextEndTime > bookingRealTimeSpan)
                                                {
                                                    bookingStartTime = nextEndTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;

                                                }
                                            }

                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list)
                                    {
                                        if (cc.list.IndexOf(tuple) == cc.list.Count - 2)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[cc.list.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.6);
                                            if (bookingStartTime >= nextEndTime && cc.list.IndexOf(tuple) == cc.list.Count - 2)
                                            {
                                                if (bookingStartTime - nextEndTime >= pufferlow)
                                                {
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (bookingStartTime - nextEndTime < pufferlow)
                                                {
                                                    bookingStartTime = nextEndTime;
                                                    cc.list.Insert(cc.list.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Exit:;
                foreach (ChargingColumn cc in listofBookingChargingColumn)
                {

                    foreach (var v in cc.list)
                    {
                        Console.WriteLine("CC: " + cc.charging_column_type_id.manufacturer_name + " start_time: " + v.Item1 + " end_time: " + v.Item2 + "\n");
                        
                    }
                    Console.WriteLine(cc.list.Count());
                    Console.Write("\n");
                }
            }

        }
    }
}


*/