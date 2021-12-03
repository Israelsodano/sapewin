using System;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public class WorkedHoursService : CalculationBase
    {
        public WorkedHoursService(IAppointmentsService appointmentsService) : base(appointmentsService)
        { }

        public IEnumerable<EletronicPoint> CalculateWorkedHoursByEletronicPoint(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsPairsByEletronicPoint)
        {
            foreach (var eletronicPointPairs in eletronicPointsPairsByEletronicPoint)
                yield return CalculateWorkedHours(eletronicPointPairs.Key, eletronicPointPairs);
        }

        private EletronicPoint CalculateWorkedHours(EletronicPoint eletronicPoint, 
                                                    IEnumerable<EletronicPointPairs> pairs)
        {
            if (eletronicPoint.Schedule is null) return eletronicPoint;

            var appointments = pairs.SelectMany(x=> x.GetAppointments()).ToList();
            var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPoint);

            var iiappointment = GetIntervalInAppointment(rappointments, appointments);
            var ioappointment = GetIntervalOutAppointment(rappointments, appointments);

            var wpairs = GetOnlyPairsInDefaultWorkPeriod(pairs, 
                                                        rappointments.eappointment, 
                                                        rappointments.wappointment);

            return SetValuesInEletronicPoint(eletronicPoint, 
                                             wpairs, 
                                             iiappointment.DateHour, 
                                             ioappointment.DateHour);
        }

        private EletronicPoint SetValuesInEletronicPoint(EletronicPoint eletronicPoint, 
                                                         IEnumerable<EletronicPointPairs> wpairs,
                                                         DateTime iiappointment,
                                                         DateTime ioappointment)
        {
            var fpworkedHours = GetWorkedMinutesInFirstPeriod(wpairs, iiappointment);
            var spworkedHours = GetWorkedMinutesInSecondPeriod(wpairs, iiappointment);

            ApplyMinutesOfInterval(eletronicPoint, 
                                  iiappointment, 
                                  ioappointment, 
                                  ref fpworkedHours, 
                                  ref spworkedHours);

            eletronicPoint.PaidHoursFirstPeriodInMinutes = (int)fpworkedHours;
            eletronicPoint.PaidHoursSecondPeriodInMinutes = (int)spworkedHours;

            return eletronicPoint;
        }
            

        private void ApplyMinutesOfInterval(EletronicPoint eletronicPoint, 
                                            DateTime iiappointment, 
                                            DateTime ioappointments, 
                                            ref double fpworkedHours, 
                                            ref double spworkedHours)
        {
            if(eletronicPoint.Schedule.DiscountInterval) return;

            var dinterval = (ioappointments - iiappointment).TotalMinutes;

            if(dinterval % 2 == 0)
            {
                dinterval -= 1;
                spworkedHours += 1;
            }

            fpworkedHours += dinterval / 2;
            spworkedHours += dinterval / 2;
        }

        private double GetWorkedMinutesInFirstPeriod(IEnumerable<EletronicPointPairs> pairs, DateTime iiapointment) =>
            pairs.Where(x=> x.OriginalWayOut.DateHour <= iiapointment).Select(x=> x.GetPeriodMinutes()).Sum();

        private double GetWorkedMinutesInSecondPeriod(IEnumerable<EletronicPointPairs> pairs, DateTime iiapointment) =>
            pairs.Where(x=> x.OriginalEntry.DateHour > iiapointment).Select(x=> x.GetPeriodMinutes()).Sum();


        private IEnumerable<EletronicPointPairs> GetOnlyPairsInDefaultWorkPeriod(IEnumerable<EletronicPointPairs> pairs,
                                                                                 DateTime eappointment,
                                                                                 DateTime wappointment) =>
            pairs.Where(x=> 
                !IsNullPair(x) &&
                x.OriginalEntry.DateHour >= eappointment && 
                x.OriginalWayOut.DateHour <= wappointment);
    }
}