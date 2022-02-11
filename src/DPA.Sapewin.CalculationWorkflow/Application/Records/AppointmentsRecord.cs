using System;

namespace DPA.Sapewin.CalculationWorkflow.Application.Records
{
    public record AppointmentsRecord(DateTime eappointment, 
                               DateTime iiappointment, 
                               DateTime ioappointment, 
                               DateTime wappointment);
}