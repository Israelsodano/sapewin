using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class ProximityCard : Entity
    {
        public virtual int EmployeeId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual string CardNumber { get; set; }
        public virtual Employees Employee { get; set; }
    }
}