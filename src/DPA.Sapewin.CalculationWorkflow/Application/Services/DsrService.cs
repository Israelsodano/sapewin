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
        public IEnumerable<EletronicPoint> CalculateDsrs(IEnumerable<IGrouping<Employee, EletronicPoint>> groups)
        {
            return null;
        }

        private EletronicPoint CalculateDsr(IGrouping<Employee, EletronicPoint> eletronicPointsByEmployee)
        {
            DateTime startDate = eletronicPointsByEmployee.FirstOrDefault().Date,
                     endDate = eletronicPointsByEmployee.LastOrDefault().Date;

            SetStartDate(ref startDate, eletronicPointsByEmployee.Key);
            SetEndDate(ref endDate, eletronicPointsByEmployee.Key);

            var weeks = CalculateWeeks(startDate, endDate, eletronicPointsByEmployee).ToArray();

            return null;

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
        CalculateWeeks(DateTime startDate, 
                       DateTime endDate,
                       IEnumerable<EletronicPoint> eletronicPoints)
        {
            var weeks = (int)(endDate - startDate).TotalDays / 6;
            var startWeekDate = startDate;

            for (var i = 0; i < weeks; i++)
            {
                var endWeekDate = startWeekDate.AddDays(7);
                var weekPoints = eletronicPoints.Where(x => x.Date >= startWeekDate && x.Date <= endWeekDate);


                startWeekDate = endWeekDate.AddDays(1);
                yield return CalculateWeek(weekPoints);
            }
        }

        private IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear,
            double absencesAmount, double arrearsAmount)>
        CalculateWeek(IEnumerable<EletronicPoint> weekPoints)
        {
            foreach (var point in weekPoints)
                yield return CalculateDay(point);
        }
        
        private (double valorDsr, bool isDsr, bool isAbsentOrArrear,
            double absencesAmount, double arrearsAmount)
        CalculateDay(EletronicPoint point)
        {
            double[] absences = point.GetAbsences();
            double[] arrears = point.GetArrears();
            return true switch
            {
                true when Saturday(point)
                    => (point.Employee.Parameter.SaturdayDsr, true,
                    SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

                true when DayOff(point)
                    => (point.Employee.Parameter.DayOffDsr, true,
                    SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

                true when Holiday(point)
                    => (point.Employee.Parameter.HolidayDsr, true,
                    SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

                true when Sunday(point)
                    => (point.Employee.Parameter.SundayDsr, true,
                    SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

                _ => throw new ArgumentNullException()
            };
        }
        private static bool Saturday(EletronicPoint point)
        => point.Employee.Parameter.SaturdayDsr != 0
            && point.NonScheduleReference == EletronicPoint.Saturday;
        private static bool DayOff(EletronicPoint point)
        => point.Employee.Parameter.DayOffDsr != 0
            && point.NonScheduleReference == EletronicPoint.DayOff;

        private static bool Holiday(EletronicPoint point)
        => point.Employee.Parameter.HolidayDsr != 0
            && point.NonScheduleReference == EletronicPoint.Holiday;

        private static bool Sunday(EletronicPoint point)
        => point.Employee.Parameter.SundayDsr != 0
            && point.NonScheduleReference == EletronicPoint.Sunday;

        private static bool SomePeriodEqualsZero(double[] absences, double[] arrears)
        => absences.Any(x => x != 0) || arrears.Any(x => x != 0);

        private static DateTime SetStartDate(ref DateTime startDate, in Employee employee)
        =>  startDate = startDate.DayOfWeek == employee.Scale.Turn.WeekTurn.Next()
            ? startDate
            : startDate.AddDays(startDate.DayOfWeek < employee.Scale.Turn.WeekTurn
                ? startDate.DayOfWeek - employee.Scale.Turn.WeekTurn
                : employee.Scale.Turn.WeekTurn - startDate.DayOfWeek);

        private static void SetEndDate(ref DateTime endDate, in Employee employee)
        => endDate = endDate.DayOfWeek == employee.Scale.Turn.WeekTurn
                   ? endDate : endDate.AddDays(Math.Abs(endDate.DayOfWeek - employee.Scale.Turn.WeekTurn));
    }
}