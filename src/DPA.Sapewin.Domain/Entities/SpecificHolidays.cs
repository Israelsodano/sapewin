using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class SpecificHoliday : Entity
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public HolidayGroup HolidaysGroup { get; set; }
    }
}