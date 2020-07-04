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
        User Create(User user);
        bool Delete(int Id);
    }
}
