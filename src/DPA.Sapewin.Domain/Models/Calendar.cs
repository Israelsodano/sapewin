using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.Domain.Models
{
    public class Calendar
    {
        public static Calendar Build(DateTime date, Schedule schedule)
        => new()
        {
            Date = date,
            Schedule = schedule
        };

        public static Calendar Build(DateTime date,
                          Schedule schedule,
                          Leave leave,
                          Employee employee,
                          IEnumerable<GenericHoliday> genericHolidays,
                          ScheduleScales scheduleScales)
        {
            var result = new Calendar
            {
                Date = date
            };
            if (employee.DaysOff.Any(x => x.Date.Date == date.Date))
            {
                result.Schedule = null;
                result.NonScheduleReference = Ponto.Folga;
            }
            else
            {
                result.Schedule = schedule;

                result.NonScheduleReference = GetNonScheduleReference(leave, employee, genericHolidays, date, scheduleScales);
            }
            return result;
        }
        private static bool ValidateForScheduleReference(Employee employee,
            IEnumerable<GenericHoliday> genericHolidays, DateTime date)
        => ((employee.HolidayId is not null ||
            employee.HolidaysGroup.SpecificHolidays.Any(x => 
                x.Day == date.Day && 
                x.Month == date.Month && 
                (x.Year == 0 || x.Year == date.Year))) ||
            genericHolidays.Any(x => 
                x.Day == date.Day && 
                x.Month == date.Month &&
                (x.Year == 0 || x.Year == date.Year))) &&
            employee.Holiday == EmployeeHoliday.DayOff;

        private static string GetNonScheduleReference(Leave leave, Employee employee,
            IEnumerable<GenericHoliday> genericHolidays, DateTime date, ScheduleScales scheduleScales)
        => leave is null
            ? ValidateForScheduleReference(employee, genericHolidays, date)
                ? Ponto.Feriado
                : scheduleScales.ScheduleId.ToString()
            : leave.Abbreviation;

        public DateTime Date { get; set; }
        public Schedule Schedule { get; set; }
        public string NonScheduleReference { get; set; }
    }

}