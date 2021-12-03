using DPA.Sapewin.Repository;
using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class EletronicPointPairs : Entity
    {
        public Guid EletronicPointId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? EntryAppointmentId { get; set; }
        public Guid? WayOutAppointmentId { get; set; }
        public string AbrMotivoAbonoEnt { get; set; }
        public string AbrMotivoAbonoSai { get; set; }
        public int Order { get; set; }
        public EletronicPoint EletronicPoint { get; set; }
        public Appointment OriginalEntry { get; set; }
        public Appointment OriginalWayOut { get; set; }
        public Appointment[] GetAppointments() => new [] { OriginalEntry, OriginalWayOut };
        public double GetPeriodMinutes() => 
            OriginalEntry is null || OriginalWayOut is null ? 0 : 
            (OriginalWayOut.DateHour - OriginalEntry.DateHour).TotalMinutes;
    }
}