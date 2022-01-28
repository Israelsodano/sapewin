using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class EletronicPoint : Entity
    {
        public Guid EmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        public double PaidDsr { get; set; }
        public double DiscountedDsr { get; set; }
        public double FirstPeriodPaidExtraHour { get; set; }
        public double SecondPeriodPaidExtraHour { get; set; }
        public double IntervalPaidExtraHour { get; set; }
        public string DesExtraInter { get; set; }
        public string DesExtrapPer1 { get; set; }
        public string DesExtrapPer2 { get; set; }
        public int? IDHorario { get; set; }
        public DateTime Date { get; set; }
        public bool Trated { get; set; }
        public int PaidHoursFirstPeriodInMinutes { get; set; }
        public int PaidHoursSecondPeriodInMinutes { get; set; }
        public string FaltaPagPer1 { get; set; }
        public string FaltaPagPer2 { get; set; }
        public double FirstPeriodDiscountedAbsencesMinutes { get; set; }
        public double SecondPeriodDiscountedAbsencesMinutes { get; set; }
        public string AtrasoPagPer1 { get; set; }
        public string AtrasoPagPer2 { get; set; }
        public double SecondPeriodDiscountedArrearsInMinutes { get; set; }
        public double FirstPeriodDiscountedArrearsInMinutes { get; set; }
        public string SaidaPagPer1 { get; set; }
        public string SaidaPagPer2 { get; set; }
        public double FirstPeriodDiscountedWayOutInMinutes { get; set; }
        public double SecondPeriodDiscountedWayOutInMinutes { get; set; }
        public double FirstPeriodBonusPayment { get; set; }
        public double SecondPeriodBonusPayment { get; set; }
        public double ExtraBonusFirstPeriodPayment { get; set; }
        public double ExtraBonusSecondPeriodPayment { get; set; }
        public double ExtraBonusIntervalPayment { get; set; }
        public string ExtraAdicDesPer1 { get; set; }
        public string ExtraAdicDesPer2 { get; set; }
        public string ExtraAdicDesInter { get; set; }
        public string ReferenciaSemHorario { get; set; }
        public IEnumerable<EletronicPointPairs> Pairs { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }
        public Schedule Schedule { get; set; }
        public Employee Employee { get; set; }
        public string NonScheduleReference { get; set; }

        public double[] GetAbsences() => new[]
        {
            this.FirstPeriodDiscountedAbsencesMinutes,
            this.SecondPeriodDiscountedAbsencesMinutes
        };
        public double[] GetArrears() => new[]
        {
            this.FirstPeriodDiscountedArrearsInMinutes,
            this.SecondPeriodDiscountedArrearsInMinutes,
            this.FirstPeriodDiscountedWayOutInMinutes,
            this.SecondPeriodDiscountedWayOutInMinutes
        };
        public const string DayOff = "DayOff";
        public const string Saturday = "Saturday";
        public const string Sunday = "Sunday";
        public const string Holiday = "Holiday";
    }
}