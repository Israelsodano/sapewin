using System;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Commands
{
    public class GetGenericHolidaysCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}