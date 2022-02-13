using System;
using System.Collections.Generic;
using Automatonymous;
using DPA.Sapewin.CalculationWorkflow.Domain.Commands;
using DPA.Sapewin.CalculationWorkflow.Domain.Commands.Calculation;
using DPA.Sapewin.CalculationWorkflow.Domain.Events;
using DPA.Sapewin.Domain.Entities;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Application.Saga
{
    public class CalculationWorkflowSagaStateMachine :
        MassTransitStateMachine<CalculationWorkflowSagaInstance>
    {
        public CalculationWorkflowSagaStateMachine(DateTime startDate, DateTime endDate, IEnumerable<Employee> employees)
        {
            Event(() => StartProcess, x => x.CorrelateById(x => x.Message.CorrelationId));
            CompositeEvent(SnapAppointmentsCompositeEvent, x => x.Status., AppointmentsWereGotten, EletronicPointsWereGotten, EmployeeCalendarsWereBuilt);

            DuringAny(
                When(SnapAppointmentsCompositeEvent)
                    .Then(x => x.Instance.Apply(x.)))
            InstanceState(x => x.CurrentState);

            Initially(
                When(StartProcess)
                    .Then(x => x.Instance.Apply(x.Data))
                    .PublishAsync(context => context.Init<ClearTableCommand>(context.Data))
                    .TransitionTo(CleaningTables),

                When(StartProcess)
                    .Then(x => x.Instance.Apply(x.Data))
                    .PublishAsync(context => context.Init<GetGenericHolidaysCommand>(context.Data))
                    .TransitionTo(GettingGenericHolidays));

            During(GettingGenericHolidays,
                When(GenericHolidaysWereGotten)
                    .Then(x => x.Instance.Apply(x.Data))
                    .Publish(context
                        => context.Init<BuildEmployeeCalendarsCommand>(
                                BuildEmployeeCalendarsCommand.BuildInitialized(
                                    startDate, endDate, employees, context.Data.GottenGenericHolidays)))
                    .TransitionTo(BuildingEmployeeCalendars));

            During(BuildingEmployeeCalendars,
                When(EmployeeCalendarsWereBuilt)
                    .Then(x => x.Instance.Apply(x.Data))
                    .TransitionTo(GettingAppointments),

                When(AppointmentsWereGotten)
                    .Then(x => x.Instance.Apply(x.Data))
                    .Publish(context => context.Init<GetEletronicPointsCommand>(context.Data))
                    .TransitionTo(GettingEletronicPoints));
        }
        public State CleaningTables { get; private set; }
        public State BuildingEmployeeCalendars { get; private set; }
        public State CalculatingArrears { get; private set; }
        public State CalculatingEarlyWayOuts { get; private set; }
        public State CalculatingWorkedHours { get; private set; }
        public State CalculatingAbsences { get; private set; }
        public State CalculatingExtraHours { get; private set; }
        public State CalculatingNightlyAdditional { get; private set; }
        public State GettingGenericHolidays { get; private set; }
        public State GettingAppointments { get; private set; }
        public State GettingEletronicPoints { get; private set; }
        public State GettingDirtyAppointments { get; private set; }


        #region Composite
        public Event SnapAppointmentsCompositeEvent { get; private set; }

        #endregion
        #region Commands
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
        public Event<GetAppointmentsCommand> GetAppointments { get; private set; }
        public Event<GetEletronicPointsCommand> GetEletronicPoints { get; private set; }
        #endregion

        #region Events
        public Event<GenericHolidaysWereGottenEvent> GenericHolidaysWereGotten { get; private set; }
        public Event<EmployeeCalendarsWereBuiltEvent> EmployeeCalendarsWereBuilt { get; private set; }
        public Event<AppointmentsWereGottenEvent> AppointmentsWereGotten { get; private set; }
        public Event<EletronicPointsWereGottenEvent> EletronicPointsWereGotten { get; private set; }


        #endregion
    }
}