using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class EletronicPoint : Entity
    {
        public Guid EmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        public string DsrPago { get; set; }
        public string DsrDescontado { get; set; }
        public string PagExtrapPer1 { get; set; }
        public string PagExtrapPer2 { get; set; }
        public string PagExtrapIntervalo { get; set; }
        public string DesExtraInter { get; set; }
        public string DesExtrapPer1 { get; set; }
        public string DesExtrapPer2 { get; set; }
        public int? IDHorario { get; set; }
        public DateTime Date { get; set; }
        public bool Tratado { get; set; }
        public string HoraPagPer1 { get; set; }
        public string HoraPagPer2 { get; set; }
        public string FaltaPagPer1 { get; set; }
        public string FaltaPagPer2 { get; set; }
        public string FaltaDesPer1 { get; set; }
        public string FaltaDesPer2 { get; set; }
        public string AtrasoPagPer1 { get; set; }
        public string AtrasoPagPer2 { get; set; }
        public double SecondPeriodDiscountedArrearsInMinutes { get; set; }
        public double FirstPeriodDiscountedArrearsInMinutes { get; set; }
        public string SaidaPagPer1 { get; set; }
        public string SaidaPagPer2 { get; set; }
        public string SaidaDesPer1 { get; set; }
        public string SaidaDesPer2 { get; set; }
        public string AdicionalPagPer1 { get; set; }
        public string AdicionalPagPer2 { get; set; }
        public string ExtraAdicPagPer1 { get; set; }
        public string ExtraAdicPagPer2 { get; set; }
        public string ExtraAdicPagInter { get; set; }
        public string ExtraAdicDesPer1 { get; set; }
        public string ExtraAdicDesPer2 { get; set; }
        public string ExtraAdicDesInter { get; set; }
        public string ReferenciaSemHorario { get; set; }
        public IEnumerable<EletronicPointPairs> Pairs { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }
        public Schedule Schedule { get; set; }
        public Employee Employee { get; set; }

        public const string DayOff = "DayOff";
        public const string Saturday = "Saturday";
        public const string Sunday = "Sunday";
        public const string Holiday = "Holiday";
    }
}