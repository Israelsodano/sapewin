using System;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events.ClearTables
{
    public interface ITablesWasClearedSuccessfullyEvent : CorrelatedBy<Guid>
    {
         
    }
}