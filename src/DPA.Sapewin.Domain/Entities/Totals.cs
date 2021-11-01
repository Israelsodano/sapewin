using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Totals : Entity
    {
        public int TotalMinutesInFirstPeriod { get; set; }

        public int TotalMinutesInSecondPeriod { get; set; }

        public int TotalMinutesInInterval { get; set; }

        public int TotalMinutesInJourney { get; set; }
    }
}