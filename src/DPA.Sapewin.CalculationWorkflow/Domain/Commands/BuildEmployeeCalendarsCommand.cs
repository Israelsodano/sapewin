using System;
using System.Collections.Generic;
using DPA.Sapewin.Domain.Entities;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Commands
{
    public class BuildEmployeeCalendarsCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<GenericHoliday> GenericHolidays { get; set; }

        public static BuildEmployeeCalendarsCommand
            BuildFromStartProcess(StartProcessCommand command)
        => command is null ? null : new()
        {
            Employees = command.Employees,
            EndDate = command.EndDate,
            GenericHolidays = command.GenericHolidays,
            StartDate = command.StartDate
        };
    }
}