using System;
using System.Linq;
using System.Threading.Tasks;
using DPA.Sapewin.CalculationWorkflow.Application.Services;
using DPA.Sapewin.CalculationWorkflow.Domain.Events.ImportDirtyNotes;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DPA.Sapewin.CalculationWorkflow.Application.Consumers
{
    public class ImportDirtyNotesConsumer : IConsumer<IImportDirtyNotesSubmittedEvent>
    {
        private readonly IAppointmentsService _appointmentsService;
        private readonly IUnitOfWork<Employee> _employeeUnitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public ImportDirtyNotesConsumer(IAppointmentsService appointmentsService,
                                   IUnitOfWork<Employee> employeeUnitOfWork,
                                   IPublishEndpoint publishEndpoint)
        {
            _appointmentsService = appointmentsService ?? throw new ArgumentNullException(nameof(appointmentsService));
            _employeeUnitOfWork = employeeUnitOfWork ?? throw new ArgumentNullException(nameof(employeeUnitOfWork));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        public async Task Consume(ConsumeContext<IImportDirtyNotesSubmittedEvent> context)
        {
            var employees = await _employeeUnitOfWork.Repository
                                .GetAll(x=> context.Message.Employees.Contains(x.Id))
                                .Include("")
                                .ToArrayAsync();

            var appointments = await _appointmentsService.ImportDirtyNotes(employees, 
                                                  context.Message.StartDate,
                                                  context.Message.EndDate);

            await _publishEndpoint.Publish<IDirtyNotesSuccessfullyImportedEvent>(new {
                context.Message.CorrelationId
            });
        }
    }
}