using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Uebungsprojekt.Models
{
    public enum Role
    {
        [Display(Name = "VIP")]
        VIP,
        [Display(Name = "Employee")]
        Employee,
        [Display(Name = "Guest")]
        Guest
    }
}