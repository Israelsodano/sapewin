using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class SpecificHoliday : Entity
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public HolidayGroup HolidaysGroup { get; set; }
    }
}