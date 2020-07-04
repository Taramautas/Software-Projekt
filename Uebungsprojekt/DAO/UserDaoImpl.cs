using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class UserDaoImpl : UserDao
    {
        private IMemoryCache _cache;

        public UserDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Adds a user to the Userlist if there is one, else it creates a new List and adds the user
        /// </summary>
        /// <param name="user">User that is to be added</param>
        /// <returns>the added User</returns>
        public User Create(User user)
        {
            if (_cache.TryGetValue("CreateUser", out List<User> createdUsers))
            {
                createdUsers.Add(user);
                return user;
            }
            else
            {
                createdUsers = new List<User> { user };
                _cache.Set("CreateUser", createdUsers);
                return user;
            }
        }

        /// <summary>
        /// Delets the User with specified Id
        /// </summary>
        /// <param name="_Id">User Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateUser", out List<User> createdUsers))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdUsers.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of Users in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of Users</returns>
        public List<User> GetAll()
        {
            if (_cache.TryGetValue("CreateUser", out List<User> createdUsers))
            {
                return createdUsers;
            }
            else
            {
                createdUsers = new List<User>();
                _cache.Set("CreateUser", createdUsers);
                return createdUsers;
            }
        }

        /// <summary>
        /// Finds a User with specified ID and returns it
        /// </summary>
        /// <param name="_Id">User Id</param>
        /// <returns>User with specified Id on success and null on failure</returns>
        public User GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateUser", out List<User> createdUsers))
            {

                return createdUsers.Find(x => x.Id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
