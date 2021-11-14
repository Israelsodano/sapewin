using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class DayOff : Entity
    {
        public Guid EmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime Date { get; set; }
        public Employee Employee { get; set; }
    }
}