using System.Collections.Generic;
using DPA.Sapewin.CalculationWorkflow.Domain.Common;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events
{
    public class GenericHolidaysWereGottenEvent : CalculationWorkflowMessage
    {
        public IEnumerable<GenericHoliday> GottenGenericHolidays { get; set; }
    }
}