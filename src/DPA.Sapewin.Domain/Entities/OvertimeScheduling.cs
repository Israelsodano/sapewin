using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class OvertimeScheduling : Entity
    {
        public Guid ParameterId { get; set; }
        public Guid CompanyId { get; set; }
        public string Hours { get; set; }
        public int Percentage { get; set; }
        public int? Additional { get; set; }
        public OvertimeSchedulingType Type { get; set; }
        public Parameters Parameters { get; set; }
    }
    public enum OvertimeSchedulingType
    {
        WorkingDays = 1, Saturdays = 2, Sundays = 3, Holidays = 4, DaysOff = 5
    }
}