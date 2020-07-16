using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Uebungsprojekt.Algorithm;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace ListsUnitTests
{
    public class Tests
    {
        private BookingDaoImpl _bookingDao;
        private LocationDaoImpl _locationDao;
        private ChargingColumnDaoImpl _chargingcolumndao;
        private ChargingZoneDaoImpl _chargingZoneDao;
        private VehicleDaoImpl _vehicleDao;
        private ChargingColumnTypeDaoImpl _chargingColumnTypeDao;
        private UserDaoImpl _userDao;

        [SetUp]
        public void Setup()
        {
            _bookingDao = new BookingDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _locationDao = new LocationDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _chargingcolumndao = new ChargingColumnDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _chargingZoneDao = new ChargingZoneDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _vehicleDao = new VehicleDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _chargingColumnTypeDao = new ChargingColumnTypeDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestConnectorCompare()
        {
            // create vehicles
            _vehicleDao.GetAll();
            int veh1_id = _vehicleDao.Create("Tesla", 80, new List<ConnectorType> { ConnectorType.Tesla_Supercharger, ConnectorType.CCS_Combo_2_Plug });
            int veh2_id = _vehicleDao.Create("Porsche", 95, new List<ConnectorType> { ConnectorType.Type_1_Plug, ConnectorType.Tesla_Supercharger, ConnectorType.CHAdeMO_Plug });
            Vehicle veh1 = _vehicleDao.GetById(veh1_id);
            Vehicle veh2 = _vehicleDao.GetById(veh2_id);

            // create chargingColumnTypes
            int cct_id1 = _chargingColumnTypeDao.Create("Terra 54 CT", "ABB", 50, new List<ConnectorType> { ConnectorType.CCS_Combo_2_Plug, ConnectorType.Type_2_Plug });
            int cct_id2 = _chargingColumnTypeDao.Create("eStation smart multi QC45", "Innogy", 60, new List<ConnectorType> {  ConnectorType.Type_2_Plug, ConnectorType.CHAdeMO_Plug, ConnectorType.Type_1_Plug });
            ChargingColumnType cct1 = _chargingColumnTypeDao.GetById(cct_id1);
            ChargingColumnType cct2 = _chargingColumnTypeDao.GetById(cct_id2);

            // create a Location
            int locationdao_id = 0;
            _locationDao.GetAll(locationdao_id);
            int loc_id1 = _locationDao.Create("Munich", "12345", "addressstreet", locationdao_id);
            Location loc1 = _locationDao.GetById(loc_id1, locationdao_id);

            // Create ChargingZone
            int ChargingZoneDao_id = 0;
            _chargingZoneDao.GetAll(ChargingZoneDao_id);
            int cz_id1 = _chargingZoneDao.Create(5, loc1, ChargingZoneDao_id);
            ChargingZone cz1 = _chargingZoneDao.GetById(cz_id1, ChargingZoneDao_id);

            // Create ChargingColumns
            int charcoldao_id = 0;
            _chargingcolumndao.GetAll(charcoldao_id);
            int charcol_id1 = _chargingcolumndao.Create(cct1, false, false, cz1, new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id2 = _chargingcolumndao.Create(cct2, false, false, cz1, new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            ChargingColumn cc1 = _chargingcolumndao.GetById(charcol_id1, charcoldao_id);
            ChargingColumn cc2 = _chargingcolumndao.GetById(charcol_id2, charcoldao_id);

            // create a few bookings
            int bookingdao_id = 0;
            _bookingDao.GetAll(bookingdao_id);

            int bk_id1 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 15, 12, 0, 0), new DateTime(2020, 7, 15, 13, 30, 0), veh1, new User(), loc1, null, bookingdao_id);
            int bk_id2 = _bookingDao.Create(30, 90, new DateTime(2020, 7, 15, 12, 0, 0), new DateTime(2020, 7, 15, 13, 30, 0), veh2, new User(), loc1, null, bookingdao_id);
            Booking bk1 = _bookingDao.GetById(bk_id1, bookingdao_id);
            Booking bk2 = _bookingDao.GetById(bk_id2, bookingdao_id);

            Assert.AreEqual(true, HelpFunctions.ConnectorCompare(cc1, bk1)); // 1 gemeinsamen conTyp
            Assert.AreEqual(true, HelpFunctions.ConnectorCompare(cc2, bk2)); // 2 gemeinsame conTyp
            Assert.AreEqual(false, HelpFunctions.ConnectorCompare(cc1, bk2)); // 0 gemeinsame conTyp

        }

        [Test]
        public void TestRealChargingTime()
        {
            // create vehicles
            _vehicleDao.GetAll();
            int veh1_id = _vehicleDao.Create("Tesla", 80, new List<ConnectorType> { ConnectorType.Tesla_Supercharger, ConnectorType.CCS_Combo_2_Plug });
            int veh2_id = _vehicleDao.Create("Porsche", 95, new List<ConnectorType> { ConnectorType.Type_2_Plug, ConnectorType.CCS_Combo_2_Plug, ConnectorType.Type_1_Plug });
            Vehicle veh1 = _vehicleDao.GetById(veh1_id);
            Vehicle veh2 = _vehicleDao.GetById(veh2_id);

            // create chargingColumnTypes
            int cct_id1 = _chargingColumnTypeDao.Create("Terra 54 CT", "ABB", 50, new List<ConnectorType> { ConnectorType.CCS_Combo_2_Plug, ConnectorType.Type_2_Plug });
            int cct_id2 = _chargingColumnTypeDao.Create("eStation smart multi QC45", "Innogy", 60, new List<ConnectorType> { ConnectorType.CCS_Combo_2_Plug, ConnectorType.Type_2_Plug, ConnectorType.CHAdeMO_Plug });
            ChargingColumnType cct1 = _chargingColumnTypeDao.GetById(cct_id1);
            ChargingColumnType cct2 = _chargingColumnTypeDao.GetById(cct_id2);

            // create a Location
            int locationdao_id = 0;
            _locationDao.GetAll(locationdao_id);
            int loc_id1 = _locationDao.Create("Munich", "12345", "addressstreet", locationdao_id);
            Location loc1 = _locationDao.GetById(loc_id1, locationdao_id);

            // Create ChargingZone
            int ChargingZoneDao_id = 0;
            _chargingZoneDao.GetAll(ChargingZoneDao_id);
            int cz_id1 = _chargingZoneDao.Create(5, loc1, ChargingZoneDao_id);
            ChargingZone cz1 = _chargingZoneDao.GetById(cz_id1, ChargingZoneDao_id);

            // Create ChargingColumns
            int charcoldao_id = 0;
            _chargingcolumndao.GetAll(charcoldao_id);
            int charcol_id1 = _chargingcolumndao.Create(cct1, false, false, cz1, new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id2 = _chargingcolumndao.Create(cct2, false, false, cz1, new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            ChargingColumn cc1 = _chargingcolumndao.GetById(charcol_id1, charcoldao_id);
            ChargingColumn cc2 = _chargingcolumndao.GetById(charcol_id2, charcoldao_id);

            // create a few bookings
            int bookingdao_id = 0;
            _bookingDao.GetAll(bookingdao_id);

            int bk_id1 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 15, 12, 0, 0), new DateTime(2020, 7, 15, 13, 30, 0), veh1, new User(), loc1, null, bookingdao_id);
            int bk_id2 = _bookingDao.Create(30, 90, new DateTime(2020, 7, 15, 12, 0, 0), new DateTime(2020, 7, 15, 13, 30, 0), veh2, new User(), loc1, null, bookingdao_id);
            Booking bk1 = _bookingDao.GetById(bk_id1, bookingdao_id);
            Booking bk2 = _bookingDao.GetById(bk_id2, bookingdao_id);

            TimeSpan ts = Uebungsprojekt.Algorithm.ChargingTime.RealChargingTime(cct1, bk1);

            Console.WriteLine(" real_charging_time: " + ts);

            Assert.Pass();
        }


        [Test]
        public void TestFindUnacceptedBookings()
        {
            // create some users
            _userDao.GetAll();
            int user_id1 = _userDao.Create("Viktor", "viktor@zaun.com", "laser", Role.Employee);
            int user_id2 = _userDao.Create("Annie", "annie@tibbers.com", "fire", Role.Guest);
            int user_id3 = _userDao.Create("Orianna", "orianna@dollhouse.com", "ball", Role.VIP);
            int user_id4 = _userDao.Create("Galio", "galio@demacia.com", "COLOSSUS", Role.Guest);
            int user_id5 = _userDao.Create("Zed", "zed@shadowassociation.com", "UNSEENBLAD3", Role.Employee);
            int user_id6 = _userDao.Create("Thresh", "thresh@shadowisles.com", "lantern", Role.VIP);

            // get new id and add a few bookings
            int id = BookingDaoImpl.CreateNewDaoId();
            _bookingDao.GetAll(id);
            int bId1 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), _userDao.GetById(user_id1), new Location(), new ChargingColumn(), id); //Emp
            int bId2 = _bookingDao.Create(20, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), _userDao.GetById(user_id2), new Location(), new ChargingColumn(), id); //Guest
            int bId3 = _bookingDao.Create(30, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), _userDao.GetById(user_id3), new Location(), new ChargingColumn(), id); //VIP
            int bId4 = _bookingDao.Create(40, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), _userDao.GetById(user_id4), new Location(), new ChargingColumn(), id); //Guest
            int bId5 = _bookingDao.Create(50, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), _userDao.GetById(user_id5), new Location(), new ChargingColumn(), id); //Emp
            int bId6 = _bookingDao.Create(60, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), _userDao.GetById(user_id6), new Location(), new ChargingColumn(), id); //VIP

            _bookingDao.GetById(bId2, id).accepted = true;
            _bookingDao.GetById(bId6, id).accepted = true;

            List<Booking> bookings = _bookingDao.GetAll(id);

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

            // Check if there are the right amount of unacceptedBookings
            Assert.AreEqual(4, unacceptedBookings.Count);

            // Check if every booking in unacceptedBookings is in the right order
            /*
            foreach (Booking b in unacceptedBookings)
            {
                Console.WriteLine("Role: " + b.user.role);
            }
                */
            int i = 1;
            foreach(Booking b in unacceptedBookings)
            {
                if(i <= 1)
                {
                    Assert.AreEqual(Role.VIP, b.user.role);
                }
                if (i <= 3 && i > 1)
                {
                    Assert.AreEqual(Role.Employee, b.user.role);
                }
                if (i <= 4 && i > 3)
                {
                    Assert.AreEqual(Role.Guest, b.user.role);
                }
                i++;
            }
        }

        [Test]
        public void TestListOfBookingLocations()
        {
            int locationdao_id = LocationDaoImpl.CreateNewDaoId();
            _locationDao.GetAll(locationdao_id);
            int loc_id1 = _locationDao.Create("Munich", "12345", "addressstreet", locationdao_id);
            int loc_id2 = _locationDao.Create("Augsburg", "12345", "addressstreet", locationdao_id);
            int loc_id3 = _locationDao.Create("Ingolcity", "12345", "addressstreet", locationdao_id);

            // get new id and add a few bookings
            int bookingdao_id = BookingDaoImpl.CreateNewDaoId();
            _bookingDao.GetAll(bookingdao_id);
            _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), new User(), _locationDao.GetById(loc_id1, locationdao_id), new ChargingColumn(), bookingdao_id);
            _bookingDao.Create(20, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), new User(), _locationDao.GetById(loc_id1, locationdao_id), new ChargingColumn(), bookingdao_id);
            _bookingDao.Create(30, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), new User(), _locationDao.GetById(loc_id2, locationdao_id), new ChargingColumn(), bookingdao_id);
            _bookingDao.Create(40, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), new User(), _locationDao.GetById(loc_id3, locationdao_id), new ChargingColumn(), bookingdao_id);
            _bookingDao.Create(50, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), new User(), _locationDao.GetById(loc_id1, locationdao_id), new ChargingColumn(), bookingdao_id);

            List<Booking> bookings = _bookingDao.GetAll(bookingdao_id);
            // List<Location> listofBookingLocations = bookings.OfType<Location>().ToList();
            List<Location> listofBookingLocations = new List<Location>();
            foreach (Booking b in bookings)
            {
                if(listofBookingLocations.Find(x => x.id == b.location.id) == null)
                {
                    listofBookingLocations.Add(b.location);
                }
            }

            Console.WriteLine("listofBookingLocations: " + listofBookingLocations.Count);

            Assert.AreEqual(3, listofBookingLocations.Count);

            foreach(Location loc in listofBookingLocations)
            {
                Console.WriteLine("Location: " + loc.city);
            }
            /*
            Assert.AreEqual(_locationDao.GetById(loc_id1, locationdao_id).city, listofBookingLocations.Find(x => x.city == "Munich"));
            Assert.AreEqual(_locationDao.GetById(loc_id2, locationdao_id).city, listofBookingLocations.Find(x => x.city == "Augsburg"));
            Assert.AreEqual(_locationDao.GetById(loc_id3, locationdao_id).city, listofBookingLocations.Find(x => x.city == "Ingolcity"));
            */
        }
    
        [Test]
        public void TestListOfConnectorTypes()
        {
            var connectorTypes = Enum.GetValues(typeof(ConnectorType)).Cast<ConnectorType>().ToList();
            Assert.Pass();
        }

        [Test]
        public void TestListOfChargingColumns()
        {
            // Create Locations
            int locationdao_id = LocationDaoImpl.CreateNewDaoId();
            _locationDao.GetAll(locationdao_id);
            int loc_id1 = _locationDao.Create("Munich", "12345", "addressstreet", locationdao_id);
            int loc_id2 = _locationDao.Create("Augsburg", "12345", "addressstreet", locationdao_id);
            int loc_id3 = _locationDao.Create("Ingolcity", "12345", "addressstreet", locationdao_id);
            // Create ChargingZones
            int ChargingZoneDao_id = ChargingZoneDaoImpl.CreateNewDaoId();
            _chargingZoneDao.GetAll(ChargingZoneDao_id);
            int cz_id1 = _chargingZoneDao.Create(5, _locationDao.GetById(loc_id1, locationdao_id), ChargingZoneDao_id);
            int cz_id2 = _chargingZoneDao.Create(6, _locationDao.GetById(loc_id2, locationdao_id), ChargingZoneDao_id);
            int cz_id3 = _chargingZoneDao.Create(7, _locationDao.GetById(loc_id1, locationdao_id), ChargingZoneDao_id);
            int cz_id4 = _chargingZoneDao.Create(7, _locationDao.GetById(loc_id3, locationdao_id), ChargingZoneDao_id);
            // Create ChargingColumns
            int charcoldao_id = ChargingColumnDaoImpl.CreateNewDaoId();
            _chargingcolumndao.GetAll(charcoldao_id);
            int charcol_id1 = _chargingcolumndao.Create(new ChargingColumnType(), false, false, _chargingZoneDao.GetById(cz_id1, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id2 = _chargingcolumndao.Create(new ChargingColumnType(), false, false, _chargingZoneDao.GetById(cz_id2, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id3 = _chargingcolumndao.Create(new ChargingColumnType(), false, false, _chargingZoneDao.GetById(cz_id2, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id4 = _chargingcolumndao.Create(new ChargingColumnType(), false, false, _chargingZoneDao.GetById(cz_id4, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id5 = _chargingcolumndao.Create(new ChargingColumnType(), false, false, _chargingZoneDao.GetById(cz_id3, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id6 = _chargingcolumndao.Create(new ChargingColumnType(), false, false, _chargingZoneDao.GetById(cz_id1, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id7 = _chargingcolumndao.Create(new ChargingColumnType(), false, false, _chargingZoneDao.GetById(cz_id3, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);

            // Create List with Locations
            List<Location> listofBookingLocations = new List<Location>();
            listofBookingLocations.Add(_locationDao.GetById(loc_id1, locationdao_id));
            listofBookingLocations.Add(_locationDao.GetById(loc_id3, locationdao_id));
            Console.WriteLine("listofBookingLocations: " + listofBookingLocations.Count);

            // Alternativ way to filter Columns:
            List<ChargingColumn> altListofAllChargingColumn = _chargingcolumndao.GetAll(charcoldao_id);
            Console.WriteLine("altListofAllChargingColumn: " + altListofAllChargingColumn.Count);
            List<ChargingColumn> altListOfCharCol = new List<ChargingColumn>();
            foreach (Location loc in listofBookingLocations)
            {
                foreach (ChargingColumn cc in altListofAllChargingColumn)
                {
                    if (cc.charging_zone.location.id == loc.id)
                    {
                        altListOfCharCol.Add(cc);
                    }
                }
            }

            // Check for alternativ Version
            int idSumListofChargingColumn = 0;
            foreach (ChargingColumn cc in altListOfCharCol)
            {
                idSumListofChargingColumn += cc.id;
            }
            Assert.AreEqual(5, altListOfCharCol.Count());
            Assert.AreEqual(charcol_id1 + charcol_id4 + charcol_id5 + charcol_id6 + charcol_id7, idSumListofChargingColumn);


            // Get ChargingColumns in specified Locations
            List<ChargingColumn> listofAllChargingColumn = _chargingcolumndao.GetAll(charcoldao_id);
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

            // Check if it filtered the correct Columns
            idSumListofChargingColumn = 0;
            foreach (ChargingColumn cc in listofChargingColumn)
            {
                idSumListofChargingColumn += cc.id;
            }
            Assert.AreEqual(5, listofChargingColumn.Count());
            Assert.AreEqual(charcol_id1 + charcol_id4 + charcol_id5 + charcol_id6 + charcol_id7, idSumListofChargingColumn);

            
        }

        [Test]
        public void TestconnectorTypes()
        {
            List<ConnectorType> connectorTypes = Enum.GetValues(typeof(ConnectorType)).Cast<ConnectorType>().ToList();
            Assert.AreEqual(6, connectorTypes.Count);
            foreach(ConnectorType ct in connectorTypes)
            {
                Console.WriteLine("ConType: " + ct);
            }
        }

        [Test]
        public void TestListofBookingChargingColumn()
        {
            // Create Locations
            int locationdao_id = LocationDaoImpl.CreateNewDaoId();
            _locationDao.GetAll(locationdao_id);
            int loc_id1 = _locationDao.Create("Munich", "12345", "addressstreet", locationdao_id);
            int loc_id2 = _locationDao.Create("Augsburg", "12345", "addressstreet", locationdao_id);
            int loc_id3 = _locationDao.Create("Ingolcity", "12345", "addressstreet", locationdao_id);
            // Create ChargingZones
            int ChargingZoneDao_id = ChargingZoneDaoImpl.CreateNewDaoId();
            _chargingZoneDao.GetAll(ChargingZoneDao_id);
            int cz_id1 = _chargingZoneDao.Create(5, _locationDao.GetById(loc_id1, locationdao_id), ChargingZoneDao_id);
            int cz_id2 = _chargingZoneDao.Create(6, _locationDao.GetById(loc_id2, locationdao_id), ChargingZoneDao_id);
            int cz_id3 = _chargingZoneDao.Create(7, _locationDao.GetById(loc_id1, locationdao_id), ChargingZoneDao_id);
            int cz_id4 = _chargingZoneDao.Create(7, _locationDao.GetById(loc_id3, locationdao_id), ChargingZoneDao_id);
            // Create ChargingColumnsTypes
            _chargingColumnTypeDao.GetAll();
            int cct_id1 = _chargingColumnTypeDao.Create("30", "McDonalds", 3, new List<ConnectorType> { ConnectorType.CCS_Combo_2_Plug, ConnectorType.Tesla_Supercharger, ConnectorType.Type_1_Plug });
            int cct_id2 = _chargingColumnTypeDao.Create("40", "BurgerKing", 2, new List<ConnectorType> { ConnectorType.CCS_Combo_2_Plug, ConnectorType.Type_2_Plug, ConnectorType.CHAdeMO_Plug });
            int cct_id3 = _chargingColumnTypeDao.Create("35", "KFC", 1, new List<ConnectorType> { ConnectorType.Type_2_Plug });

            // Create ChargingColumns
            int charcoldao_id = ChargingColumnDaoImpl.CreateNewDaoId();
            _chargingcolumndao.GetAll(charcoldao_id);
            int charcol_id1 = _chargingcolumndao.Create(_chargingColumnTypeDao.GetById(cct_id1), false, false, _chargingZoneDao.GetById(cz_id1, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id2 = _chargingcolumndao.Create(_chargingColumnTypeDao.GetById(cct_id2), false, false, _chargingZoneDao.GetById(cz_id2, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id3 = _chargingcolumndao.Create(_chargingColumnTypeDao.GetById(cct_id2), false, false, _chargingZoneDao.GetById(cz_id2, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id4 = _chargingcolumndao.Create(_chargingColumnTypeDao.GetById(cct_id3), false, false, _chargingZoneDao.GetById(cz_id4, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id5 = _chargingcolumndao.Create(_chargingColumnTypeDao.GetById(cct_id1), false, false, _chargingZoneDao.GetById(cz_id3, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            int charcol_id6 = _chargingcolumndao.Create(_chargingColumnTypeDao.GetById(cct_id3), false, false, _chargingZoneDao.GetById(cz_id1, ChargingZoneDao_id), new List<Tuple<DateTime, DateTime>>(), charcoldao_id);
            List<ChargingColumn> listofChargingColumn = _chargingcolumndao.GetAll(charcoldao_id); ;

            // Create List with Locations
            List<Location> listofBookingLocations = new List<Location>();
            listofBookingLocations.Add(_locationDao.GetById(loc_id1, locationdao_id));
            listofBookingLocations.Add(_locationDao.GetById(loc_id3, locationdao_id));
            Console.WriteLine("listofBookingLocations: " + listofBookingLocations.Count);

            // Create Vehicles
            _vehicleDao.GetAll();
            int veh_id1 = _vehicleDao.Create("Tesla", 50, new List<ConnectorType> { ConnectorType.CCS_Combo_2_Plug, ConnectorType.Tesla_Supercharger });
            int veh_id2 = _vehicleDao.Create("Porsche", 50, new List<ConnectorType> { ConnectorType.CCS_Combo_2_Plug, ConnectorType.Schuko_Socket });
            int veh_id3 = _vehicleDao.Create("VW", 50, new List<ConnectorType> { ConnectorType.Type_1_Plug, ConnectorType.CHAdeMO_Plug });


            // get new id and add a few bookings
            int bookingDaoId = BookingDaoImpl.CreateNewDaoId();
            _bookingDao.GetAll(bookingDaoId);
            int bId1 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0),  _vehicleDao.GetById(veh_id1), new User(), _chargingcolumndao.GetById(charcol_id1, charcoldao_id).charging_zone.location, _chargingcolumndao.GetById(charcol_id1, charcoldao_id), bookingDaoId);
            int bId2 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0),  _vehicleDao.GetById(veh_id1), new User(), _chargingcolumndao.GetById(charcol_id2, charcoldao_id).charging_zone.location, _chargingcolumndao.GetById(charcol_id2, charcoldao_id), bookingDaoId);
            int bId3 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0),  _vehicleDao.GetById(veh_id3), new User(), _chargingcolumndao.GetById(charcol_id3, charcoldao_id).charging_zone.location, _chargingcolumndao.GetById(charcol_id3, charcoldao_id), bookingDaoId);
            int bId4 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0),  _vehicleDao.GetById(veh_id2), new User(), _chargingcolumndao.GetById(charcol_id2, charcoldao_id).charging_zone.location, _chargingcolumndao.GetById(charcol_id2, charcoldao_id), bookingDaoId);
            int bId5 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0),  _vehicleDao.GetById(veh_id3), new User(), _chargingcolumndao.GetById(charcol_id5, charcoldao_id).charging_zone.location, _chargingcolumndao.GetById(charcol_id5, charcoldao_id), bookingDaoId);
            int bId6 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0),  _vehicleDao.GetById(veh_id1), new User(), _chargingcolumndao.GetById(charcol_id6, charcoldao_id).charging_zone.location, _chargingcolumndao.GetById(charcol_id6, charcoldao_id), bookingDaoId);
            int bId7 = _bookingDao.Create(10, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0),  _vehicleDao.GetById(veh_id2), new User(), _chargingcolumndao.GetById(charcol_id4, charcoldao_id).charging_zone.location, _chargingcolumndao.GetById(charcol_id4, charcoldao_id), bookingDaoId);

            List<Booking> unacceptedBookings = _bookingDao.GetAll(bookingDaoId);

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

            // Check if listofBookingChargingColumn is correct
            int listofBookingChargingColumnSum = 0;
            foreach(ChargingColumn cc in listofBookingChargingColumn)
            {
                listofBookingChargingColumnSum += cc.id;
            }
            Assert.AreEqual(4, listofBookingChargingColumn.Count);
            Assert.AreEqual(charcol_id1 + charcol_id2 + charcol_id3 + charcol_id5, listofBookingChargingColumnSum);
        }
    }

}