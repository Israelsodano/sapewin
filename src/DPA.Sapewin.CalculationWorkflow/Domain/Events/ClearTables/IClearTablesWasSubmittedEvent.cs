using System;
using DPA.Sapewin.Domain.Models.Enums;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events.ClearTables
{
    public interface IClearTablesWasSubmittedEvent : CorrelatedBy<Guid>
    { 
        Guid[] Employees { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        ProcessingType ProcessingType { get; set; }
    }
}