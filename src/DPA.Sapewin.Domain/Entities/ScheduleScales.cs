using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class ScheduleScales : Entity
    {
        public Guid CompanyId { get; set; }
        public Guid ScaleId { get; set; }
        public Guid ScheduleId { get; set; }
        public int Order { get; set; }
        public int DaysAmount { get; set; }
        public string Days { get; set; }
        public bool Direct { get; set; }
        public ScheduleScalesStartDay StartDay { get; set; }
        public DateTime EntryTime { get; set; }
        public Companie Company { get; set; }
        public Scales Scales { get; set; }
    }
    public enum ScheduleScalesStartDay
    {
        Segunda = 1, Terca = 2, Quarta = 3, Quinta = 4, Sexta = 5, Sabado = 6, Domingo = 7
    }
}