using System;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface INightlyBonusService
    {

    }
    public class NightlyBonusService : CalculationBase, INightlyBonusService
    {
        public NightlyBonusService(IAppointmentsService appointmentsService) : base(appointmentsService)
        { }

        public IEnumerable<EletronicPoint> CalculateNightlyBonuss(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> groups)
        {
            foreach (var pairsByEletronicPoint in groups)
                yield return CalculateNightlyBonus(pairsByEletronicPoint.Key, pairsByEletronicPoint);
        }
        private EletronicPoint CalculateNightlyBonus(EletronicPoint eletronicPoint, IEnumerable<EletronicPointPairs> pairs)
        {
            var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPoint);

            var totalNightlyBonusPairs = GetTotalNightlyBonusPairs(pairs, eletronicPoint);
            var extraBonusPairs = GetExtraBonusPairs(totalNightlyBonusPairs, rappointments);

            eletronicPoint.FirstPeriodBonusPayment = GetFirstPeriodBonus(totalNightlyBonusPairs, rappointments);
            eletronicPoint.SecondPeriodBonusPayment = GetSecondPeriodBonus(totalNightlyBonusPairs, rappointments);

            eletronicPoint.ExtraBonusFirstPeriodPayment = GetExtraBonusFirstPeriod(extraBonusPairs, rappointments);
            eletronicPoint.ExtraBonusIntervalPayment = GetExtraBonusInterval(extraBonusPairs, rappointments);
            eletronicPoint.ExtraBonusSecondPeriodPayment = GetExtraBonusSecondPeriod(extraBonusPairs, rappointments);

            return HandleJourneyExtraCase(eletronicPoint);
        }
        private EletronicPoint HandleJourneyExtraCase(EletronicPoint ep)
        {
            if (SumBonusPayments(ep) < ep.Employee.Parameter.JourneyExtra)
                ep.ExtraBonusIntervalPayment = ep.FirstPeriodBonusPayment = ep.SecondPeriodBonusPayment = 0;
            return ep;
        }
        private double SumBonusPayments(EletronicPoint ep)
        => ep.ExtraBonusIntervalPayment + ep.FirstPeriodBonusPayment + ep.SecondPeriodBonusPayment;
        private double GetSecondPeriodBonus(in IEnumerable<EletronicPointPairs> totalNightlyBonusPairs,
            (DateTime entryAppointmentDate, DateTime intervalInAppointmentDate,
            DateTime intervalOutAppointmentDate, DateTime wayOutAppointmentDate) rappointments)
        => (from pair in totalNightlyBonusPairs
            where pair.HigherAppointment.DateHour >= rappointments.intervalOutAppointmentDate
                && pair.HigherAppointment.DateHour <= rappointments.wayOutAppointmentDate
            select pair)
            .Sum(x => DiffInMinutes(x));
        private double GetFirstPeriodBonus(in IEnumerable<EletronicPointPairs> totalNightlyBonusPairs,
            (DateTime entryAppointmentDate, DateTime intervalInAppointmentDate,
            DateTime intervalOutAppointmentDate, DateTime wayOutAppointmentDate) rappointments)
        => (from pair in totalNightlyBonusPairs
            where pair.HigherAppointment.DateHour >= rappointments.entryAppointmentDate
                && pair.HigherAppointment.DateHour <= rappointments.intervalInAppointmentDate
            select pair)
            .Sum(x => DiffInMinutes(x));
        private double GetExtraBonusInterval(in IEnumerable<EletronicPointPairs> extraBonusPairs,
            (DateTime entryAppointmentDate, DateTime intervalInAppointmentDate,
            DateTime intervalOutAppointmentDate, DateTime wayOutAppointmentDate) rappointments)
        => (from pair in extraBonusPairs
            where pair.LowerAppointment.DateHour >= rappointments.intervalInAppointmentDate
                && pair.HigherAppointment.DateHour <= rappointments.wayOutAppointmentDate
            select pair)
            .Sum(x => DiffInMinutes(x));
        private double GetExtraBonusSecondPeriod(in IEnumerable<EletronicPointPairs> extraBonusPairs,
            (DateTime entryAppointmentDate, DateTime intervalInAppointmentDate,
            DateTime intervalOutAppointmentDate, DateTime wayOutAppointmentDate) rappointments)
        => (from pair in extraBonusPairs
            where pair.LowerAppointment.DateHour >= rappointments.intervalOutAppointmentDate
            select pair)
            .Sum(x => DiffMinHourInMinutes(x, rappointments.wayOutAppointmentDate));
        private double GetExtraBonusFirstPeriod(in IEnumerable<EletronicPointPairs> extraBonusPairs,
            (DateTime entryAppointmentDate, DateTime intervalInAppointmentDate,
            DateTime intervalOutAppointmentDate, DateTime wayOutAppointmentDate) rappointments)
        => (from pair in extraBonusPairs
            where pair.HigherAppointment.DateHour <= rappointments.intervalInAppointmentDate
            select pair)
            .Sum(x => DiffMaxHourInMinutes(x, rappointments.entryAppointmentDate));
        private DateTime GetStartBonusDate(in EletronicPoint ep)
        => ep.Employee.Parameter.StartNightlyBonus < ep.Schedule.Period.Entry
            ? ep.Date.AddMinutes(ep.Employee.Parameter.StartNightlyBonus).AddDays(1)
            : ep.Date.AddMinutes(ep.Employee.Parameter.StartNightlyBonus);
        private DateTime GetEndBonus(in EletronicPoint ep)
        => ep.Employee.Parameter.EndNightlyBonus < ep.Schedule.Period.Entry
            ? ep.Date.AddMinutes(ep.Employee.Parameter.EndNightlyBonus).AddDays(1)
            : ep.Date.AddMinutes(ep.Employee.Parameter.EndNightlyBonus);

        private IEnumerable<EletronicPointPairs> GetTotalNightlyBonusPairs(in IEnumerable<EletronicPointPairs> pairs, EletronicPoint ep)
        {
            var startBonusDate = GetStartBonusDate(ep);
            var endBonusDate = GetEndBonus(ep);

            return from pair in pairs
                   where !IsNullPair(pair)
                   && (pair.LowerAppointment.DateHour >= startBonusDate
                   && (ep.Employee.Parameter.EndOfWorkBonus
                       || (pair.OriginalWayOut ?? pair.OriginalEntry).DateHour <= endBonusDate))
                   select pair;
        }


        private IEnumerable<EletronicPointPairs> GetExtraBonusPairs(in IEnumerable<EletronicPointPairs> totalNightlyBonusPairs,
            (DateTime entryAppointmentDate, DateTime intervalInAppointmentDate, DateTime intervalOutAppointmentDate, DateTime wayOutAppointmentDate) rappointments)
        => from pair in totalNightlyBonusPairs
           where pair.LowerAppointment.DateHour < rappointments.entryAppointmentDate
            || (pair.OriginalWayOut ?? pair.OriginalEntry).DateHour > rappointments.wayOutAppointmentDate
            || ((pair.LowerAppointment.DateHour >= rappointments.intervalInAppointmentDate
            && pair.HigherAppointment.DateHour <= rappointments.intervalOutAppointmentDate))
           select pair;
    }
}