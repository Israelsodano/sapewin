using System;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Common
{
    public abstract class CalculationWorkflowMessage : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }


    }
}