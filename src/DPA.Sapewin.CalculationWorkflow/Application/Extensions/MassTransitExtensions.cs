using Automatonymous;
using DPA.Sapewin.CalculationWorkflow.Application.Consumers;
using DPA.Sapewin.CalculationWorkflow.Application.Saga;
using DPA.Sapewin.CalculationWorkflow.Domain.Commands;
using MassTransit;
using MassTransit.Saga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPA.Sapewin.CalculationWorkflow.Application.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISagaRepository<CalculationWorkflowSagaInstance>>(x => new InMemorySagaRepository<CalculationWorkflowSagaInstance>());
            services.AddSingleton<SagaStateMachine<CalculationWorkflowSagaInstance>, CalculationWorkflowSagaStateMachine>();

            services.AddMassTransit(configure =>
            {
                configure.AddSagaStateMachine<CalculationWorkflowSagaStateMachine, CalculationWorkflowSagaInstance>();

                configure.AddConsumer<CalculationWorkflowConsumer>();

                configure.AddBus(registration => Bus.Factory.CreateUsingRabbitMq(context =>
                {
                    context.Host(configuration.GetConnectionString("RabbitMq"));

                    var sagaEndpoint = "calculation-workflow-saga";

                    context.Message<StartProcessCommand>(x => x.SetEntityName(sagaEndpoint));

                    context.ReceiveEndpoint(sagaEndpoint, e =>
                    {
                        e.StateMachineSaga<CalculationWorkflowSagaInstance>(registration);
                    });
                }));
            });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}