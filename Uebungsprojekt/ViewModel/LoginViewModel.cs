using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.ViewModel
{
    public class LoginViewModel
    {
        /// <summary>User Email for login</summary>
        [EmailAddress]
        [Required(ErrorMessage = "Please enter a valid email adress.")]
        public string email { get; set; } 
        
        // TODO: Add enryption method to password set
        /// <summary>User Password for login</summary>
        [PasswordPropertyText]
        [Required(ErrorMessage = "Please enter your password.")]
        public string password { get; set; }
        
        /// <summary>URL to redirect after successful login</summary>
        public string redirect_url { get; set; }
            
    }
}