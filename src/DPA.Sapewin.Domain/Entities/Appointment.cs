using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Appointment : Entity
    {       
        public string Nfr { get; set; }

        public int Nsr { get; set; }

        public string UniqueAppointmentKey { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid CompanyId { get; set; }

        public DateTime DateHour { get; set; }

        public static implicit operator Appointment(DirtyNote obj){
            DateTime d = Convert.ToDateTime(obj.Date.ToShortDateString() + " " + obj.Hour);

            return obj == null ? null : new Appointment
            {
                Nfr = obj.Nfr,
                Nsr = obj.Nsr,
                UniqueAppointmentKey = obj.UniqueAppointmentKey,
                DateHour = d,
            };
        }
    }
}