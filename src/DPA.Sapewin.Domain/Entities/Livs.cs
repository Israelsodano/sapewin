using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Livs : Entity
    {
        public bool AdcSaturday { get; set; }
        public bool AdcSunday { get; set; }
        public bool Holiday { get; set; }
        public bool DayOff { get; set; }
        public bool Turn { get; set; }
    }
}