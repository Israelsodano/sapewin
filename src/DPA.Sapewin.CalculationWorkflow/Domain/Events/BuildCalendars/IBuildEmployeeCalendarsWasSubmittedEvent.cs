using System;
using System.Collections.Generic;
using DPA.Sapewin.CalculationWorkflow.Domain.Common;
using DPA.Sapewin.Domain.Entities;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events.BuildCalendars
{
    public interface IBuildEmployeeCalendarsWasSubmittedEvent : CorrelatedBy<Guid>
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        Guid[] Employees { get; set; }
    }
}