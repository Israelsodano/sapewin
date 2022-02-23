using Automatonymous;
using DPA.Sapewin.CalculationWorkflow.Domain.Events;
using DPA.Sapewin.CalculationWorkflow.Domain.Events.BuildCalendars;
using DPA.Sapewin.CalculationWorkflow.Domain.Events.ClearTables;
using DPA.Sapewin.CalculationWorkflow.Domain.Events.ImportDirtyNotes;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Application.Saga
{
    public class CalculationWorkflowSagaStateMachine :
        MassTransitStateMachine<CalculationWorkflowSagaInstance>
    {
        public CalculationWorkflowSagaStateMachine()
        {
            Event(() => StartProcess, x => x.CorrelateById(x => x.Message.CorrelationId));

            InstanceState(x=> x.CurrentState);
          
            Initially(
                When(StartProcess)
                    .Then(x => { 
                        x.Instance.Apply(x.Data);
                        x.Instance.Apply(x.Data.Employees, 
                                         x.Data.StartDate,
                                         x.Data.EndDate,
                                         x.Data.ProcessingType); 
                    })
                    .PublishAsync(context => context.Init<IClearTablesWasSubmittedEvent>(new {
                        context.Data.CorrelationId,
                        context.Data.Employees,
                        context.Data.StartDate,
                        context.Data.EndDate,
                        context.Data.ProcessingType,
                    }))
                    .PublishAsync(context => context.Init<IBuildEmployeeCalendarsWasSubmittedEvent>(new {
                        context.Data.CorrelationId,
                        context.Data.Employees,
                        context.Data.StartDate,
                        context.Data.EndDate,
                    }))
                    .TransitionTo(InitialBuildsAndImportations));

            CompositeEvent(() => BuildAndImportationsWasDone, 
                            x => x.CurrentState, 
                            CalendarsWasBuilded, 
                            BuildedCalendarsWasDone,
                            ImportDirtyNotesWasDone);

            During(InitialBuildsAndImportations, 
                When(TablesWasCleared)
                .Then(x => x.Instance.Apply(x.Data))
                .Publish(context => context.Init<IClearTablesWasDoneEvent>(new { 
                    context.Data.CorrelationId
                }))
                .Publish(context => context.Init<IImportDirtyNotesSubmittedEvent>(new { 
                    context.Data.CorrelationId,
                    context.Instance.Employees,
                    context.Instance.StartDate,
                    context.Instance.EndDate,
                })),
                When(CalendarsWasBuilded)
                .Then(x => x.Instance.Apply(x.Data))
                .Publish(context => context.Init<IBuildCalendarsWasDoneEvent>(new { 
                    context.Data.CorrelationId 
                })),
                When(DirtyNotesWasImported)
                .Then(x => x.Instance.Apply(x.Data))
                .Publish(context => context.Init<ImportDirtyNotesWasDoneEvent>(new { 
                    context.Data.CorrelationId 
                })), 
                When(BuildAndImportationsWasDone));

        
        }

        public State InitialBuildsAndImportations { get; private set; }
        

        public Event BuildAndImportationsWasDone { get; private set; }
        public Event<IStartProcessEventWasSubmitted> StartProcess { get; private set; }
        public Event<ITablesWasClearedSuccessfullyEvent> TablesWasCleared { get; private set; }
        public Event<IClearTablesWasDoneEvent> ClearedTablesWasDone { get; private set; }
        public Event<IEmployeeCalendarsSuccessfullyBuildedEvent> CalendarsWasBuilded { get; private set; }
        public Event<IBuildCalendarsWasDoneEvent> BuildedCalendarsWasDone { get; private set; }
        public Event<IDirtyNotesSuccessfullyImportedEvent> DirtyNotesWasImported { get; private set; }
        public Event<ImportDirtyNotesWasDoneEvent> ImportDirtyNotesWasDone { get; private set; }
    }
}