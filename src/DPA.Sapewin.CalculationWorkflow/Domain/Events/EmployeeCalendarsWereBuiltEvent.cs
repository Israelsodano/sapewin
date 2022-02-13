using System.Collections.Generic;
using DPA.Sapewin.CalculationWorkflow.Domain.Common;
using DPA.Sapewin.Domain.Models;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events
{
    public class EmployeeCalendarsWereBuiltEvent : CalculationWorkflowMessage
    {
        public IAsyncEnumerable<EmployeeCalendars> BuiltEmployeeCalendars { get; set; }

    }
}