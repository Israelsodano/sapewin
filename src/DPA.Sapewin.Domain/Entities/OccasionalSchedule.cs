using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class OccasionalSchedule : Entity
    {
        public Guid EmployeeId { get; set; }

        public Guid ScheduleId { get; set; }

        public Guid CompanyId { get; set; }

        public DateTime Date { get; set; }

        public Employee Employee { get; set; }

        public Schedule Schedule { get; set; }
    }
}