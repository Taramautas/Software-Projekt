using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public enum Role
    {
        [Display(Name = "VIP")]
        VIP,
        [Display(Name = "Employee")]
        Employee,
        [Display(Name = "Guest")]
        Guest,
        [Display(Name = "Planner")]
        Planner,
        [Display(Name = "Assistant")]
        Assistant
    }
}