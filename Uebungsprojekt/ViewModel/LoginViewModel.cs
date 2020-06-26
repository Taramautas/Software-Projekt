using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.ViewModel
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Please enter a valid email adress.")]
        public string email { get; set; } 
        
        [PasswordPropertyText]
        [Required(ErrorMessage = "Please enter your password.")]
        public string password { get; set; }
        
        public string redirect_url { get; set; }
            
    }
}