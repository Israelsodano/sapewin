using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IArrearService
    {
        IAsyncEnumerable<EletronicPoint> CalculateArrearsByEletronicPoint(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsPairsByEletronicPoint);
    }

    public class ArrearsService : IArrearService
    {
        private readonly IAppointmentsService _appointmentsService;
        private readonly IUnitOfWork<EletronicPoint> _unitOfWorkEletronicPoint;

        public ArrearsService(IAppointmentsService appointmentsService,
                              IUnitOfWork<EletronicPoint> unitOfWorkEletronicPoint)
        {
            _appointmentsService = appointmentsService ?? throw new ArgumentNullException(nameof(appointmentsService));
            _unitOfWorkEletronicPoint = unitOfWorkEletronicPoint ?? throw new ArgumentNullException(nameof(appointmentsService));
        }

        public async IAsyncEnumerable<EletronicPoint> CalculateArrearsByEletronicPoint(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsPairsByEletronicPoint)
        {
            foreach (var pairsByEletronicPoint in eletronicPointsPairsByEletronicPoint)
            {
                var celetronicPoint = CalculateArrears(pairsByEletronicPoint.Key,
                                                       pairsByEletronicPoint);

                celetronicPoint = _unitOfWorkEletronicPoint.Repository.Update(celetronicPoint);
                await _unitOfWorkEletronicPoint.SaveChangesAsync();

                yield return celetronicPoint;
            }
        }

        private EletronicPoint CalculateArrears(EletronicPoint eletronicPoint, 
                                      IEnumerable<EletronicPointPairs> pairs)
        {
            if(eletronicPoint.Schedule is null) return eletronicPoint;

            var appointments = (from p in pairs.SelectMany(x=> x.GetAppointments()) 
                               where p is not null
                               select p).ToList();  

            var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPoint);
                               
            var eintervals = GetAllEntryApointmentsBasedInEletronicPont(eletronicPoint, 
                                                                        rappointments.eappointment).ToArray();
            
            var iiappointment = GetIntervalInAppointment(rappointments, appointments);
            var ioappointment = GetIntervalOutAppointment(rappointments, appointments);

            var eappointments = from ea in eintervals  
                      select new RelationAppointmetDate(_appointmentsService.GetBestAppointment(ea, 
                                                                     ref appointments, 
                                                                     true, 
                                                                     eintervals), 
                                                        ea);
            
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
                            eletronicPoint.Employee.Parameter.ArrearJourney &&
                            fparrear < eletronicPoint.Employee.Parameter.ArrearToleratedInFirstPeriod ? fparrear : 0;
                            
            sparrear = fparrear + sparrear < 
                            eletronicPoint.Employee.Parameter.ArrearJourney && 
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
                            select df > 0 ? df : 0).Sum();
            
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
            select df > 0 ? df : 0).Sum();

        private Appointment GetIntervalInAppointment((DateTime eappointment, 
                                                      DateTime iiapointment, 
                                                      DateTime ioappointment, 
                                                      DateTime wappointment) rappointments,
                                                      List<Appointment> appointments) =>
            _appointmentsService.GetBestAppointment(rappointments.iiapointment, 
                                                    ref appointments, 
                                                    true, 
                                                    rappointments.eappointment,
                                                    rappointments.iiapointment,
                                                    rappointments.ioappointment,
                                                    rappointments.wappointment);

        private Appointment GetIntervalOutAppointment((DateTime eappointment, 
                                                      DateTime iiapointment, 
                                                      DateTime ioappointment, 
                                                      DateTime wappointment) rappointments,
                                                      List<Appointment> appointments) =>
            _appointmentsService.GetBestAppointment(rappointments.ioappointment, 
                                                    ref appointments, 
                                                    true, 
                                                    rappointments.eappointment,
                                                    rappointments.iiapointment,
                                                    rappointments.ioappointment,
                                                    rappointments.wappointment);
        

        private IEnumerable<DateTime> GetAllEntryApointmentsBasedInEletronicPont(EletronicPoint eletronicPoint, 
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