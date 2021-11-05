using System;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models;
using DPA.Sapewin.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IEmployeeCalendarService
    {
        IEnumerable<EmployeeCalendars> BuildEmployeeCalendars(IEnumerable<Employee> employees, DateTime startDate,
            DateTime endDate, IEnumerable<GenericHoliday> genericHolidays);
    }

    internal class EmployeeCalendarService : IEmployeeCalendarService
    {
        private readonly IUnitOfWork<Schedule> _scheduleUnit;
        private readonly IUnitOfWork<Leave> _leavesUnit;
        private readonly ILogger _logger;
        EmployeeCalendarService(IUnitOfWork<Schedule> scheduleUnit, IUnitOfWork<Leave> leavesUnit,
            ILogger<EmployeeCalendarService> logger)
        {
            _leavesUnit = leavesUnit ?? throw new ArgumentNullException(nameof(leavesUnit));
            _scheduleUnit = scheduleUnit ?? throw new ArgumentNullException(nameof(scheduleUnit));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IEnumerable<EmployeeCalendars> BuildEmployeeCalendars(IEnumerable<Employee> employees, DateTime startDate,
            DateTime endDate, IEnumerable<GenericHoliday> genericHolidays)
        {
            startDate = startDate.AddDays(-10);
            endDate = endDate.AddDays(10);

            foreach (var employee in employees)
            {
                var schedules = GetSchedulesFromRepository(employee);
                var employeeLeaves = GetEmployeeLeavesFromRepository(employee.Id);
                var employeeCalendars = EmployeeCalendars.Build(employee);

                while (startDate <= endDate)
                {

                    var schedule = GetOccasionalSchedule(employee, startDate);
                    var days = (startDate - employee.Scale.StartDate).Days;
                    var dayScale = GetScheduleScales(days, employee.Scale.ScheduleScales)
                        .ToArray()[GetDayScaleIndex(days, employee)];

                    var leave = GetLeave(employeeLeaves, startDate);
                    HandleSchedule(leave, schedules, dayScale, ref schedule);

                    employeeCalendars.Calendars = GetCalendars(ref startDate,
                        GetDaysAmount(employee, startDate, dayScale),
                            schedule, leave, employee, genericHolidays, dayScale);

                    schedule = null;
                }
                yield return employeeCalendars;
            }
        }
        private static IEnumerable<ScheduleScales> GetScheduleScales(int days, IEnumerable<ScheduleScales> scheduleScales)
            => days > 0 ? scheduleScales.OrderBy(x => x.Order)
                : scheduleScales.OrderByDescending(x => x.Order);
        private static int GetDaysAmount(Employee employee, DateTime startDate, ScheduleScales dayScale)
            => employee.OccasionalSchedules.Any(x => x.Date.Date == startDate.Date)
                ? 1 : dayScale.DaysAmount;
        private static Leave GetLeave(IEnumerable<Leave> employeeLeaves, DateTime startDate)
            => employeeLeaves.FirstOrDefault(x => x.StartDate <= startDate && startDate <= x.EndDate);
        private static int GetDayScaleIndex(int days, Employee employee)
            => Math.Abs(days) % employee.Scale.ScheduleScales.Count();
        private Schedule GetOccasionalSchedule(Employee employee, DateTime startDate)
        {
            _logger.LogInformation($"Getting calendar for startDate: { startDate } - employee ID: {employee.Id}");
            return employee.OccasionalSchedules.FirstOrDefault(x => x.Date.Date == startDate.Date)?.Schedule;
        }
        private static IEnumerable<Calendar> GetCalendars(ref DateTime startDate, int daysAmount, Schedule schedule,
            Leave leave, Employee employee, IEnumerable<GenericHoliday> genericHolidays, ScheduleScales dayScale)
        {
            var result = new List<Calendar>();
            for (int i = 0; i < daysAmount; i++)
            {
                result.Add(Calendar
                    .Build(startDate, schedule, leave, employee, genericHolidays, dayScale));
                startDate = startDate.AddDays(1);
            }
            return result;
        }
        private IEnumerable<Schedule> GetSchedulesFromRepository(Employee employee)
        {
            _logger.LogInformation($"Getting schedules for employee ID: {employee.Id}");
            return _scheduleUnit.Repository
                    .GetAll(x =>
                        employee.Scale.ScheduleScales
                        .Any(y => y.ScheduleId == x.Id && y.CompanyId == x.CompanyId))
                    .Include(x => x.AuxiliaryIntervals).ToArray();
        }
        private IEnumerable<Leave> GetEmployeeLeavesFromRepository(Guid employeeId)
        {
            _logger.LogInformation($"Getting employeeLeaves for employee ID: {employeeId}");
            return _leavesUnit.Repository.GetAll(x => x.EmployeeId == employeeId).ToArray();
        }
        private static void HandleSchedule(Leave leave, IEnumerable<Schedule> schedules, ScheduleScales dayScale, ref Schedule schedule)
            => schedule = leave is not null
                ? null : schedule is not null
                    ? schedule : schedules.FirstOrDefault(x =>
                        x.Id == dayScale.ScheduleId &&
                        x.CompanyId == dayScale.CompanyId);
    }
}