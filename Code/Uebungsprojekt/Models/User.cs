using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class User 
    {
        public int id { get; set; }
        
        [Required(ErrorMessage = "Please enter the users name.")]
        public string name { get; set; }
        
        [Required(ErrorMessage = "Please enter the users email address.")]
        [EmailAddress]
        public string email { get; set; }
        
        [Required(ErrorMessage = "Please define a password.")]
        public string password { get; set; }
        
        [Required(ErrorMessage = "Please specify the users role.")]
        public Role role { get; set; }

    }
}