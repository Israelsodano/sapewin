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

            var weeks = (int)(endDate - startDate).TotalDays / 6;

            for (var i = 0; i < weeks; i++)
            {
                var endWeekDate = startDate.AddDays(7);
                var weekPoints = eletronicPoints.Where(x => x.Date >= startDate && x.Date <= endWeekDate);


            }

        }

        private decimal Calculate(EletronicPoint point, EmployeeCalendars employeeCalendars)
        {

        }
        private void SetStartDate(ref DateTime startDate, in Employee employee)
        => startDate = startDate.DayOfWeek == employee.Scale.Turn.WeekTurn.Next()
            ? startDate
            : startDate.AddDays(startDate.DayOfWeek < employee.Scale.Turn.WeekTurn
                ? startDate.DayOfWeek - employee.Scale.Turn.WeekTurn
                : employee.Scale.Turn.WeekTurn - startDate.DayOfWeek);

        private void SetEndDate(ref DateTime endDate, in Employee employee)
        => endDate = endDate.DayOfWeek == employee.Scale.Turn.WeekTurn
                   ? endDate
                   : endDate.AddDays(Math.Abs(endDate.DayOfWeek - employee.Scale.Turn.WeekTurn));
    }
}