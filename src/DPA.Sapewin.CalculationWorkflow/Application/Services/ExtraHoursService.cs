using System;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.CalculationWorkflow.Application.Records;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IExtraHoursService
    {
        IEnumerable<EletronicPoint> CalculateExtraHours(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsByPairs);
    }
    public class ExtraHoursService : CalculationBase, IExtraHoursService
    {
        public ExtraHoursService(IAppointmentsService appointmentsService) : base(appointmentsService) { }

        public IEnumerable<EletronicPoint> CalculateExtraHours(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsByPairs)
        {
            foreach (var ep in eletronicPointsByPairs)
                yield return CalculateExtraHour(ep.Key, ep);
        }

        private EletronicPoint CalculateExtraHour(EletronicPoint eletronicPoint, IEnumerable<EletronicPointPairs> pairs)
        {
            var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPoint);
            var extraPairs = GetExtraPairs(pairs, rappointments.eappointment, rappointments.wappointment,
                                GetIntervalInAppointment(rappointments, pairs.SelectMany(x => x.GetAppointments()).ToList()));

            eletronicPoint.FirstPeriodPaidExtraHour = GetFirstExtra(extraPairs, rappointments.eappointment);
            eletronicPoint.SecondPeriodDiscountedArrearsInMinutes = GetSecondExtra(extraPairs, rappointments.wappointment);
            eletronicPoint.IntervalPaidExtraHour = GetExtraInterval(extraPairs, rappointments);

            return HandleExtraHoursLimit(eletronicPoint);
        }
        private EletronicPoint HandleExtraHoursLimit(EletronicPoint eletronicPoint)
        {
            if (SumExtraHours(eletronicPoint) < eletronicPoint.Employee.Parameter.JourneyExtra)
                eletronicPoint.IntervalPaidExtraHour = eletronicPoint.SecondPeriodDiscountedArrearsInMinutes = eletronicPoint.FirstPeriodPaidExtraHour = 0;

            eletronicPoint.FirstPeriodPaidExtraHour = eletronicPoint.FirstPeriodPaidExtraHour < eletronicPoint.Employee.Parameter.TotalExtraHoursFirstPeriod
                ? 0 : eletronicPoint.FirstPeriodPaidExtraHour;
            eletronicPoint.SecondPeriodDiscountedArrearsInMinutes = eletronicPoint.SecondPeriodDiscountedArrearsInMinutes < eletronicPoint.Employee.Parameter.TotalExtraHoursSecondPeriod
                ? 0 : eletronicPoint.SecondPeriodDiscountedArrearsInMinutes;
            eletronicPoint.IntervalPaidExtraHour = eletronicPoint.IntervalPaidExtraHour < eletronicPoint.Employee.Parameter.TotalExtraHoursInterval
                ? 0 : eletronicPoint.IntervalPaidExtraHour;

            return eletronicPoint;
        }
        private double SumExtraHours(EletronicPoint eletronicPoint)
        => eletronicPoint.IntervalPaidExtraHour
            + eletronicPoint.SecondPeriodDiscountedArrearsInMinutes
            + eletronicPoint.FirstPeriodPaidExtraHour;

        private IEnumerable<EletronicPointPairs> GetExtraPairs(IEnumerable<EletronicPointPairs> pairs, DateTime eappointment,
            DateTime wappointment, Appointment iiappointment)
        => from p in pairs
           where !IsNullPair(p) && (p.OriginalEntry ?? p.OriginalWayOut).DateHour < eappointment
                                || (p.OriginalWayOut ?? p.OriginalEntry).DateHour > wappointment
                                || ((p.OriginalEntry ?? p.OriginalWayOut).DateHour >= iiappointment.DateHour
                                && (p.OriginalWayOut ?? p.OriginalEntry).DateHour <= iiappointment.DateHour)
           select p;

        private double GetFirstExtra(IEnumerable<EletronicPointPairs> extraPairs, DateTime eappointment)
        => (from ep in extraPairs
            where (ep.OriginalEntry ?? ep.OriginalWayOut).DateHour < eappointment
            select ep)
            .Sum(ep => DiffMaxHourInMinutes(ep, eappointment));

        private double GetSecondExtra(IEnumerable<EletronicPointPairs> extraPairs, DateTime wappointment)
        => (from ep in extraPairs
            where (ep.OriginalWayOut ?? ep.OriginalEntry).DateHour > wappointment
            select ep).Sum(ep => DiffMinHourInMinutes(ep, wappointment));

        private double GetExtraInterval(IEnumerable<EletronicPointPairs> extraPairs, 
                                        AppointmentsRecord rappointments)
        => (from ep in extraPairs
            where (ep.OriginalEntry ?? ep.OriginalWayOut).DateHour >= rappointments.eappointment
                  && (ep.OriginalWayOut ?? ep.OriginalEntry).DateHour <= rappointments.wappointment
            select ep).Sum(ep => DiffInMinutes(ep));

    }
}