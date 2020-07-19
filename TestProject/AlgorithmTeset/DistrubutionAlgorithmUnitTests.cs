using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace UnitTest.Algorithm
{

    public class DistrubutionAlgorithmUnitTests
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
        /*
        [Test]
        public void TestDistributionAlgorithm()
        {
            // create some users
            _userDao.GetAll();
            int user_id1 = _userDao.Create("Viktor", "viktor@zaun.com", "laser", Role.Employee);
            int user_id2 = _userDao.Create("Annie", "annie@tibbers.com", "fire", Role.Guest);
            int user_id3 = _userDao.Create("Orianna", "orianna@dollhouse.com", "ball", Role.VIP);
            int user_id4 = _userDao.Create("Galio", "galio@demacia.com", "COLOSSUS", Role.Guest);
            int user_id5 = _userDao.Create("Zed", "zed@shadowassociation.com", "UNSEENBLAD3", Role.Employee);
            int user_id6 = _userDao.Create("Thresh", "thresh@shadowisles.com", "lantern", Role.VIP);
            User userEmp = _userDao.GetById(user_id1);
            User userGue = _userDao.GetById(user_id2);
            User userVip = _userDao.GetById(user_id3);

            // create chargingColumnTypes
            int cct_id1 = _chargingColumnTypeDao.Create("Terra 54 CT", "ABB", 2, new List<Tuple<ConnectorType, int>> { Tuple.Create(ConnectorType.CCS_Combo_2_Plug, 80), Tuple.Create(ConnectorType.Type_2_Plug, 60)});
            int cct_id2 = _chargingColumnTypeDao.Create("eStation smart multi QC45", "Innogy", 3, new List<Tuple<ConnectorType, int>> { Tuple.Create(ConnectorType.CCS_Combo_2_Plug, 80), Tuple.Create(ConnectorType.Type_2_Plug, 70), Tuple.Create(ConnectorType.CHAdeMO_Plug, 80) });
            int cct_id3 = _chargingColumnTypeDao.Create("DC Ladestation", "Delta", 2, new List<Tuple<ConnectorType, int>> { Tuple.Create(ConnectorType.Tesla_Supercharger, 60), Tuple.Create(ConnectorType.Type_1_Plug, 100) });
            int cct_id4 = _chargingColumnTypeDao.Create("Evlink", "Schneider Electric", 3, new List<Tuple<ConnectorType, int>> { Tuple.Create(ConnectorType.Schuko_Socket, 50), Tuple.Create(ConnectorType.Type_2_Plug, 150), Tuple.Create(ConnectorType.Tesla_Supercharger, 60) });
            ChargingColumnType cct1 = _chargingColumnTypeDao.GetById(cct_id1);
            ChargingColumnType cct2 = _chargingColumnTypeDao.GetById(cct_id2);
            ChargingColumnType cct3 = _chargingColumnTypeDao.GetById(cct_id3);
            ChargingColumnType cct4 = _chargingColumnTypeDao.GetById(cct_id4);

            // create a Location
            int locationdao_id = 0;
            _locationDao.GetAll(locationdao_id);
            int loc_id1 = _locationDao.Create("Munich", "12345", "addressstreet", locationdao_id);
            Location loc1 = _locationDao.GetById(loc_id1, locationdao_id);

            // Create ChargingZone
            int ChargingZoneDao_id = 0;
            _chargingZoneDao.GetAll(ChargingZoneDao_id);
            int cz_id1 = _chargingZoneDao.Create("alpha", 5, loc1, ChargingZoneDao_id);
            ChargingZone cz1 = _chargingZoneDao.GetById(cz_id1, ChargingZoneDao_id);

            // Create ChargingColumns
            int charcoldao_id = 1;
            _chargingcolumndao.GetAll(charcoldao_id);
            int charcol_id1 = _chargingcolumndao.Create(cct1,  cz1, null, charcoldao_id);
            int charcol_id2 = _chargingcolumndao.Create(cct2,  cz1, null, charcoldao_id);
            int charcol_id3 = _chargingcolumndao.Create(cct3,  cz1, null, charcoldao_id);
            int charcol_id4 = _chargingcolumndao.Create(cct4,  cz1, null, charcoldao_id);
            ChargingColumn cc1 = _chargingcolumndao.GetById(charcol_id1, charcoldao_id);
            ChargingColumn cc2 = _chargingcolumndao.GetById(charcol_id2, charcoldao_id);
            ChargingColumn cc3 = _chargingcolumndao.GetById(charcol_id3, charcoldao_id);
            ChargingColumn cc4 = _chargingcolumndao.GetById(charcol_id4, charcoldao_id);

            // create vehicles
            _vehicleDao.GetAll();
            int veh1_id = _vehicleDao.Create("Tesla", 80, new List<ConnectorType> { ConnectorType.Tesla_Supercharger, ConnectorType.CCS_Combo_2_Plug },userGue);
            int veh2_id = _vehicleDao.Create("Porsche", 95, new List<ConnectorType> { ConnectorType.Type_2_Plug, ConnectorType.CCS_Combo_2_Plug, ConnectorType.Type_1_Plug },userGue);
            int veh3_id = _vehicleDao.Create("VW", 58, new List<ConnectorType> { ConnectorType.Schuko_Socket, ConnectorType.CHAdeMO_Plug }, userGue);
            Vehicle veh1 = _vehicleDao.GetById(veh1_id);
            Vehicle veh2 = _vehicleDao.GetById(veh2_id);
            Vehicle veh3 = _vehicleDao.GetById(veh3_id);


            // create a few bookings
            int bookingdao_id = 1;
            _bookingDao.GetAll(bookingdao_id);
            ///Vip user
            _bookingDao.Create(10, 100, new DateTime(2020, 7, 15, 6, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh1, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(80, 100, new DateTime(2020, 7, 15, 6, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh2, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(60, 100, new DateTime(2020, 7, 15, 6, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh3, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(31, 100, new DateTime(2020, 7, 15, 10, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh1, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(67, 100, new DateTime(2020, 7, 15, 10, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh2, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(50, 100, new DateTime(2020, 7, 15, 10, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh3, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(34, 100, new DateTime(2020, 7, 15, 14, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh1, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(19, 100, new DateTime(2020, 7, 15, 14, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh2, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(23, 100, new DateTime(2020, 7, 15, 14, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh3, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(10, 100, new DateTime(2020, 7, 15, 14, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh1, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(10, 100, new DateTime(2020, 7, 15, 17, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh2, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(10, 100, new DateTime(2020, 7, 15, 12, 0, 0), new DateTime(2020, 7, 15, 18, 30, 0), veh3, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(10, 100, new DateTime(2020, 8, 15, 6, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(20, 100, new DateTime(2020, 8, 15, 6, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(60, 100, new DateTime(2020, 8, 15, 6, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(31, 100, new DateTime(2020, 8, 15, 10, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(68, 100, new DateTime(2020, 8, 15, 10, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(50, 100, new DateTime(2020, 8, 15, 10, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(34, 100, new DateTime(2020, 8, 15, 14, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(19, 100, new DateTime(2020, 8, 15, 14, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(23, 100, new DateTime(2020, 8, 15, 14, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(10, 100, new DateTime(2020, 8, 15, 14, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(10, 100, new DateTime(2020, 8, 15, 18, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userVip, loc1,  bookingdao_id);
            _bookingDao.Create(10, 100, new DateTime(2020, 8, 15, 12, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userVip, loc1,  bookingdao_id);

            _bookingDao.Create(30, 100, new DateTime(2020, 8, 15, 8, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(21, 100, new DateTime(2020, 8, 15, 8, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(16, 100, new DateTime(2020, 8, 15, 8, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(90, 100, new DateTime(2020, 8, 15, 8, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(30, 100, new DateTime(2020, 8, 15, 8, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(80, 100, new DateTime(2020, 8, 15, 8, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(70, 100, new DateTime(2020, 8, 15, 12, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(77, 100, new DateTime(2020, 8, 15, 12, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(34, 100, new DateTime(2020, 8, 15, 12, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(56, 100, new DateTime(2020, 8, 15, 12, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(53, 100, new DateTime(2020, 8, 15, 12, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userEmp, loc1,  bookingdao_id);
            _bookingDao.Create(18, 100, new DateTime(2020, 8, 15, 12, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userEmp, loc1,  bookingdao_id);

            _bookingDao.Create(18, 100, new DateTime(2020, 8, 15, 14, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(32, 100, new DateTime(2020, 8, 15, 15, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(75, 100, new DateTime(2020, 8, 15, 15, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(65, 100, new DateTime(2020, 8, 15, 16, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(63, 100, new DateTime(2020, 8, 15, 14, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(86, 100, new DateTime(2020, 8, 15, 11, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(55, 100, new DateTime(2020, 8, 15, 14, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(86, 100, new DateTime(2020, 8, 15, 11, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(31, 100, new DateTime(2020, 8, 15, 13, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh3, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(31, 100, new DateTime(2020, 8, 15, 12, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh2, userGue, loc1,  bookingdao_id);
            _bookingDao.Create(89, 100, new DateTime(2020, 8, 15, 10, 0, 0), new DateTime(2020, 8, 15, 18, 30, 0), veh1, userGue, loc1,  bookingdao_id);















            foreach (ChargingColumn cc in _chargingcolumndao.GetAll(charcoldao_id))
            {
                Console.WriteLine("list\n");
                foreach (var v in cc.list)
                {
                    Console.WriteLine("CC: " + cc.charging_column_type_id.manufacturer_name + " start_time: " + v.Item1 + " end_time: " + v.Item2 + "\n");
                }
            }

            Uebungsprojekt.Algorithm.DistributionAlgorithm.DistributionAlg(_chargingcolumndao, charcoldao_id, _bookingDao, new DateTime(2020, 8, 15), bookingdao_id);

            foreach (ChargingColumn cc in _chargingcolumndao.GetAll(charcoldao_id))
            {
                Console.WriteLine("list\n");
                foreach (var v in cc.list)
                {
                    Console.WriteLine("CC: " + cc.charging_column_type_id.manufacturer_name + " start_time: " + v.Item1 + " end_time: " + v.Item2 + "\n");
                }
            }

            
            foreach(Booking b in _bookingDao.GetAll(bookingdao_id))
            {
                Console.WriteLine("BookingId: " + b.id + " Role: " + b.user.role + " start_time: " + b.start_time + " end_time: " + b.end_time + " Vehicle: " + b.vehicle.id+"\n");
                Console.WriteLine("accepted: " + b.accepted + "\n");
            }

            foreach (ChargingColumn cc in _chargingcolumndao.GetAll(charcoldao_id))
            {
                for (int i = 0; i < cc.charging_column_type_id.max_parallel_charging; i++)
                {
                    CollectionAssert.IsOrdered(cc.list[i].Item1);
                    Console.WriteLine("Liste sortiert");
                }
            }

            Assert.AreEqual(35, Uebungsprojekt.Algorithm.DistributionAlgorithm.acc.Count());
            Console.WriteLine("akzeptierte Buchungen sind 35");

            
            
        }
        */
    }
}
