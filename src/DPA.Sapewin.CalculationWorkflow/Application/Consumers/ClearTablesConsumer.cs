using System.Linq;
using System;
using System.Threading.Tasks;
using DPA.Sapewin.CalculationWorkflow.Application.Services;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using DPA.Sapewin.CalculationWorkflow.Domain.Events.ClearTables;

namespace DPA.Sapewin.CalculationWorkflow.Application.Consumers
{
    public class ClearTablesConsumer : IConsumer<IClearTablesWasSubmittedEvent>
    {
        private readonly IClearTablesService _clearTablesService;
        private readonly IUnitOfWork<Employee> _employeeUnitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public ClearTablesConsumer(IClearTablesService clearTablesService,
                                   IUnitOfWork<Employee> employeeUnitOfWork,
                                   IPublishEndpoint publishEndpoint)
        {
            _clearTablesService = clearTablesService ?? throw new ArgumentNullException(nameof(clearTablesService));
            _employeeUnitOfWork = employeeUnitOfWork ?? throw new ArgumentNullException(nameof(employeeUnitOfWork));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Consume(ConsumeContext<IClearTablesWasSubmittedEvent> context)
        {
            var employees = await _employeeUnitOfWork.Repository
                                .GetAll(x=> context.Message.Employees.Contains(x.Id))
                                .Include("")
                                .ToArrayAsync();
            
            await _clearTablesService.Clear(context.Message.StartDate, 
                                      context.Message.EndDate,
                                      employees,
                                      context.Message.ProcessingType);

            await _publishEndpoint.Publish<ITablesWasClearedSuccessfullyEvent>(new {
                context.Message.CorrelationId
            });
        }
    }
}