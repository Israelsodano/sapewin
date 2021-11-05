using System.Collections;
using System.Collections.Generic;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.Domain.Models
{
    public class EmployeeCalendars
    {
        public Employee Employee { get; set;}
        public IEnumerable<Calendar> Calendars { get; set; }
        public static EmployeeCalendars Build(Employee employee)
        => new() 
        {
            Employee = employee
        };
    }
}