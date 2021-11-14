using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Repository;
using DPA.Sapewin.Domain.Models;
using System.Linq;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IEletronicPointService
    {
        IAsyncEnumerable<IGrouping<Employee, EletronicPoint>> GetAggregatedEletronicPointsByEmployee(IEnumerable<EmployeeCalendars> employeesCalendars,
                                                                                                     IEnumerable<Appointment> appointments);
    }

    public class EletronicPointService : IEletronicPointService
    {
        private readonly IUnitOfWork<EletronicPoint> _unitOfWorkEletronicPoint;
        

        public EletronicPointService(IUnitOfWork<EletronicPoint> unitOfWorkEletronicPoint)
        {
            _unitOfWorkEletronicPoint = unitOfWorkEletronicPoint ?? throw new ArgumentNullException(nameof(unitOfWorkEletronicPoint));
        }

        public async IAsyncEnumerable<IGrouping<Employee, EletronicPoint>> GetAggregatedEletronicPointsByEmployee(IEnumerable<EmployeeCalendars> employeesCalendars,
                                                                                                       IEnumerable<Appointment> appointments)
        {
            foreach (var employeeCalendars in employeesCalendars)
            {
                var eeletronicPoints = BuildEletronicPoints(employeeCalendars,
                                                (from a in appointments
                                                where a.EmployeeId == employeeCalendars.Employee.Id &&
                                                    a.CompanyId == employeeCalendars.Employee.CompanyId
                                                select a).ToArray())
                                                .ToArray();
                
                await _unitOfWorkEletronicPoint.Repository.InsertAsync(eeletronicPoints);
                await _unitOfWorkEletronicPoint.SaveChangesAsync();

                yield return new Grouping<Employee, EletronicPoint>(employeeCalendars.Employee,
                                                                    eeletronicPoints);   

            }   
        }
        
        private IEnumerable<EletronicPoint> BuildEletronicPoints(EmployeeCalendars employeeCalendars, 
                                                                 IEnumerable<Appointment> appointments)
        {
            foreach (var calendar in employeeCalendars.Calendars)
            {
                GetScheduleTurnByScale(employeeCalendars.Employee, 
                                       calendar.NonScheduleReference, out var mturn);

                var rschedules = GetReferencePreviousAndLaterSchedules(calendar, employeeCalendars);

                var rturns = GetPreviousAndLaterTurnDate(mturn, 
                                                         calendar,
                                                         rschedules.pschedule,
                                                         rschedules.refschedule, 
                                                         rschedules.pschedule);

                var pappointments = (from a in appointments
                                   where a.DateHour >= rturns.pturn &&
                                         a.DateHour <= rturns.lturn                                         
                                   select a).ToArray();
                
                yield return new EletronicPoint
                {
                    Date = calendar.Date.Date,
                    Appointments = pappointments,
                    Schedule = rschedules.refschedule,
                    EmployeeId = employeeCalendars.Employee.Id,
                    Employee = employeeCalendars.Employee
                };
            }
        }


        private (DateTime pturn, DateTime lturn) GetPreviousAndLaterTurnDate(int? mturn, 
                                                                             Calendar calendar,
                                                                             Schedule pschedule,
                                                                             Schedule refschedule,
                                                                             Schedule lschedule)
        {
            int minutesturn = GetPreviousMinutesTurn(mturn, 
                                                 pschedule, 
                                                 refschedule);

            DateTime pturn = calendar.Date
                                .AddDays(minutesturn < refschedule.Period.Entry ? 0 : -1)
                                .AddMinutes(minutesturn);
            
            minutesturn = GetLaterMinutesTurn(mturn, 
                                                lschedule, 
                                                refschedule);

            DateTime lturn = calendar.Date
                                .AddDays(refschedule.Period.WayOut < minutesturn ? 0 : 1)
                                .AddDays(refschedule.IsTwentyFourHours ? 1 : 0)
                                .AddMinutes(minutesturn);

            
            return (pturn, lturn);
        }


        private int CalibrateDay(int m) => m >= 0 ? m : m + 1440;

        private int GetPreviousMinutesTurn(int? mturn,
                                   Schedule pschedule,
                                   Schedule refschedule) => 
            mturn ?? 
                refschedule.DayTurn ?? 
                    (CalibrateDay(pschedule.Period.Entry - refschedule.Period.WayOut) / 2)  
                        + pschedule.Period.WayOut;

        private int GetLaterMinutesTurn(int? mturn,
                                   Schedule lschedule,
                                   Schedule refschedule) => 
            mturn ?? 
                refschedule.DayTurn ?? 
                    (CalibrateDay(lschedule.Period.Entry - refschedule.Period.WayOut) / 2)  
                        + refschedule.Period.WayOut;

        private (Schedule refschedule, Schedule pschedule, Schedule lschedule) GetReferencePreviousAndLaterSchedules(Calendar calendar, 
                                                                                                                   EmployeeCalendars employeeCalendars)
        {
            Schedule refschedule = calendar.Schedule ?? employeeCalendars.GetNearestCalendarWithSchedule(calendar.Date).Schedule,
                      pschedule = employeeCalendars.GetNearestCalendarWithSchedule(calendar.Date.AddDays(-1)).Schedule ?? 
                                        refschedule,
                      lschedule = employeeCalendars.GetNearestCalendarWithSchedule(calendar.Date.AddDays(1)).Schedule ?? 
                                        refschedule;
            
            return (refschedule, pschedule, lschedule);
        }
                                    
        private int? GetScheduleTurnByScale(Employee employee, string reference, out int? turn) => reference switch 
        {
            EletronicPoint.DayOff => turn = employee.Scale.Turn.DayOff,
            EletronicPoint.Holiday => turn = employee.Scale.Turn.Holiday,
            EletronicPoint.Saturday => turn = employee.Scale.Turn.Saturday,
            EletronicPoint.Sunday => turn = employee.Scale.Turn.Sunday,
            _ => turn = null
        };
    }
}