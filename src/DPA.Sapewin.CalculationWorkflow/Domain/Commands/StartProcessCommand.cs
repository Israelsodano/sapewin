using System;
using System.Collections.Generic;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models.Enums;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Commands
{
    public class StartProcessCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public ProcessingTypes ProcessingType { get; set; }
        public IEnumerable<GenericHoliday> GenericHolidays { get; set; }
    }
}