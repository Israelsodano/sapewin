using System;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IEarlyWayOutService
    {

    }
    public class EarlyWayOutService : CalculationBase, IEarlyWayOutService
    {
        public EarlyWayOutService(IAppointmentsService appointmentService) : base(appointmentService)
        {

        }
        public IEnumerable<EletronicPoint> CalculateEarlyWayOut(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointPairsGroups, IEnumerable<EmployeeCalendars> ecalendars)
        {
            foreach (var ep in eletronicPointPairsGroups)
                yield return CalculateEarlyWayOut(ep.Key, ep);
        }


        private EletronicPoint CalculateEarlyWayOut(EletronicPoint eletronicPoint, IEnumerable<EletronicPointPairs> pairs)
        {
            if (eletronicPoint.Schedule is null) return eletronicPoint;

            var appointments = GetNotNullAppointments(pairs);

            var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPoint);

            var eintervals = GetAllWayOutApointmentsBasedInEletronicPoint(eletronicPoint, rappointments).ToArray();

            var iiappointment = GetIntervalInAppointment(rappointments, appointments);
            var ioappointment = GetIntervalOutAppointment(rappointments, appointments);

            var eappointments = GetRelationAppointmetDates(appointments, eintervals);

            var fEarlyWayOut = GetPeriodEarlyWayOut(eletronicPoint, from a in eappointments
                                                                    where a.appointment.DateHour <= iiappointment.DateHour
                                                                    select a);

            var sEarlyWayOut = GetPeriodEarlyWayOut(eletronicPoint, from a in eappointments
                                                                    where a.appointment.DateHour > iiappointment.DateHour
                                                                    select a);

            return SetValuesInEletronicPoint(eletronicPoint, fEarlyWayOut, sEarlyWayOut);
        }
        private IEnumerable<DateTime> GetAllWayOutApointmentsBasedInEletronicPoint(EletronicPoint eletronicPoint,
            (DateTime eappointment, DateTime iiappointment, DateTime ioappointment, DateTime wappointment) rappointments)
        {
            var ax = (from aux in eletronicPoint.Schedule.AuxiliaryIntervals ?? new AuxiliaryInterval[] { }
                      where aux.DiscountInterval && aux.Kind == AuxiliaryIntervalKind.Fixed
                      select
                      eletronicPoint.Date.AddMinutes(aux.WayOut)
                              .AddDays(_appointmentsService
                                          .GetOnlyMinutesFromDateTime(rappointments.eappointment) > aux.Entry ?
                                              1 : 0
                              )).ToList();

            ax.AddRange(new[] { rappointments.ioappointment, rappointments.wappointment });

            return ax;
        }
        private double GetPeriodEarlyWayOut(EletronicPoint eletronicPoint,
                                              IEnumerable<RelationAppointmetDate> filteredRelationAppointmentsDates)
        => eletronicPoint.Employee.FixedInterval
        ? (from df in
                (from f in
                    (filteredRelationAppointmentsDates)
                 select (f.appointment.DateHour - f.rdate).TotalMinutes)
           where df > 0
           select df).Sum()
        : 0;

        private EletronicPoint SetValuesInEletronicPoint(EletronicPoint eletronicPoint,
                                               double fpwayout,
                                               double swayout)
        {
            fpwayout = fpwayout + swayout <
                            eletronicPoint.Employee.Parameter.JourneyWayOut &&
                            fpwayout < eletronicPoint.Employee.Parameter.WayOutToleratedInFirstPeriod ? fpwayout : 0;

            swayout = fpwayout + swayout <
                            eletronicPoint.Employee.Parameter.JourneyWayOut &&
                            swayout < eletronicPoint.Employee.Parameter.WayOutToleratedInSecondPeriod ? swayout : 0;

            eletronicPoint.FirstPeriodDiscountedWayOutInMinutes = fpwayout;
            eletronicPoint.SecondPeriodDiscountedWayOutInMinutes = swayout;

            return eletronicPoint;
        }
    }
}