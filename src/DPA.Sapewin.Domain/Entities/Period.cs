using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Period : Entity
    {
        public int Entry { get; set; }

        public int? IntervalIn { get; set; }

        public int? IntervalOut { get; set; }

        public int WayOut { get; set; }
    }
}