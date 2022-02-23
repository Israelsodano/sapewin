using System;
using System.Linq;
using System.Threading.Tasks;
using DPA.Sapewin.CalculationWorkflow.Application.Services;
using DPA.Sapewin.CalculationWorkflow.Domain.Events.BuildCalendars;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DPA.Sapewin.CalculationWorkflow.Application.Consumers
{
    public class BuildEmployeesCalendarConsumer : IConsumer<IBuildEmployeeCalendarsWasSubmittedEvent>
    {
        private readonly IUnitOfWork<Employee> _employeeUnitOfWork;
        private readonly IUnitOfWork<GenericHoliday> _genericHolidayUnitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        public readonly IEmployeeCalendarService _employeeCalendarService;

        public BuildEmployeesCalendarConsumer(IEmployeeCalendarService employeeCalendarService,
                                            IUnitOfWork<Employee> employeeUnitOfWork,
                                            IUnitOfWork<GenericHoliday> genericHolidayUnitOfWork,
                                            IPublishEndpoint publishEndpoint)
        {
            _employeeCalendarService = employeeCalendarService ?? throw new ArgumentNullException(nameof(employeeCalendarService));
            _employeeUnitOfWork = employeeUnitOfWork ?? throw new ArgumentNullException(nameof(employeeUnitOfWork));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Consume(ConsumeContext<IBuildEmployeeCalendarsWasSubmittedEvent> context)
        {
            var employees = await _employeeUnitOfWork.Repository
                                .GetAll(x=> context.Message.Employees.Contains(x.Id))
                                .Include("")
                                .ToArrayAsync();

            var holidays = await _genericHolidayUnitOfWork.Repository.GetAll(x=> 
                (x.Day >= context.Message.StartDate.Day && 
                 x.Month >= context.Message.StartDate.Month && 
                 (x.Year == 0 || x.Year >= context.Message.StartDate.Year)) && 
                 
                (x.Day <= context.Message.EndDate.Day && 
                 x.Month <= context.Message.EndDate.Month && 
                 (x.Year == 0 || x.Year <= context.Message.EndDate.Year))).ToArrayAsync();

            var employeeCalendars = await _employeeCalendarService.BuildEmployeeCalendars(employees,
                                                                                  context.Message.StartDate,
                                                                                  context.Message.EndDate,
                                                                                  holidays).ToArrayAsync();
            
            await _publishEndpoint.Publish<IEmployeeCalendarsSuccessfullyBuildedEvent>(new {
                context.Message.CorrelationId,
                employeeCalendars
            });
        }           
    }
}