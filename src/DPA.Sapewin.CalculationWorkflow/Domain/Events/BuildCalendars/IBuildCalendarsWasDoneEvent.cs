using System;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events.BuildCalendars
{
    public interface IBuildCalendarsWasDoneEvent : CorrelatedBy<Guid>
    {
         
    }
}