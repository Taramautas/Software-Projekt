using Microsoft.AspNetCore.Identity;

namespace Uebungsprojekt.Models
{
    public class User 
    {
        public int id { get; set; }
        
        public string name { get; set; }
        
        public string email { get; set; }
        
        public string password { get; set; }
        public Role role { get; set; }
        
        public User()
        {
            
        }
        
    }
}