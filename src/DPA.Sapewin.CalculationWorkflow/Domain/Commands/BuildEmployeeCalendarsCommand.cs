using System;
using System.Collections.Generic;
using DPA.Sapewin.CalculationWorkflow.Domain.Common;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Commands
{
    public class BuildEmployeeCalendarsCommand : CalculationWorkflowMessage
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<GenericHoliday> GenericHolidays { get; set; }


        public static BuildEmployeeCalendarsCommand BuildInitialized(
            DateTime startDate, DateTime endDate,
            IEnumerable<Employee> employees,
            IEnumerable<GenericHoliday> genericHolidays)
        => new()
        {
            StartDate = startDate,
            EndDate = endDate,
            Employees = employees,
            GenericHolidays = genericHolidays
        };

    }
}