using System;
using DPA.Sapewin.CalculationWorkflow.Domain.Common;
using DPA.Sapewin.Domain.Models.Enums;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events
{
    public interface IStartProcessEventWasSubmitted : CorrelatedBy<Guid>
    {
        Guid[] Employees { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        ProcessingType ProcessingType { get; set; }
    }
}