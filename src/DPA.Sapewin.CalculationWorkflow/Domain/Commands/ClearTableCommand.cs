using System;
using MassTransit;
using System.Collections.Generic;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models.Enums;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Commands.Calculation
{
    public abstract class ClearTableCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public ProcessingTypes processingType { get; set; }
    }
}