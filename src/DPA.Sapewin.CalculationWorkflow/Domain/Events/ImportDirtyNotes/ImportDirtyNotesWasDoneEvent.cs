using System;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events.ImportDirtyNotes
{
    public interface ImportDirtyNotesWasDoneEvent : CorrelatedBy<Guid>
    {
         
    }
}