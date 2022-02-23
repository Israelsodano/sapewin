using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Repository;
using System.Linq;
using DPA.Sapewin.Domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Mvc.Filters;
using DPA.Sapewin.CalculationWorkflow.Application.Records;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IAppointmentsService
    {
        Task<IEnumerable<Appointment>> ImportDirtyNotes(IEnumerable<Employee> employees,
                                           DateTime initial,
                                           DateTime final);

        Appointment GetBestAppointment(DateTime refappointment, 
                                               ref List<Appointment> appointments, 
                                               bool verifyBestDateToo, 
                                               params DateTime[] rappointments);

        int GetOnlyMinutesFromDateTime(DateTime? date);


        AppointmentsRecord GetAppointmentsBasedInEletronicPoint(EletronicPoint eletronicPoint);

        IAsyncEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> 
            SnapEmployeesAppointments(IEnumerable<IGrouping<Employee, EletronicPoint>> eletronicPointsByEmployee);
    }

    public class AppointmentsService : IAppointmentsService
    {
        private readonly IUnitOfWork<Appointment> _unitOfWorkAppointment;
        private readonly IUnitOfWork<DirtyNote> _unitOfWorkDirtyNote;
        private readonly IUnitOfWork<EletronicPointPairs> _unitOfWorkEletronicPointPairs;

        public AppointmentsService(IUnitOfWork<Appointment> unitOfWorkAppointment,
                                   IUnitOfWork<EletronicPointPairs> unitOfWorkEletronicPointPairs,
                                   IUnitOfWork<DirtyNote> unitOfWorkDirtyNote)
        {
            _unitOfWorkAppointment = unitOfWorkAppointment ?? throw new ArgumentNullException(nameof(unitOfWorkAppointment));
            _unitOfWorkEletronicPointPairs = unitOfWorkEletronicPointPairs ?? throw new ArgumentNullException(nameof(unitOfWorkEletronicPointPairs));
            _unitOfWorkDirtyNote = unitOfWorkDirtyNote ?? throw new ArgumentNullException(nameof(unitOfWorkDirtyNote));
        }

        public async IAsyncEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> SnapEmployeesAppointments(IEnumerable<IGrouping<Employee, EletronicPoint>> eletronicPointsByEmployee)
        {
            foreach (var eletronicPointsOfEmployee in eletronicPointsByEmployee)
                foreach (var eletronicPoint in eletronicPointsOfEmployee)
                    yield return new Grouping<EletronicPoint, EletronicPointPairs>(eletronicPoint, 
                                                                                    await SnapEmployeeAppointments(eletronicPoint));
        }

        private async Task<IEnumerable<EletronicPointPairs>> SnapEmployeeAppointments(EletronicPoint eletronicPoint)
        {
            var appointments = eletronicPoint.Appointments.ToList();

            var rappointments = GetAppointmentsBasedInEletronicPoint(eletronicPoint);

            Appointment eappointment = GetEntryAppointment(rappointments,
                                                           ref appointments),

                        iiappointment = GetIntervalInAppointment(rappointments,
                                                                 eletronicPoint,
                                                                ref appointments),

                        ioappointment = GetIntervalOutAppointment(rappointments,
                                                                  eletronicPoint,
                                                                  ref appointments),

                        wappointment = GetWayOutAppointment(rappointments,
                                                            ref appointments);

            var elpointpairs = BuildEletronicPointPairs((eappointment,
                                                         iiappointment,
                                                         ioappointment,
                                                         wappointment), eletronicPoint)
                                                         .ToList();

            elpointpairs = ArrangeEletronicPointsPairs(elpointpairs,
                                                       appointments).ToList();

            await _unitOfWorkEletronicPointPairs.Repository.InsertAsync(elpointpairs);
            await _unitOfWorkEletronicPointPairs.SaveChangesAsync();

            return elpointpairs;
        }

        private IEnumerable<EletronicPointPairs> ArrangeEletronicPointsPairs(IList<EletronicPointPairs> eletronicPointPairs,
                                                                             IEnumerable<Appointment> appointments)
        {
            foreach (var appointment in appointments)
            {
                var npair = BuildNewPairBasedInAppointments(eletronicPointPairs, appointment);
                eletronicPointPairs.Add(npair);
            }

            return eletronicPointPairs;
        }

        private EletronicPointPairs BuildNewPairBasedInAppointments(IList<EletronicPointPairs> eletronicPointsPairs,
                                                                    Appointment appointment)
        {
            var bpair = eletronicPointsPairs.FirstOrDefault(x => VerifyIfIsBetweenCurrentAppointments(x, appointment)) ??
                            GetWithSmalllestDifferenceBasedInAppointment(eletronicPointsPairs, appointment);

            return BuildNewPairArrangePairAppointments(ref bpair, appointment);
        }

        private EletronicPointPairs GetWithSmalllestDifferenceBasedInAppointment(IList<EletronicPointPairs> eletronicPointPairs,
                                                                                 Appointment appointment) =>
            (from d in (from e in eletronicPointPairs
                        select new
                        {
                            Target = e,
                            Difference = Math.Abs(((e.OriginalEntry ?? e.OriginalWayOut).DateHour - appointment.DateHour).Ticks)
                        })
             orderby d.Difference
             select d.Target).FirstOrDefault();

        private EletronicPointPairs BuildNewPairArrangePairAppointments(ref EletronicPointPairs eletronicPointPairs,
                                                                        Appointment appointment)
        {
            var appointments = (from a in new[] { appointment,
                                                    eletronicPointPairs.OriginalEntry,
                                                    eletronicPointPairs.OriginalWayOut }
                                orderby a?.DateHour
                                select a).ToArray();

            eletronicPointPairs.OriginalEntry = appointments[0];
            eletronicPointPairs.EntryAppointmentId = appointments[0].Id;

            eletronicPointPairs.OriginalWayOut = appointments[1];
            eletronicPointPairs.WayOutAppointmentId = appointments[1].Id;

            return appointments[2] is null ? null : EnlaceAppointments(null,
                                                                       appointments[2],
                                                                       eletronicPointPairs.EletronicPoint);
        }


        private bool VerifyIfIsBetweenCurrentAppointments(EletronicPointPairs eletronicPointPairs, Appointment appointment) =>
            (eletronicPointPairs.OriginalEntry is null ||
            eletronicPointPairs.OriginalWayOut is null) ||
            eletronicPointPairs.OriginalEntry.DateHour <= appointment.DateHour &&
            eletronicPointPairs.OriginalWayOut.DateHour >= appointment.DateHour;

        private IEnumerable<EletronicPointPairs> BuildEletronicPointPairs((Appointment eappointment,
                                                                           Appointment iiappointment,
                                                                           Appointment ioappointment,
                                                                           Appointment wappointment) rappointments,
                                                                           EletronicPoint eletronicPoint) =>
            rappointments.iiappointment is not null ||
            rappointments.ioappointment is not null ?
                new[] { EnlaceAppointments(rappointments.eappointment,
                                            rappointments.iiappointment,
                                            eletronicPoint),
                         EnlaceAppointments(rappointments.ioappointment,
                                            rappointments.wappointment,
                                            eletronicPoint) } :

                new[] { EnlaceAppointments(rappointments.eappointment,
                                            rappointments.wappointment,
                                            eletronicPoint) };

        private EletronicPointPairs EnlaceAppointments(Appointment a1,
                                                       Appointment a2,
                                                       EletronicPoint eletronicPoint)
        {
            var eletronicPointPairs = new EletronicPointPairs
            {
                EmployeeId = eletronicPoint.EmployeeId,
                CompanyId = eletronicPoint.CompanyId,
                EletronicPointId = eletronicPoint.Id,
                EletronicPoint = eletronicPoint,
            };

            var appointments = (from a in new[] { a1, a2 }
                                orderby a?.DateHour
                                select a).ToArray();

            if (appointments.Any(x => x is null))
                return BuildNullPair(ref eletronicPointPairs,
                                         eletronicPoint,
                                         appointments);

            eletronicPointPairs.OriginalEntry = appointments[0];
            eletronicPointPairs.EntryAppointmentId = appointments[0].Id;

            eletronicPointPairs.OriginalWayOut = appointments[1];
            eletronicPointPairs.WayOutAppointmentId = appointments[1].Id;

            return eletronicPointPairs;
        }

        private EletronicPointPairs BuildNullPair(ref EletronicPointPairs eletronicPointPairs,
                                                  EletronicPoint eletronicPoint,
                                                  Appointment[] appointments)
        {
            var nnappointment = appointments.FirstOrDefault(x => x is not null);
            var mappointment = GetOnlyMinutesFromDateTime(nnappointment.DateHour);

            if (Math.Abs(mappointment - eletronicPoint.Schedule.Period.Entry) <
                Math.Abs(mappointment - eletronicPoint.Schedule.Period.WayOut))
            {
                eletronicPointPairs.OriginalEntry = nnappointment;
                eletronicPointPairs.EntryAppointmentId = nnappointment.Id;
            }
            else
            {
                eletronicPointPairs.OriginalWayOut = nnappointment;
                eletronicPointPairs.WayOutAppointmentId = nnappointment.Id;
            }

            return eletronicPointPairs;
        }

        public int GetOnlyMinutesFromDateTime(DateTime? date) => !date.HasValue ? 0 : (date.Value.Hour * 60) + date.Value.Minute;

        private Appointment GetWayOutAppointment(AppointmentsRecord rappointments,
                                                 ref List<Appointment> appointments) =>

            GetBestAppointment(rappointments.wappointment,
                                ref appointments,
                                false,
                                rappointments.eappointment,
                                rappointments.iiappointment,
                                rappointments.ioappointment,
                                rappointments.wappointment);

        private Appointment GetIntervalInAppointment(AppointmentsRecord rappointments,
                                                    EletronicPoint eletronicPoint,
                                                    ref List<Appointment> appointments)
        {
            if (!eletronicPoint.Schedule.Period.IntervalIn.HasValue) return null;

            var iiappointment = GetBestAppointment(rappointments.iiappointment,
                                                  ref appointments,
                                                  true,
                                                  rappointments.eappointment,
                                                    rappointments.iiappointment,
                                                    rappointments.ioappointment,
                                                    rappointments.wappointment);

            if (iiappointment is null && eletronicPoint.Employee.Interval == EmployeeInterval.PreAssigned)
                iiappointment = new Appointment
                {
                    CompanyId = eletronicPoint.Employee.CompanyId,
                    EmployeeId = eletronicPoint.EmployeeId,
                    DateHour = eletronicPoint.Date
                            .AddMinutes(eletronicPoint.Schedule.Period.IntervalIn.Value)
                            .AddDays(eletronicPoint.Schedule.Period.IntervalIn.Value < eletronicPoint.Schedule.Period.Entry ? 1 : 0)
                };

            return iiappointment;
        }

        private Appointment GetIntervalOutAppointment(AppointmentsRecord rappointments,
                                                    EletronicPoint eletronicPoint,
                                                    ref List<Appointment> appointments)
        {
            if (!eletronicPoint.Schedule.Period.IntervalIn.HasValue) return null;

            var ioappointment = GetBestAppointment(rappointments.ioappointment,
                                                  ref appointments,
                                                  true,
                                                  rappointments.eappointment,
                                                    rappointments.iiappointment,
                                                    rappointments.ioappointment,
                                                    rappointments.wappointment);

            if (ioappointment is null && eletronicPoint.Employee.Interval == EmployeeInterval.PreAssigned)
                ioappointment = new Appointment
                {
                    CompanyId = eletronicPoint.Employee.CompanyId,
                    EmployeeId = eletronicPoint.EmployeeId,
                    DateHour = eletronicPoint.Date
                            .AddMinutes(eletronicPoint.Schedule.Period.IntervalOut.Value)
                            .AddDays(eletronicPoint.Schedule.Period.IntervalOut.Value < eletronicPoint.Schedule.Period.Entry ? 1 : 0)
                };

            return ioappointment;
        }

        private Appointment GetEntryAppointment(AppointmentsRecord rappointments,
                                                ref List<Appointment> appointments) =>

            GetBestAppointment(rappointments.eappointment,
                                ref appointments,
                                false,
                                rappointments.eappointment,
                                rappointments.iiappointment,
                                rappointments.ioappointment,
                                rappointments.wappointment);
                                        
        public Appointment GetBestAppointment(DateTime refappointment, 
                                               ref List<Appointment> appointments, 
                                               bool verifyBestDateToo, 
                                               params DateTime[] rappointments)
        {
            var bappointment = GetBestAppointmentByDateTime(refappointment, appointments);
            var bdate = GetBestDateByAppointment(bappointment, 
                                                 rappointments);

            var r = !verifyBestDateToo ?
                        bappointment :

                            bdate == refappointment ?
                                bappointment : null;

            appointments.RemoveAll(x => x.Id == r?.Id);

            return r;
        }

        private DateTime GetBestDateByAppointment(Appointment appointment, params DateTime[] dates) =>
           (from d in (from dt in dates
                       select new
                       {
                           Target = dt,
                           Difference = Math.Abs((dt - appointment.DateHour).Ticks)
                       })
            orderby d.Difference
            select d.Target).FirstOrDefault();

        private Appointment GetBestAppointmentByDateTime(DateTime refappointment, IEnumerable<Appointment> appointments) =>
            (from d in (from a in appointments
                        select new
                        {
                            Target = a,
                            Difference = Math.Abs((a.DateHour - refappointment.Date).Ticks)
                        })
             orderby d.Difference
             select d.Target).FirstOrDefault();


        public AppointmentsRecord GetAppointmentsBasedInEletronicPoint(EletronicPoint eletronicPoint)
        {
            DateTime eappointment = eletronicPoint.Date.AddMinutes(eletronicPoint.Schedule.Period.Entry),
                     iiappointment = eletronicPoint.Date.AddMinutes(eletronicPoint.Schedule.Period.IntervalIn ?? 0)
                                                        .AddDays(eletronicPoint.Schedule.Period.IntervalIn.HasValue ? 
                                                                    GetDayCompensation(eletronicPoint.Schedule.Period.Entry, 
                                                                                       eletronicPoint.Schedule.Period.IntervalIn.Value) : 
                                                                    0),

                     ioappointment = eletronicPoint.Date.AddMinutes(eletronicPoint.Schedule.Period.IntervalOut ?? 0)
                                                        .AddDays(eletronicPoint.Schedule.Period.IntervalIn.HasValue ? 
                                                                    GetDayCompensation(eletronicPoint.Schedule.Period.IntervalIn.Value, 
                                                                                       eletronicPoint.Schedule.Period.IntervalOut.Value) : 
                                                                    0),
                     wappointment = eletronicPoint.Date.AddMinutes(eletronicPoint.Schedule.Period.WayOut)
                                                        .AddDays(eletronicPoint.Schedule.Period.IntervalIn.HasValue ? 
                                                                    GetDayCompensation(eletronicPoint.Schedule.Period.IntervalOut.Value, 
                                                                                       eletronicPoint.Schedule.Period.WayOut) : 
                                                                    0);

            return new(eappointment, 
                        iiappointment, 
                        ioappointment, 
                        wappointment);
        }

        public int GetDayCompensation(int entry, int wayOut) => wayOut <= entry ? 1 : 0;

        public async Task<IEnumerable<Appointment>> ImportDirtyNotes(IEnumerable<Employee> employees, 
                                           DateTime initial, 
                                           DateTime final)
        {
            var dirtyNotes = _unitOfWorkDirtyNote.Repository
                                .GetAll(x =>
                                    employees.Any(z => z.Pis == x.Pis) &&
                                    x.Date >= initial && x.Date <= final).ToArray();

            var appointments = _unitOfWorkAppointment.Repository
                                    .GetAll(x =>
                                        employees.Any(y => y.Id == x.EmployeeId) &&
                                        x.DateHour >= initial && x.DateHour <= final).ToList();

            dirtyNotes = (from d in dirtyNotes
                          where !appointments.Any(x => x.UniqueAppointmentKey == d.UniqueAppointmentKey)
                          select d).ToArray();

            if (dirtyNotes.Any())
            {
                var ap = (from d in dirtyNotes select (Appointment)d)
                        .ToArray();

                await _unitOfWorkAppointment.Repository
                        .InsertAsync(ap);

                await _unitOfWorkAppointment.SaveChangesAsync();
                appointments.AddRange(ap);
            }

            return appointments;
        }
        
        public async IAsyncEnumerable<IGrouping<Employee, EletronicPointPairs>> SnapEmployeesAppointmentsLoad(IEnumerable<IGrouping<Employee, EletronicPoint>> eletronicPointsByEmployee)
        {
            foreach (var employee in eletronicPointsByEmployee)
                foreach (var point in employee)
                {
                    var pairs = SnapEmployeeAppointmentsLoad(point);

                    await _unitOfWorkEletronicPointPairs.Repository.InsertAsync(pairs);
                    await _unitOfWorkEletronicPointPairs.SaveChangesAsync();

                    yield return new Grouping<Employee, EletronicPointPairs>(employee.Key, pairs);
                }
        }
        private IEnumerable<EletronicPointPairs> SnapEmployeeAppointmentsLoad(EletronicPoint point)
        {
            var appointments = point.Appointments.OrderBy(x => x.DateHour).ToList();
            for (var i = 0; i < appointments.Count; i += 2)
                yield return EnlaceEletronicPointPairsLoad(appointments[i], i == appointments.Count - 1 ? 
                                                                    null : 
                                                                    appointments[i + 1], 
                                                            point);
        }

        private EletronicPointPairs EnlaceEletronicPointPairsLoad(Appointment a1, Appointment a2, EletronicPoint ePoint)
        => HandleAppointments(new EletronicPointPairs {
                EmployeeId = ePoint.EmployeeId,
                CompanyId = ePoint.CompanyId,
                EletronicPointId = ePoint.Id,
                EletronicPoint = ePoint,
            }, a1, a2);
            
        private void HandleBothAppointmentsNotNull(ref EletronicPointPairs result, Appointment[] appointments)
        {
            result.OriginalEntry = appointments[0];
            result.EntryAppointmentId = appointments[0].Id;
            result.OriginalWayOut = appointments[1];
            result.WayOutAppointmentId = appointments[1].Id;
        }

        private void HandleNullAppointment(ref EletronicPointPairs result, Appointment[] appointments)
        {
            var owayout = appointments.FirstOrDefault(x => x is not null);
            result.OriginalWayOut = owayout;
            result.WayOutAppointmentId = owayout.Id;
        }

        private EletronicPointPairs HandleAppointments(EletronicPointPairs result, Appointment a1, Appointment a2)
        {
            var appointments =  new Appointment[] { a1, a2 }.OrderBy(x => x?.DateHour).ToArray();
            if (a1 is not null || a2 is not null)
            {
                if (a1 is not null && a2 is not null) HandleBothAppointmentsNotNull(ref result, appointments);
                else HandleNullAppointment(ref result, appointments);
            }
            return result;
        }
    }
}