using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public enum ConnectorType
    {
        [Display(Name = "Schuko Socket")]
        Schuko_Socket,
        [Display(Name = "Type 1 Plug")]
        Type_1_Plug,
        [Display(Name = "Type 2 Plug")]
        Type_2_Plug,
        [Display(Name = "CHAdeMO Plug")]
        CHAdeMO_Plug,
        [Display(Name = "Tesla Supercharger")]
        Tesla_Supercharger,
        [Display(Name = "CCS Combo 2 Plug")]
        CCS_Combo_2_Plug
    }
}