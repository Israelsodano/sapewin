using System;
using System.Collections.Generic;
using System.Linq;
using Automatonymous;
using MassTransit.Saga;

namespace DPA.Sapewin.CalculationWorkflow.Application.Saga
{
    public class CalculationWorkflowSagaInstance : SagaStateMachineInstance, ISagaVersion
    {
        public string CurrentState { get; set; }
        public DateTime OrderDate { get; set; }
        public IList<object> Events { get; set; }
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public CompositeEventStatus Status { get; set; }


        public void Apply(object o)
        {
            Events ??= new List<object>();
            Events.Add(o);
        }

        public T GetEvent<T>() => (T)(Events.FirstOrDefault(x => x.GetType() == typeof(T)) ?? default(T));
    }
}