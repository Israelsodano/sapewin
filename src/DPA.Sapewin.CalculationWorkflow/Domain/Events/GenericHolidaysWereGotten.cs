using System;
using System.Collections.Generic;
using DPA.Sapewin.Domain.Entities;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events
{
    public class GenericHolidaysWereGotten : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public IEnumerable<GenericHoliday> GenericHolidays { get; set; }
    }
}