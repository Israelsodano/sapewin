using System;
using System.Collections.Generic;
using System.Linq;
using Automatonymous;
using DPA.Sapewin.Domain.Models.Enums;
using MassTransit.Saga;
using Newtonsoft.Json.Linq;

namespace DPA.Sapewin.CalculationWorkflow.Application.Saga
{
    public class CalculationWorkflowSagaInstance : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public Guid[] Employees { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProcessingType ProcessingType { get; set; }
        public IDictionary<string, JObject> Events { get; set; }
        public int Version { get; set; }

        public void Apply(Guid[] employees, 
                          DateTime startDate,
                          DateTime endDate,
                          ProcessingType ProcessingType)
        {
           Employees = employees;
           StartDate = startDate;
           EndDate = endDate;
        }

        public void Apply(object o)
        {
            Events ??= new Dictionary<string, JObject>();
            Events.Add(o.GetType().Name, JObject.FromObject(o));
        }

        public T GetEvent<T>() where T : class 
        {
            var e = Events.FirstOrDefault(x=> x.Key == typeof(T).Name);
            return e.Value?.ToObject<T>();
        }
    }
}