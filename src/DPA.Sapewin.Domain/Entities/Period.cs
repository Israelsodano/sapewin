using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Period : Entity
    {
        public string Entry { get; set; }

        public string IntervalIn { get; set; }

        public string IntervalOut { get; set; }

        public string WayOut { get; set; }
    }
}