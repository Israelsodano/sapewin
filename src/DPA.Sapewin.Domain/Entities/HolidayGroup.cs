using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class HolidayGroup : Entity
    {
        public string Description { get; set; }

        public IList<SpecificHoliday> SpecificHolidays { get; set; }
    }
}