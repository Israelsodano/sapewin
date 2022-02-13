<<<<<<< HEAD
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using DPA.Sapewin.Domain.Entities;
// using DPA.Sapewin.Domain.Models;

// namespace DPA.Sapewin.CalculationWorkflow.Application.Services
// {
//     public interface IDsrService
//     {

//     }
//     public class DsrService : IDsrService
//     {
//         public IEnumerable<EletronicPoint> CalculateDsrs(IEnumerable<IGrouping<Employee, EletronicPoint>> groups,
//             IEnumerable<EmployeeCalendars> calendars, DateTime startDate, DateTime endDate)
//         {

//         }
//         private EletronicPoint CalculateDsr(Employee employee, IEnumerable<EletronicPoint> eletronicPoints,
//             in IEnumerable<EmployeeCalendars> calendars, DateTime startDate, DateTime endDate)
//         {

//             var calendar = calendars.FirstOrDefault(x => x.Employee.Equals(employee.Scale));
//             SetStartDate(ref startDate, employee);
//             SetEndDate(ref endDate, employee);

//             var weeks = CalculateWeeks(startDate, endDate, eletronicPoints, calendar).ToArray();

//         }
//         private IEnumerable<IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear, double absencesAmount, double arrearsAmount)>> HandleWeeks(
//             IEnumerable<IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear, double absencesAmount, double arrearsAmount)>> weeks)
//         {
//             foreach (var week in weeks)
//                 yield return HandleWeek(week, out double totalAbsencesInMinutes,
//                                             out double totalArrearsInMinutes,
//                                             out double absentDaysAmount,
//                                             out double arrearsDaysAmount);
//         }

//         private IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear, double absencesAmount, double arrearsAmount)> HandleWeek(
//             IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear, double absencesAmount, double arrearsAmount)> week,
//             out double totalAbsencesInMinutes, out double totalArrearsInMinutes, out double absentDaysAmount, out double arrearsDaysAmount)
//         {
//             totalAbsencesInMinutes = totalArrearsInMinutes = absentDaysAmount = arrearsDaysAmount = 0;
//             foreach (var day in week)
//             {
//                 if (!day.isDsr)
//                     if (day.isAbsentOrArrear)
//                     {
//                         totalArrearsInMinutes += day.arrearsAmount;
//                         totalAbsencesInMinutes += day.absencesAmount;

//                         if(day.absencesAmount > 0)
//                             absentDaysAmount++;
                        
//                         if(day.arrearsAmount > 0)
//                             arrearsDaysAmount++;
//                     }

//                 if(day.isDsr && !day.)
//             }
//         }
//         private IEnumerable<IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear,
//             double absencesAmount, double arrearsAmount)>>
//         CalculateWeeks(DateTime startDate, DateTime endDate,
//             IEnumerable<EletronicPoint> eletronicPoints, EmployeeCalendars calendar)
//         {
//             var weeks = (int)(endDate - startDate).TotalDays / 6;

//             var startWeekDate = startDate;
//             for (var i = 0; i < weeks; i++)
//             {
//                 var endWeekDate = startWeekDate.AddDays(7);
//                 var weekPoints = eletronicPoints.Where(x => x.Date >= startWeekDate && x.Date <= endWeekDate);


//                 startWeekDate = endWeekDate.AddDays(1);
//                 yield return CalculateWeek(weekPoints, calendar);
//             }
//         }

//         private IEnumerable<(double valorDsr, bool isDsr, bool isAbsentOrArrear,
//             double absencesAmount, double arrearsAmount)>
//         CalculateWeek(IEnumerable<EletronicPoint> weekPoints, EmployeeCalendars employeeCalendar)
//         {
//             foreach (var point in weekPoints)
//                 yield return CalculateDay(point, employeeCalendar);
//         }
//         private (double valorDsr, bool isDsr, bool isAbsentOrArrear,
//             double absencesAmount, double arrearsAmount)
//         CalculateDay(EletronicPoint point, EmployeeCalendars employeeCalendars)
//         {
//             double[] absences = point.GetAbsences();
//             double[] arrears = point.GetArrears();
//             return true switch
//             {
//                 true when Saturday(employeeCalendars, point)
//                     => (employeeCalendars.Employee.Parameter.SaturdayDsr, true,
//                     SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

