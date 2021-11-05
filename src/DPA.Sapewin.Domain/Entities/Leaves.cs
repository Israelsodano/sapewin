using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Leave : Entity
    {
        public Guid EmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        public string Abbreviation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Employee Employee { get; set; }
    }
}