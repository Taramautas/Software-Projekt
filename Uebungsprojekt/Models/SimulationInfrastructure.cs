namespace Uebungsprojekt.Models
{
    public class SimulationInfrastructure
    {
        public int id { get; set; }
        public int location_dao_id { get; set; }
        public int charging_zone_dao_id { get; set; }
        public int charging_column_dao_id { get; set; }
    }
}