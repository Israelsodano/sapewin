using Automatonymous;
using DPA.Sapewin.CalculationWorkflow.Application.Consumers;
using DPA.Sapewin.CalculationWorkflow.Application.Saga;
using DPA.Sapewin.CalculationWorkflow.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPA.Sapewin.CalculationWorkflow.Application.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SagaStateMachine<CalculationWorkflowSagaInstance>, CalculationWorkflowSagaStateMachine>();

            services.AddMassTransit(configure =>
            {
                configure.AddSagaStateMachine<CalculationWorkflowSagaStateMachine, 
                                              CalculationWorkflowSagaInstance>()
                                .RedisRepository(configuration.GetConnectionString("Redis"));

                configure.AddConsumer<ClearTablesConsumer>();

                configure.UsingRabbitMq((context, cfg) => {

                    cfg.Host(configuration.GetConnectionString("RabbitMq"));

                    var sagaEndpoint = "calculation-workflow-saga";

                    cfg.Message<IStartProcessEventWasSubmitted>(x => x.SetEntityName(sagaEndpoint));

                    cfg.ReceiveEndpoint(sagaEndpoint, e =>
                    {
                        e.StateMachineSaga<CalculationWorkflowSagaInstance>(context);
                    });
                });
            });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}