//                 true when DayOff(employeeCalendars, point)
//                     => (employeeCalendars.Employee.Parameter.DayOffDsr, true,
//                     SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

//                 true when Holiday(employeeCalendars, point)
//                     => (employeeCalendars.Employee.Parameter.HolidayDsr, true,
//                     SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

//                 true when Sunday(employeeCalendars, point)
//                     => (employeeCalendars.Employee.Parameter.SundayDsr, true,
//                     SomePeriodEqualsZero(absences, arrears), absences.Sum(), arrears.Sum()),

//                 _ => throw new ArgumentNullException()
//             };
//         }
//         private static bool Saturday(EmployeeCalendars employeeCalendars, EletronicPoint point)
//         => employeeCalendars.Employee.Parameter.SaturdayDsr != 0
//             && employeeCalendars.Calendars.FirstOrDefault(x => x.Date == point.Date)
//             .NonScheduleReference == EletronicPoint.Saturday;
//         private static bool DayOff(EmployeeCalendars employeeCalendars, EletronicPoint point)
//         => employeeCalendars.Employee.Parameter.DayOffDsr != 0
//             && employeeCalendars.Calendars.FirstOrDefault(x => x.Date == point.Date)
//             .NonScheduleReference == EletronicPoint.DayOff;

//         private static bool Holiday(EmployeeCalendars employeeCalendars, EletronicPoint point)
//         => employeeCalendars.Employee.Parameter.HolidayDsr != 0
//             && employeeCalendars.Calendars.FirstOrDefault(x => x.Date == point.Date)
//             .NonScheduleReference == EletronicPoint.Holiday;

//         private static bool Sunday(EmployeeCalendars employeeCalendars, EletronicPoint point)
//         => employeeCalendars.Employee.Parameter.SundayDsr != 0
//             && employeeCalendars.Calendars.FirstOrDefault(x => x.Date == point.Date)
//             .NonScheduleReference == EletronicPoint.Sunday;

//         private static bool SomePeriodEqualsZero(double[] absences, double[] arrears)
//         => absences.Any(x => x != 0) || arrears.Any(x => x != 0);
//         private static void SetStartDate(ref DateTime startDate, in Employee employee)
//         => startDate = startDate.DayOfWeek == employee.Scale.Turn.WeekTurn.Next()
//             ? startDate
//             : startDate.AddDays(startDate.DayOfWeek < employee.Scale.Turn.WeekTurn
//                 ? startDate.DayOfWeek - employee.Scale.Turn.WeekTurn
//                 : employee.Scale.Turn.WeekTurn - startDate.DayOfWeek);

