using System;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events.ImportDirtyNotes
{
    public interface IImportDirtyNotesSubmittedEvent : CorrelatedBy<Guid>
    {
         
        Guid[] Employees { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }
}