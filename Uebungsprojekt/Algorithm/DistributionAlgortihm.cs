using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Algorithm;
using MathNet.Numerics.Optimization;

namespace Uebungsprojekt.Algorithm
{
    public partial class DistributionAlgorithm
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
                            if (columnconnector.Item1 == connector)
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
                for (int i = 0; i < cc.charging_column_type_id.max_parallel_charging; ++i)
                {
                    Console.WriteLine("muss");
                    Console.WriteLine(cc.charging_column_type_id.max_parallel_charging);
                    Console.WriteLine(i);
                    result.Add(new ChargingColumn
                    {

                        id = cc.id,
                        charging_column_type_id = new ChargingColumnType
                        {
                            id = cc.charging_column_type_id.id,
                            model_name = cc.charging_column_type_id.model_name,
                            manufacturer_name = cc.charging_column_type_id.manufacturer_name,
                            max_parallel_charging = 1,
                            connectors = new List<Tuple<ConnectorType, int>> { Tuple.Create(cc.charging_column_type_id.connectors[i].Item1, cc.charging_column_type_id.connectors[i].Item2) }
                        },
                        busy = false,
                        emergency_reserve = false,
                        charging_zone = cc.charging_zone,
                        list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>> { Tuple.Create(cc.list[i].Item1, cc.list[i].Item2) }
                    });
                }
            }
            foreach (ChargingColumn cc in result)
            {
                Console.WriteLine("ID: " + cc.id + "\n Modelname:" + cc.charging_column_type_id.model_name + "\n ConnectorType:" + cc.charging_column_type_id.connectors[0].Item2 + "\n");
                foreach (Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType> tuple in cc.list)
                {
                    foreach (Tuple<DateTime, DateTime> tuple1 in tuple.Item1)
                    {
                        Console.WriteLine("StartTime: " + tuple1.Item1 + "\t EndTime: " + tuple1.Item2);
                    }
                }

            }

            foreach (ChargingColumn cc in listofBookingChargingColumn)
            {
                Console.WriteLine(cc.id + "\n");
            }
            Console.WriteLine("result id: \n");
            foreach (ChargingColumn cc in result)
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
                foreach (ChargingColumn cc in result)
                {
                    Console.WriteLine(cc.id);
                    TimeSpan bookingRealTimeSpan = ChargingTime.RealChargingTime(cc.charging_column_type_id, b);

                    Console.WriteLine(bookingindex);
                    if (cc.charging_zone.location == b.location)
                    {
                        if (HelpFunctions.ConnectorCompare(cc, b))
                        {
                            Console.WriteLine("==0");

                            if (cc.list[0].Item1.Count == 0)
                            {
                                cc.list[0].Item1.Insert(0, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                b.charging_column = cc;
                                b.Accept();
                                b.start_time = bookingStartTime;
                                b.end_time = bookingStartTime + bookingRealTimeSpan;
                                goto Exit;
                            }

                            Console.WriteLine("==1");

                            if (cc.list[0].Item1.Count == 1)
                            {
                                Tuple<DateTime, DateTime> tuple1 = cc.list[0].Item1[0];
                                DateTime currentStartTime1 = tuple1.Item1;
                                DateTime currentEndTime1 = tuple1.Item2;

                                if (cc.charging_column_type_id.connectors[0].Item2 >= 50)
                                {
                                    // Booking is between the last booking and the next booking without conflicts
                                    if (currentEndTime1 <= bookingStartTime)
                                    {
                                        Console.WriteLine(01.1111);
                                        if (bookingStartTime - currentEndTime1 >= pufferhigh)
                                        {

                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;


                                        }
                                        else if (bookingStartTime - currentEndTime1 < pufferhigh)
                                        {

                                            bookingStartTime = currentEndTime1;
                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            b.charging_column = cc;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                            goto Exit;

                                        }


                                    }

                                    if (currentStartTime1 >= bookingEndTime)
                                    {
                                        Console.WriteLine(01.2111);
                                        // Booking is between the last booking and the next booking without conflicts



                                        if (currentStartTime1 - bookingEndTime >= pufferhigh)
                                        {
                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }
                                        else if (currentStartTime1 - bookingEndTime < pufferhigh)
                                        {
                                            bookingEndTime = currentStartTime1;
                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                            b.end_time = bookingEndTime - pufferbetween;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
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
                                                cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                b.charging_column = cc;
                                                b.Accept();
                                                b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                b.end_time = bookingEndTime - pufferbetween;
                                                cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
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
                                                cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                b.charging_column = cc;
                                                b.Accept();
                                                b.start_time = bookingStartTime;
                                                b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                                goto Exit;
                                            }
                                        }
                                    }


                                    if (currentEndTime1 > bookingStartTime)
                                    {
                                        bookingStartTime = currentEndTime1;
                                        if (bookingEndTime - bookingStartTime > bookingRealTimeSpan)
                                        {
                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }


                                    }


                                }
                                else if (cc.charging_column_type_id.connectors[0].Item2 < 50)
                                {
                                    // Booking is between the last booking and the next booking without conflicts
                                    if (currentEndTime1 <= bookingStartTime)
                                    {
                                        if (bookingStartTime - currentEndTime1 >= pufferlow)
                                        {
                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }
                                        else if (bookingStartTime - currentEndTime1 < pufferlow)
                                        {
                                            bookingStartTime = currentEndTime1;
                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }


                                    }

                                    else if (currentStartTime1 >= bookingEndTime)
                                    {
                                        if (currentStartTime1 - bookingEndTime >= pufferlow)
                                        {
                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingStartTime;
                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                            goto Exit;
                                        }
                                        else if (currentStartTime1 - bookingEndTime < pufferlow)
                                        {
                                            bookingEndTime = currentStartTime1;
                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime, bookingEndTime - bookingRealTimeSpan + pufferbetween));
                                            b.charging_column = cc;
                                            b.Accept();
                                            b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                            b.end_time = bookingEndTime - pufferbetween;
                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
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
                                                cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                b.charging_column = cc;
                                                b.Accept();
                                                b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                b.end_time = bookingEndTime - pufferbetween;
                                                cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
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
                                                cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple1) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingEndTime + pufferbetween));
                                                b.charging_column = cc;
                                                b.Accept();
                                                b.start_time = bookingStartTime;
                                                b.end_time = bookingEndTime;
                                                cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                                                goto Exit;
                                            }
                                        }
                                    }
                                }



                            }

                            else
                            {
                                Console.WriteLine("meep");

                                if (cc.charging_column_type_id.connectors[0].Item2 >= 50)
                                {

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) == 0)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;
                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.1);
                                            if (bookingEndTime < currentStartTime && cc.list[0].Item1.IndexOf(tuple) == 1)
                                            {
                                                if (currentStartTime - bookingEndTime >= pufferhigh)
                                                {
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (currentStartTime - bookingEndTime < pufferhigh)
                                                {
                                                    bookingEndTime = currentStartTime;
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }
                                        }
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) < cc.list[0].Item1.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

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
                                                        cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                        b.charging_column = cc;
                                                        b.Accept();
                                                        b.start_time = bookingStartTime;
                                                        b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                        cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                        goto Exit;
                                                    }

                                                    if (nextStartTime - bookingEndTime <= pufferhigh)
                                                    {
                                                        bookingEndTime = nextStartTime;
                                                        cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                        b.charging_column = cc;
                                                        b.Accept();
                                                        b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                        b.end_time = bookingEndTime - pufferbetween;
                                                        cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                        goto Exit;
                                                    }

                                                }

                                                else if (bookingStartTime - currentEndTime < pufferhigh)
                                                {
                                                    if (nextStartTime - bookingEndTime >= pufferhigh)
                                                    {
                                                        bookingStartTime = currentEndTime;
                                                        cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                        b.charging_column = cc;
                                                        b.Accept();
                                                        b.start_time = bookingStartTime;
                                                        b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                        cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                        goto Exit;
                                                    }
                                                    else if (nextStartTime - bookingEndTime < pufferhigh)
                                                    {
                                                        if (nextStartTime - bookingEndTime > bookingStartTime - currentEndTime)
                                                        {
                                                            bookingStartTime = currentEndTime;
                                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                            b.charging_column = cc;
                                                            b.Accept();
                                                            b.start_time = bookingStartTime;
                                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                            goto Exit;
                                                        }
                                                        else
                                                        {
                                                            bookingEndTime = nextStartTime;
                                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                            b.charging_column = cc;
                                                            b.Accept();
                                                            b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                            b.end_time = bookingEndTime - pufferbetween;
                                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                            goto Exit;
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) < cc.list[0].Item1.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

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
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }
                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) < cc.list[0].Item1.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

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

                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }

                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) < cc.list[0].Item1.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

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
                                                if (nextStartTime > bookingStartTime && nextStartTime - bookingStartTime >= bookingRealTimeSpan)
                                                {

                                                    Console.WriteLine(1);
                                                    bookingEndTime = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 2, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingEndTime;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (bookingStartTime >= nextStartTime && bookingStartTime < nextEndTime && bookingEndTime > nextEndTime && cc.list[0].Item1.IndexOf(tuple) + 1 == cc.list[0].Item1.Count - 1)
                                                {

                                                    Console.WriteLine(2);
                                                    bookingStartTime = nextEndTime;
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 2, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }
                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) == cc.list[0].Item1.Count - 2)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.6);
                                            if (bookingStartTime >= nextEndTime && cc.list[0].Item1.IndexOf(tuple) == cc.list[0].Item1.Count - 2)
                                            {
                                                if (bookingStartTime - nextEndTime >= pufferhigh)
                                                {
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (bookingStartTime - nextEndTime < pufferhigh)
                                                {
                                                    bookingStartTime = nextEndTime;
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (cc.charging_column_type_id.connectors[0].Item2 < 50)
                                {
                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) == 0)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.1);
                                            if (bookingEndTime < currentStartTime && cc.list[0].Item1.IndexOf(tuple) == 1)
                                            {
                                                if (currentStartTime - bookingEndTime >= pufferlow)
                                                {
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (currentStartTime - bookingEndTime < pufferlow)
                                                {
                                                    bookingEndTime = currentStartTime;
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }

                                            }
                                        }
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) < cc.list[0].Item1.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

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
                                                        cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                        b.charging_column = cc;
                                                        b.Accept();
                                                        b.start_time = bookingStartTime;
                                                        b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                        cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                        goto Exit;
                                                    }

                                                    if (nextStartTime - bookingEndTime <= pufferlow)
                                                    {
                                                        bookingEndTime = nextStartTime;
                                                        cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan, bookingEndTime));
                                                        b.charging_column = cc;
                                                        b.Accept();
                                                        b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                        b.end_time = bookingEndTime - pufferbetween;
                                                        cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));


                                                        goto Exit;
                                                    }

                                                }

                                                else if (bookingStartTime - currentEndTime < pufferlow)
                                                {
                                                    if (nextStartTime - bookingEndTime >= pufferlow)
                                                    {
                                                        bookingStartTime = currentEndTime;
                                                        cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                        b.charging_column = cc;
                                                        b.start_time = bookingStartTime;
                                                        b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                        b.Accept();
                                                        cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                        goto Exit;

                                                    }
                                                    else if (nextStartTime - bookingEndTime < pufferlow)
                                                    {
                                                        if (nextStartTime - bookingEndTime > bookingStartTime - currentEndTime)
                                                        {
                                                            bookingStartTime = currentEndTime;
                                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                            b.charging_column = cc;
                                                            b.Accept();
                                                            b.start_time = bookingStartTime;
                                                            b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                            goto Exit;
                                                        }
                                                        else
                                                        {
                                                            bookingEndTime = nextStartTime;
                                                            cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                            b.charging_column = cc;
                                                            b.Accept();
                                                            b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                            b.end_time = bookingEndTime - pufferbetween;
                                                            cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                            goto Exit;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) < cc.list[0].Item1.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

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
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }

                                            }

                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) < cc.list[0].Item1.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

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

                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                            }

                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) < cc.list[0].Item1.Count - 1)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

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
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingEndTime - bookingRealTimeSpan - pufferbetween, bookingEndTime));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingEndTime - bookingRealTimeSpan - pufferbetween;
                                                    b.end_time = bookingEndTime - pufferbetween;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (bookingEndTime - nextEndTime > bookingRealTimeSpan)
                                                {
                                                    bookingStartTime = nextEndTime;
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;

                                                }
                                            }

                                        }
                                    ExitTuple:;
                                    }

                                    foreach (Tuple<DateTime, DateTime> tuple in cc.list[0].Item1)
                                    {
                                        if (cc.list[0].Item1.IndexOf(tuple) == cc.list[0].Item1.Count - 2)
                                        {
                                            Tuple<DateTime, DateTime> next = cc.list[0].Item1[cc.list[0].Item1.IndexOf(tuple) + 1];

                                            DateTime currentStartTime = tuple.Item1;
                                            DateTime currentEndTime = tuple.Item2;

                                            DateTime nextStartTime = next.Item1;
                                            DateTime nextEndTime = next.Item2;
                                            Console.WriteLine(1.6);
                                            if (bookingStartTime >= nextEndTime && cc.list[0].Item1.IndexOf(tuple) == cc.list[0].Item1.Count - 2)
                                            {
                                                if (bookingStartTime - nextEndTime >= pufferlow)
                                                {
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                                                    goto Exit;
                                                }
                                                else if (bookingStartTime - nextEndTime < pufferlow)
                                                {
                                                    bookingStartTime = nextEndTime;
                                                    cc.list[0].Item1.Insert(cc.list[0].Item1.IndexOf(tuple) + 1, new Tuple<DateTime, DateTime>(bookingStartTime, bookingStartTime + bookingRealTimeSpan + pufferbetween));
                                                    b.charging_column = cc;
                                                    b.Accept();
                                                    b.start_time = bookingStartTime;
                                                    b.end_time = bookingStartTime + bookingRealTimeSpan;
                                                    cc.list[0].Item1.Sort((x, y) => x.Item1.CompareTo(y.Item1));

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
                foreach (ChargingColumn cc in result)
                {
                    Console.WriteLine("ID: " + cc.id + "\n Modelname:" + cc.charging_column_type_id.model_name + "\n ConnectorType:" + cc.charging_column_type_id.connectors[0].Item1 + "\n");
                    foreach (Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType> tuple in cc.list)
                    {
                        foreach (Tuple<DateTime, DateTime> tuple1 in tuple.Item1)
                        {
                            Console.WriteLine("StartTime: " + tuple1.Item1 + "\t EndTime: " + tuple1.Item2);
                        }
                    }
                    Console.WriteLine("\n");
                }

                foreach (ChargingColumn cc in listofBookingChargingColumn)
                {
                    foreach (ChargingColumn cc2 in result)
                    {
                        if (cc.charging_column_type_id.id == cc2.charging_column_type_id.id)
                        {

                        }
                    }
                }

            }
        }
    }
}


