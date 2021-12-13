using Automatonymous;
using DPA.Sapewin.CalculationWorkflow.Domain.Commands;
using DPA.Sapewin.CalculationWorkflow.Domain.Commands.Calculation;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Application.Saga
{
    public class StartProcessStateMachine :
        MassTransitStateMachine<StartProcessSagaInstance>
    {
        public StartProcessStateMachine()
        {
            Event(() => CalculateArrears, x => x.CorrelateById(x => x.Message.CorrelationId));

            InstanceState(x => x.CurrentState);

            Initially(
                When(StartProcess)
                    .Then(x => x.Instance.Apply(x.Data))
                    .PublishAsync(context => context.Init<ClearTableCommand>(context.Data))
                    .TransitionTo(CleaningTables),
                When(StartProcess)
                    .Then(x => x.Instance.Apply(x.Data))
                    .PublishAsync(context => context.Init<BuildEmployeeCalendarsCommand>(context.Data))
                    .TransitionTo(BuildingEmployeeCalendars));
            During(BuildingEmployeeCalendars,
                When())

        }
        public State CleaningTables { get; private set; }
        public State BuildingEmployeeCalendars { get; private set; }
        public State CalculatingArrears { get; private set; }
        public State CalculatingEarlyWayOuts { get; private set; }
        public State CalculatingWorkedHours { get; private set; }
        public State CalculatingAbsences { get; private set; }
        public State CalculatingExtraHours { get; private set; }
        public State CalculatingNightlyAdditional { get; private set; }


        public Event<StartProcessCommand> StartProcess { get; private set; }
        public Event<ClearTableCommand> ClearTable { get; private set; }
        public Event<GetGenericHolidaysCommand> GetGenericHolidays { get; private set; }
        public Event<BuildEmployeeCalendarsCommand> BuildEmployeeCalendars { get; private set; }
        public Event<CalculateArrearsCommand> CalculateArrears { get; private set; }
        public Event<CalculateEarlyWayOutCommand> CalculateEarlyWayOut { get; private set; }
        public Event<CalculateWorkedHoursCommand> CalculateWorkedHours { get; private set; }
        public Event<CalculateAbsencesCommand> CalculateAbsences { get; private set; }
        public Event<CalculateExtraHoursCommand> CalculateExtraHours { get; private set; }
        public Event<CalculateNightlyAdditionalCommand> CalculateNightlyAdditional { get; private set; }
    }
}