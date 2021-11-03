using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class ProximityCard : Entity
    {
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CardNumber { get; set; }
        public Employee Employee { get; set; }
    }
}