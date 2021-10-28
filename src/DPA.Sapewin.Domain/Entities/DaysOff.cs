using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class DaysOff : Entity
    {
        public Guid EmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime Date { get; set; }
        public Employees Employee { get; set; }
    }
}