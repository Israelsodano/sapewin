using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class SpecificHolidays : Entity
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public GrupodeFeriados HolidaysGroup { get; set; }
    }
}