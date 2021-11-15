using System;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IArrearService
    {
        IEnumerable<EletronicPoint> CalculateArrearsByEletronicPoint(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsPairsByEletronicPoint);
    }

    public class ArrearsService : CalculationBase, IArrearService
    {

        public ArrearsService(IAppointmentsService appointmentsService) : base(appointmentsService)
        { }

        public IEnumerable<EletronicPoint> CalculateArrearsByEletronicPoint(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsPairsByEletronicPoint)
        {
            foreach (var pairsByEletronicPoint in eletronicPointsPairsByEletronicPoint)
                yield return CalculateArrears(pairsByEletronicPoint.Key, pairsByEletronicPoint);
        }

        private EletronicPoint CalculateArrears(EletronicPoint eletronicPoint,
                                      IEnumerable<EletronicPointPairs> pairs)
        {
            if (eletronicPoint.Schedule is null) return eletronicPoint;

            var appointments = GetNotNullAppointments(pairs);

            var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPoint);

            var eintervals = GetAllEntryApointmentsBasedInEletronicPoint(eletronicPoint,
                                                                        rappointments.eappointment).ToArray();

            var iiappointment = GetIntervalInAppointment(rappointments, appointments);
            var ioappointment = GetIntervalOutAppointment(rappointments, appointments);

            var eappointments = GetRelationAppointmetDates(appointments, eintervals);


            var fparrear = GetFirstPeriodArrears(eletronicPoint,
                                                 eappointments,
                                                 iiappointment,
                                                 ioappointment,
                                                 (rappointments.iiappointment,
                                                  rappointments.ioappointment));

            var sparrear = GetSecondPeriodArrears(eletronicPoint,
                                                 eappointments,
                                                 iiappointment,
                                                 ioappointment,
                                                 (rappointments.iiappointment,
                                                  rappointments.ioappointment));

            return SetValuesInEletronicPoint(eletronicPoint, fparrear, sparrear);
        }

        private EletronicPoint SetValuesInEletronicPoint(EletronicPoint eletronicPoint,
                                               double fparrear,
                                               double sparrear)
        {
            fparrear = fparrear + sparrear <
                            eletronicPoint.Employee.Parameter.JourneyArrear &&
                            fparrear < eletronicPoint.Employee.Parameter.ArrearToleratedInFirstPeriod ? fparrear : 0;

            sparrear = fparrear + sparrear <
                            eletronicPoint.Employee.Parameter.JourneyArrear &&
                            sparrear < eletronicPoint.Employee.Parameter.ArrearToleratedInSecondPeriod ? sparrear : 0;

            eletronicPoint.FirstPeriodDiscountedArrearsInMinutes = fparrear;
            eletronicPoint.SecondPeriodDiscountedArrearsInMinutes = sparrear;

            return eletronicPoint;
        }

        private double GetFirstPeriodArrears(EletronicPoint eletronicPoint,
                                             IEnumerable<RelationAppointmetDate> relationAppointmetsDates,
                                             Appointment iiappointment,
                                             Appointment ioappointment,
                                             (DateTime iiappointment,
                                              DateTime ioappointment) rappointments)
        {
            var fparrear = (from df in
                                (from f in
                                    (from a in relationAppointmetsDates
                                     where a.appointment.DateHour <= iiappointment.DateHour
                                     select a)
                                 select (f.appointment.DateHour - f.rdate).TotalMinutes)
                            where df > 0
                            select df).Sum();

            var dinterval = !eletronicPoint.Employee.FixedInterval ?
                                (rappointments.ioappointment - rappointments.iiappointment).TotalMinutes -
                                (ioappointment.DateHour - iiappointment.DateHour).TotalMinutes :
                                (iiappointment.DateHour - rappointments.iiappointment).TotalMinutes;

            return fparrear + dinterval > 0 ? dinterval : 0;
        }

        private double GetSecondPeriodArrears(EletronicPoint eletronicPoint,
                                             IEnumerable<RelationAppointmetDate> relationAppointmetsDates,
                                             Appointment iiappointment,
                                             Appointment ioappointment,
                                             (DateTime iiappointment,
                                              DateTime ioappointment) rappointments) =>

            (from df in
                (from f in
                    (from a in relationAppointmetsDates
                     where a.appointment.DateHour > iiappointment.DateHour
                     select a)
                 select (f.appointment.DateHour - f.rdate.Date).TotalMinutes)
             where df > 0
             select df).Sum();


        private IEnumerable<DateTime> GetAllEntryApointmentsBasedInEletronicPoint(EletronicPoint eletronicPoint,
                                                                                 DateTime eappointment)
        {
            var ax = (from aux in eletronicPoint.Schedule.AuxiliaryIntervals ?? new AuxiliaryInterval[] { }
                      where aux.DiscountInterval && aux.Kind == AuxiliaryIntervalKind.Fixed
                      select
                      eletronicPoint.Date.AddMinutes(aux.Entry)
                              .AddDays(_appointmentsService
                                          .GetOnlyMinutesFromDateTime(eappointment) > aux.Entry ?
                                              1 : 0
                              )).ToList();

            ax.Add(eappointment);

            return ax;
        }
    }
}