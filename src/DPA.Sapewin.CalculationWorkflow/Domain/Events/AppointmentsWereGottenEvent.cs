using System.Collections.Generic;
using DPA.Sapewin.CalculationWorkflow.Domain.Common;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Events
{
    public class AppointmentsWereGottenEvent : CalculationWorkflowMessage
    {
        public IEnumerable<Appointment> GottenAppointments { get; set; }

    }
}