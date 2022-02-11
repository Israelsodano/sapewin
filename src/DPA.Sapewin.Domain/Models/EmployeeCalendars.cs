using System.Linq;
using System;
using System.Collections.Generic;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.Domain.Models
{
    public struct EmployeeCalendars
    {
        public Employee Employee { get; set; }
        public IEnumerable<Calendar> Calendars { get; set; }

        public Calendar GetNearestCalendarWithSchedule(DateTime date) =>
            (from d in (from c in Calendars
                        orderby c.Date
                        where c.Schedule is not null
                        select new
                        {
                            Difference = (c.Date.Date - date.Date).TotalDays,
                            Target = c
                        })
             orderby d.Difference
             select d).FirstOrDefault().Target;

        public static EmployeeCalendars Build(Employee employee)
        => new()
        {
            Employee = employee
        };
    }
}