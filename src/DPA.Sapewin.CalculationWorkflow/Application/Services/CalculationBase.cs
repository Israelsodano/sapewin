using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPA.Sapewin.CalculationWorkflow.Application.Records;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface ICalculationBase
    {
        Task<IEnumerable<EletronicPoint>> Calculate(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsByEmployees);
    }

    public abstract class CalculationBase : ICalculationBase
    {
        protected readonly IAppointmentsService _appointmentsService;
        protected readonly IUnitOfWork<EletronicPoint> _unitOfWorkEletronicPoint;

        public CalculationBase(IAppointmentsService appointmentsService, 
                               IUnitOfWork<EletronicPoint> unitOfWorkEletronicPoint)
        {
            _appointmentsService = appointmentsService ?? throw new ArgumentNullException(nameof(appointmentsService));
            _unitOfWorkEletronicPoint = unitOfWorkEletronicPoint ?? throw new ArgumentNullException(nameof(unitOfWorkEletronicPoint));
        }

        public abstract Task<IEnumerable<EletronicPoint>> Calculate(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsByEmployees);

        protected Task SaveEletronicPointsAsync(IEnumerable<EletronicPoint> eletronicPoints)
        {
            _unitOfWorkEletronicPoint.Repository.Update(eletronicPoints);
            return _unitOfWorkEletronicPoint.SaveChangesAsync();
        }

        protected double DiffInMinutes(EletronicPointPairs pair)
        => pair.OriginalEntry is null || pair.OriginalWayOut is null
        ? 0 : (pair.OriginalWayOut.DateHour - pair.OriginalEntry.DateHour).TotalMinutes;


        protected double DiffMaxHourInMinutes(EletronicPointPairs pair, DateTime maxHour)
        => pair.OriginalEntry is null || pair.OriginalWayOut is null
        ? 0 : ((pair.OriginalWayOut.DateHour > maxHour
            ? maxHour
            : pair.OriginalWayOut.DateHour) - pair.OriginalEntry.DateHour).TotalMinutes;

        protected double DiffMinHourInMinutes(EletronicPointPairs pair, DateTime minHour)
        => pair.OriginalEntry is null || pair.OriginalWayOut is null
        ? 0 : (pair.OriginalWayOut.DateHour - (pair.OriginalEntry.DateHour < minHour
                                                 ? minHour
                                                 : pair.OriginalEntry.DateHour)).TotalMinutes;
        protected List<Appointment> GetNotNullAppointments(IEnumerable<EletronicPointPairs> pairs)
        => (from p in pairs.SelectMany(x => x.GetAppointments())
            where p is not null
            select p).ToList();
            
        protected bool IsNullPair(EletronicPointPairs pair)
        => (pair.OriginalEntry is null || pair.OriginalWayOut is null)
        || (string.IsNullOrEmpty(pair.OriginalEntry?.UniqueAppointmentKey)
        || string.IsNullOrEmpty(pair.OriginalWayOut?.UniqueAppointmentKey));

        protected IEnumerable<RelationAppointmetDate> GetRelationAppointmetDates(List<Appointment> appointments, DateTime[] intervals)
        => from ea in intervals
           select new RelationAppointmetDate(_appointmentsService.GetBestAppointment(ea,
                                                           ref appointments,
                                                           true,
                                                           intervals), ea);
        protected Appointment GetIntervalInAppointment(AppointmentsRecord rappointments,
                                                        List<Appointment> appointments) =>
        _appointmentsService.GetBestAppointment(rappointments.iiappointment,
                                                ref appointments,
                                                true,
                                                rappointments.eappointment,
                                                rappointments.iiappointment,
                                                rappointments.ioappointment,
                                                rappointments.wappointment);

        protected Appointment GetIntervalOutAppointment(AppointmentsRecord rappointments,
                                              List<Appointment> appointments) =>
            _appointmentsService.GetBestAppointment(rappointments.ioappointment,
                                            ref appointments,
                                            true,
                                            rappointments.eappointment,
                                            rappointments.iiappointment,
                                            rappointments.ioappointment,
                                            rappointments.wappointment);

    }
}