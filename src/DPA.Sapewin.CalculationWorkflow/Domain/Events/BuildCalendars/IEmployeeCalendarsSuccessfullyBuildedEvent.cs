using System;
using DPA.Sapewin.Domain.Models;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events.BuildCalendars
{
    public interface IEmployeeCalendarsSuccessfullyBuildedEvent : CorrelatedBy<Guid>
    {
        EmployeeCalendars[] EmployeeCalendars { get; set; }
    }
}