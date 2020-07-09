using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface UserDao
    {

        // All usable Methods of UserDao
        // Implemented in UserDaoImpl
        User GetById(int Id);
        List<User> GetAll();
        int Create(string _name, string _email, string _password, Role _role, List<Vehicle> _vehicles);
        bool Delete(int Id);
    }
}
