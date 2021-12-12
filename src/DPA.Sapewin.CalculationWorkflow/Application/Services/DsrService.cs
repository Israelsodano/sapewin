using System;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IDsrService
    {

    }
    public class DsrService : IDsrService
    {
        public IEnumerable<EletronicPoint> CalculateDsrs(IEnumerable<IGrouping<Employee, EletronicPoint>> groups,
            IEnumerable<EmployeeCalendars> calendars, DateTime startDate, DateTime endDate)
        {

        }
        private EletronicPoint CalculateDsr(Employee employee, IEnumerable<EletronicPoint> eletronicPoints,
            in IEnumerable<EmployeeCalendars> calendars, DateTime startDate, DateTime endDate)
        {

            var calendar = calendars.FirstOrDefault(x => x.Employee.Equals(employee.Scale));
            SetStartDate(ref startDate, employee);
            SetEndDate(ref endDate, employee);

            var weeks = CalculateWeeks(startDate, endDate, eletronicPoints, calendar).ToArray();

        }
        private IEnumerable<IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear, double absencesAmount, double arrearsAmount)>> HandleWeeks(
            IEnumerable<IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear, double absencesAmount, double arrearsAmount)>> weeks)
        {
            foreach (var week in weeks)
                yield return HandleWeek(week, out double totalAbsencesInMinutes,
                                            out double totalArrearsInMinutes,
                                            out double absentDaysAmount,
                                            out double arrearsDaysAmount);
        }

        private IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear, double absencesAmount, double arrearsAmount)> HandleWeek(
            IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear, double absencesAmount, double arrearsAmount)> week,
            out double totalAbsencesInMinutes, out double totalArrearsInMinutes, out double absentDaysAmount, out double arrearsDaysAmount)
        {
            totalAbsencesInMinutes = totalArrearsInMinutes = absentDaysAmount = arrearsDaysAmount = 0;
            foreach (var day in week)
            {
                if (!day.isDsr)
                    if (day.isAbsentOrArrear)
                    {
                        totalArrearsInMinutes += day.arrearsAmount;
                        totalAbsencesInMinutes += day.absencesAmount;

                        if (day.absencesAmount > 0)
                            absentDaysAmount++;

                        if (day.arrearsAmount > 0)
                            arrearsDaysAmount++;
                    }

                if (day.isDsr && !day.)
            }
        }
        private IEnumerable<IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear,
            double absencesAmount, double arrearsAmount)>>
        CalculateWeeks(DateTime startDate, DateTime endDate,
            IEnumerable<EletronicPoint> eletronicPoints, EmployeeCalendars calendar)
        {
            var weeks = (int)(endDate - startDate).TotalDays / 6;

            var startWeekDate = startDate;
            for (var i = 0; i < weeks; i++)
            {
                var endWeekDate = startWeekDate.AddDays(7);
                var weekPoints = eletronicPoints.Where(x => x.Date >= startWeekDate && x.Date <= endWeekDate);


                startWeekDate = endWeekDate.AddDays(1);
                yield return CalculateWeek(weekPoints, calendar);
            }
        }

        private IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear,
            double absencesAmount, double arrearsAmount)>
        CalculateWeek(IEnumerable<EletronicPoint> weekPoints, EmployeeCalendars employeeCalendar)
        {
            foreach (var point in weekPoints)
                yield return CalculateDay(point, employeeCalendar);
        }
        private (double valorDsr, bool isDsr, bool isAbsentOrArrear,
            double absencesAmount, double arrearsAmount)
        CalculateDay(EletronicPoint point, EmployeeCalendars employeeCalendars)
        {
            double[] absences = point.GetAbsences();
            double[] arrears = point.GetArrears();
            return true switch
            {
                true when Saturday(employeeCalendars, point)
                    => (employeeCalendars.Employee.Parameter.SaturdayDsr, true,
                    SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

                true when DayOff(employeeCalendars, point)
                    => (employeeCalendars.Employee.Parameter.DayOffDsr, true,
                    SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

                true when Holiday(employeeCalendars, point)
                    => (employeeCalendars.Employee.Parameter.HolidayDsr, true,
                    SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

                true when Sunday(employeeCalendars, point)
                    => (employeeCalendars.Employee.Parameter.SundayDsr, true,
                    SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

                _ => throw new ArgumentNullException()
            };
        }
        private static bool Saturday(EmployeeCalendars employeeCalendars, EletronicPoint point)
        => employeeCalendars.Employee.Parameter.SaturdayDsr != 0
            && employeeCalendars.Calendars.FirstOrDefault(x => x.Date == point.Date)
            .NonScheduleReference == EletronicPoint.Saturday;
        private static bool DayOff(EmployeeCalendars employeeCalendars, EletronicPoint point)
        => employeeCalendars.Employee.Parameter.DayOffDsr != 0
            && employeeCalendars.Calendars.FirstOrDefault(x => x.Date == point.Date)
            .NonScheduleReference == EletronicPoint.DayOff;

        private static bool Holiday(EmployeeCalendars employeeCalendars, EletronicPoint point)
        => employeeCalendars.Employee.Parameter.HolidayDsr != 0
            && employeeCalendars.Calendars.FirstOrDefault(x => x.Date == point.Date)
            .NonScheduleReference == EletronicPoint.Holiday;

        private static bool Sunday(EmployeeCalendars employeeCalendars, EletronicPoint point)
        => employeeCalendars.Employee.Parameter.SundayDsr != 0
            && employeeCalendars.Calendars.FirstOrDefault(x => x.Date == point.Date)
            .NonScheduleReference == EletronicPoint.Sunday;

        private static bool SomePeriodEqualsZero(double[] absences, double[] arrears)
        => absences.Any(x => x != 0) || arrears.Any(x => x != 0);
        private static void SetStartDate(ref DateTime startDate, in Employee employee)
        => startDate = startDate.DayOfWeek == employee.Scale.Turn.WeekTurn.Next()
            ? startDate
            : startDate.AddDays(startDate.DayOfWeek < employee.Scale.Turn.WeekTurn
                ? startDate.DayOfWeek - employee.Scale.Turn.WeekTurn
                : employee.Scale.Turn.WeekTurn - startDate.DayOfWeek);

        private static void SetEndDate(ref DateTime endDate, in Employee employee)
        => endDate = endDate.DayOfWeek == employee.Scale.Turn.WeekTurn
                   ? endDate : endDate.AddDays(Math.Abs(endDate.DayOfWeek - employee.Scale.Turn.WeekTurn));
    }
}