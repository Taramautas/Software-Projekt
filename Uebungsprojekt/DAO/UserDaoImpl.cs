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
        /// Creates and adds a User with new Id to the Userlist if there is one, else it creates a new List and adds the User
        /// </summary>
        /// <returns>the id of the added User</returns>
        public int Create(string _name, string _email, string _password, Role _role)
        {
            if (_cache.TryGetValue("CreateUserIds", out int ids))
            {
                ++ids;
                _cache.Set("CreateUserIds", ids);
                _cache.TryGetValue("CreateUser", out List<User> createdUsers);
                User newUser = new User
                {
                    id = ids,
                    name = _name,
                    email = _email,
                    password = _password,
                    role = _role,
                };
                createdUsers.Add(newUser);
                return ids;
            }

            else
            {
                List<User> createdUsers = new List<User>();
                ids = 0;
                User newUser = new User
                {
                    id = ++ids,
                    name = _name,
                    email = _email,
                    password = _password,
                    role = _role,
                };
                createdUsers.Add(newUser);
                _cache.Set("CreateUser", createdUsers);
                _cache.Set("CreateUserIds", ids);
                return ids;
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

                return createdUsers.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
