using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.Domain.Models
{
    public class Calendar
    {
        public static Calendar Build(DateTime date,
                          Schedule schedule,
                          Leave leave,
                          Employee employee,
                          IEnumerable<GenericHoliday> genericHolidays,
                          ScheduleScales scheduleScales)
        {
            var haveDayOff = employee.DaysOff.Any(x => x.Date.Date == date.Date);
            return new Calendar 
            { 
                Date = date.Date,
                Schedule = haveDayOff ? 
                                null : 
                                schedule,
                NonScheduleReference = haveDayOff ? 
                                            EletronicPoint.DayOff : 
                                            GetNonScheduleReference(leave, 
                                                                    employee, 
                                                                    genericHolidays, 
                                                                    date, 
                                                                    scheduleScales)
            };
        }
        
        private static bool IsHolidayForEmployee(Employee employee,
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
            ? IsHolidayForEmployee(employee, genericHolidays, date)
                ? EletronicPoint.Holiday
                : scheduleScales.ScheduleId.ToString()
            : leave.Abbreviation;

        public DateTime Date { get; set; }
        public Schedule Schedule { get; set; }
        public string NonScheduleReference { get; set; }
    }

}