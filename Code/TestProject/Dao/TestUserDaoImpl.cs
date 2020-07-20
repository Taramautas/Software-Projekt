using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestUserDaoImpl
    {
        private UserDaoImpl _userDao;

        // Frage: Kann man die Assert.Equal Methode overriden?

        [SetUp]
        public void Setup()
        {
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditUserChagesObjectInCache()
        {
            // Initialize Daos
            List<User> userList1 = _userDao.GetAll();

            // Create new Users
            int user_id11 = _userDao.Create("user1", "user1@user1.de", "user1", Role.Employee);

            // Edit User in Dao1 with GetById
            Assert.AreEqual("user1", _userDao.GetById(user_id11).name);
            User user11 = _userDao.GetById(user_id11);
            user11.name = "ussr1";

            // check if the user got changed in the cache
            userList1 = _userDao.GetAll();
            Assert.AreEqual("ussr1", _userDao.GetById(user_id11).name);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectUser()
        {
            // Initialize Daos
            _userDao.GetAll();


            // Create new users in List1 and add it with Create
            User user11 = new User
            {
                id = 1,
                name = "user1",
                email = "user1@user1.de",
                password = "user1",
                role = Role.Employee
            };
            User user21 = new User
            {
                id = 2,
                name = "user2",
                email = "user2@user2.de",
                password = "user2",
                role = Role.Employee
            };
            int bk_id11 = _userDao.Create(user11.name, user11.email, user11.password, user11.role);
            int bk_id21 = _userDao.Create(user21.name, user21.email, user21.password, user21.role);

            // Check if GetById returns the correct user
            Assert.AreEqual(user11.name, _userDao.GetById(bk_id11).name);

            Assert.AreEqual(user21.name, _userDao.GetById(bk_id21).name);

        }

        
        /// <summary>
        /// Tests if users added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewUserToCache()
        {
            // Check if initial amount of users is 0
            List<User> userList1 = _userDao.GetAll();
            Assert.AreEqual(0, userList1.Count);

            // Create new user in List1 and add it with Create
            _userDao.Create(
                "user1",
                "user1@user1.de",
                "user1",
                Role.Employee
                );

            // Check if there is 1 User in userList1
            userList1 = _userDao.GetAll();
            Assert.AreEqual(1, userList1.Count);


            // Check again with 2 in userList1
            userList1 = _userDao.GetAll();
            Assert.AreEqual(1, userList1.Count);

            // Create new user in List1 and add it with Create
            _userDao.Create(
                "user2",
                "user2@user2.de",
                "user2",
                Role.Employee
                );


            // Check if there is 2 users in userList1
            userList1 = _userDao.GetAll();
            Assert.AreEqual(2, userList1.Count);
        }
        
        
        
        /// <summary>
        /// Tests if users added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewUserFromCache()
        {
            // Check if initial amount of users is 0
            List<User> userList1 = _userDao.GetAll();
            Assert.AreEqual(0, userList1.Count);

            // Create new user in List1 and add it with Create
            int bk_id11 = _userDao.Create(
                "user1",
                "user1@user1.de",
                "user1",
                Role.Employee
                );

            // Create new user in List1 and add it with Create
            int bk_id12 = _userDao.Create(
                "user2",
                "user2@user2.de",
                "user2",
                Role.Employee
                );


            // Check if there is 2 users in userList1
            userList1 = _userDao.GetAll();
            Assert.AreEqual(2, userList1.Count);

            // Delete the first user from userList1
            _userDao.Delete(bk_id11);

            // Check if there is 1 user in userList1
            userList1 = _userDao.GetAll();
            Assert.AreEqual(1, userList1.Count);

            // Check if the second User really is the correct one left
            Assert.AreEqual("user2", _userDao.GetById(bk_id12).name);
        }
    }
}