//         private static void SetEndDate(ref DateTime endDate, in Employee employee)
//         => endDate = endDate.DayOfWeek == employee.Scale.Turn.WeekTurn
//                    ? endDate : endDate.AddDays(Math.Abs(endDate.DayOfWeek - employee.Scale.Turn.WeekTurn));
//     }
// }
=======
using System;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.CalculationWorkflow.Application.Records;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IDsrService
    {
        IAsyncEnumerable<IGrouping<Employee, EletronicPoint>> Calculate(IEnumerable<IGrouping<Employee, EletronicPoint>> eletronicPointsByEmployees);
    }

    public class DsrService : IDsrService
    {
        private readonly IUnitOfWork<EletronicPoint> _unitOfWorkEletronicPoint;

        public DsrService(IUnitOfWork<EletronicPoint> unitOfWorkEletronicPoint)
        {  
           _unitOfWorkEletronicPoint = unitOfWorkEletronicPoint ?? throw new ArgumentNullException(nameof(unitOfWorkEletronicPoint)); 
        }

        public async IAsyncEnumerable<IGrouping<Employee, EletronicPoint>> Calculate(IEnumerable<IGrouping<Employee, EletronicPoint>> eletronicPointsByEmployees)
        {
            foreach (var eletronicPointsByEmployee in eletronicPointsByEmployees)
            {
                var dsrByEmployee = CalculateDsrByEmployee(eletronicPointsByEmployee);
                _unitOfWorkEletronicPoint.Repository.Update(dsrByEmployee);
                await _unitOfWorkEletronicPoint.SaveChangesAsync();

                yield return new Grouping<Employee, EletronicPoint>(eletronicPointsByEmployee.Key, 
                                                                    dsrByEmployee);
            }
        }

        private IEnumerable<EletronicPoint> CalculateDsrByEmployee(in IGrouping<Employee, EletronicPoint> eletronicPointsByEmployee)
        {
            var eletronicPointsOrdened = eletronicPointsByEmployee.OrderBy(x=> x.Date);
            
            DateTime startDate = eletronicPointsOrdened.FirstOrDefault().Date,
                     endDate = eletronicPointsOrdened.LastOrDefault().Date;

            SetStartDate(ref startDate, eletronicPointsByEmployee.Key);
            SetEndDate(ref endDate, eletronicPointsByEmployee.Key);

            var weeks = CalculateWeeks(startDate, endDate, eletronicPointsByEmployee);
            weeks = CalculateDiscountedAndPaidDsrForWeeks(weeks, eletronicPointsByEmployee.Key);

            return weeks.SelectMany(x=> x.Select(y=> y.eletronicPoint));
        }

        private IEnumerable<IEnumerable<CalculatedDay>> CalculateDiscountedAndPaidDsrForWeeks(IEnumerable<IEnumerable<CalculatedDay>> calculatedWeeks,
                                                                                              Employee employee)
        {
            foreach (var week in calculatedWeeks)
                yield return CalculateDiscountedAndPaidDsrForWeek(week, employee);
        }


        private IEnumerable<CalculatedDay> CalculateDiscountedAndPaidDsrForWeek(in IEnumerable<CalculatedDay> week, 
                                                                                in Employee employee)
        {
         
            var occurrence = Occurrence.Empty;

            foreach (var day in week)
            {
                occurrence += BuildOccurence(day);
                var discountedDsr = CalculateDiscountedDsr(day, 
                                                           week, 
                                                           occurrence);

                ApplyPaidAndDiscountedDsrInEletronicPoint(day);
            }

            ApplyDiscountToLastDsrBeforeAbsence(week, 
                                                employee);

            return week;
        }

        private void ApplyDiscountToLastDsrBeforeAbsence(in IEnumerable<CalculatedDay> week, 
                                                         in Employee employee)
        {
            if(!(employee.Parameter.DiscountDsrBeforeAbsence is true))
                return;
            
            var lastdsr = week.OrderBy(x=> x.eletronicPoint.Date).LastOrDefault(x=> x.dsr);  
            ChangeAllValuesOfDsrInWeek(week, lastdsr);
        }

        private void ChangeAllValuesOfDsrInWeek(in IEnumerable<CalculatedDay> week, 
                                                in CalculatedDay lastDsr)
        {
            foreach (var day in week)
            {
                day.eletronicPoint.PaidDsr = lastDsr.eletronicPoint.PaidDsr;
                day.eletronicPoint.DiscountedDsr = lastDsr.eletronicPoint.DiscountedDsr;
            }
        }

        private void ApplyPaidAndDiscountedDsrInEletronicPoint(in CalculatedDay calculatedDay)
        {
            if(!calculatedDay.dsr)
                return;

            calculatedDay.eletronicPoint.DiscountedDsr = Math.Min(calculatedDay.dsrAmount, 
                                                                  calculatedDay.eletronicPoint.DiscountedDsr);

            calculatedDay.eletronicPoint.PaidDsr = calculatedDay.dsrAmount - 
                                                   calculatedDay.eletronicPoint.DiscountedDsr;
            
        }

        private double CalculateDiscountedDsr(in CalculatedDay calculatedDay,
                                              in IEnumerable<CalculatedDay> week, 
                                              in Occurrence occurrence)
        {
            var exceededLimitArrears = calculatedDay.eletronicPoint.Employee.Parameter.WeeklyOccurrenceDsr <= occurrence;
            var exceededLimitAbsencesAndArrears = exceededLimitArrears || occurrence.absenceMinutes > 0;
            if(!calculatedDay.dsr || (calculatedDay.eletronicPoint.Trated || exceededLimitAbsencesAndArrears))
                return 0;

            var proportionalDsrKindHour = calculatedDay.eletronicPoint.Employee.Parameter.ProportionalDsrKindHour ?? false;
            var proportionalDsrKindDay = !proportionalDsrKindHour;

            return true switch 
            {
                true when !proportionalDsrKindDay && !proportionalDsrKindHour => 
                    calculatedDay.eletronicPoint.DiscountedDsr = calculatedDay.dsrAmount,
               
                true when proportionalDsrKindHour => 
                    calculatedDay.eletronicPoint.DiscountedDsr = 
                        (exceededLimitArrears ? occurrence.arrearMinutes : 0) + occurrence.absenceMinutes,

                true when !proportionalDsrKindHour => 
                    calculatedDay.eletronicPoint.DiscountedDsr = 
                        (calculatedDay.dsrAmount / week.Count(x=> x.workDay)) * 
                            (exceededLimitArrears ? 
                                week.Count(x=> x.HaveAbsence() || x.HaveArrear()) : 
                                week.Count(x=> x.HaveAbsence())),

                _ => throw new NotImplementedException()
            };
        }

            
        private Occurrence BuildOccurence(in CalculatedDay calculatedDay)
        {
            if(calculatedDay.dsr || !calculatedDay.eletronicPoint.Employee.Parameter.NeedDiscountWeeklyDsr)
                return new Occurrence(0, 0, calculatedDay.eletronicPoint.Date);

            return new Occurrence(calculatedDay.arrearsAmount, 
                                  calculatedDay.absencesAmount,
                                  calculatedDay.eletronicPoint.Date);
        }
        
        private IEnumerable<IEnumerable<CalculatedDay>> CalculateWeeks(DateTime startDate, 
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

        private IEnumerable<CalculatedDay> CalculateWeek(IEnumerable<EletronicPoint> weekPoints)
        {
            foreach (var point in weekPoints)
                yield return CalculateDay(point);
        }

        private CalculatedDay CalculateDay(in EletronicPoint point)
        {
            double[] absences = point.GetAbsences();
            double[] arrears = point.GetArrears();
            bool workday = point.Schedule is not null;

            return true switch
            {
                true when Saturday(point)
                    => new(point.Employee.Parameter.SaturdayDsr, true, 
                            workday, absences.Sum(), arrears.Sum(), point),

                true when DayOff(point)
                    => new(point.Employee.Parameter.DayOffDsr, true, 
                            workday, absences.Sum(), arrears.Sum(), point),

                true when Holiday(point)
                    => new(point.Employee.Parameter.HolidayDsr, true, 
                            workday, absences.Sum(), arrears.Sum(), point),

                true when Sunday(point)
                    => new(point.Employee.Parameter.SundayDsr, true, 
                            workday, absences.Sum(), arrears.Sum(), point),

                _ => new(0, false, 
                            workday, absences.Sum(), arrears.Sum(), point)
            };
        }
        private static bool Saturday(in EletronicPoint point)
        => point.Employee.Parameter.SaturdayDsr != 0
            && point.NonScheduleReference == EletronicPoint.Saturday;
        private static bool DayOff(in EletronicPoint point)
        => point.Employee.Parameter.DayOffDsr != 0
            && point.NonScheduleReference == EletronicPoint.DayOff;

        private static bool Holiday(in EletronicPoint point)
        => point.Employee.Parameter.HolidayDsr != 0
            && point.NonScheduleReference == EletronicPoint.Holiday;

        private static bool Sunday(in EletronicPoint point)
        => point.Employee.Parameter.SundayDsr != 0
            && point.NonScheduleReference == EletronicPoint.Sunday;

        private static DateTime SetStartDate(ref DateTime startDate, in Employee employee)
        =>  startDate = startDate.DayOfWeek == employee.Scale.Turn.WeekTurn.Next()
            ? startDate
            : startDate.AddDays(Math.Abs(startDate.DayOfWeek - employee.Scale.Turn.WeekTurn));

        private static void SetEndDate(ref DateTime endDate, in Employee employee)
        => endDate = endDate.DayOfWeek == employee.Scale.Turn.WeekTurn
                   ? endDate : endDate.AddDays(Math.Abs(endDate.DayOfWeek - employee.Scale.Turn.WeekTurn));
    }
}
>>>>>>> main
