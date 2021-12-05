using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Scale : Entity
    {
        public Guid CompanyId { get; set; }
        public Guid TurnId { get; set; }
        public string Description { get; set; }
        public ScalesType Type { get; set; }
        public DateTime StartDate { get; set; }
        public Livs Liv { get; set; }
        public Turns Turn { get; set; }
        public IEnumerable<ScheduleScales> ScheduleScales { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
    }
    public enum ScalesType
    {
        Weekly = 1, Relay = 2, WeeklyLoad = 3, Free = 4
    };


}