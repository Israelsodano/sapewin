using System;
using MassTransit;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Domain.Commands.Calculation
{
    public abstract class BaseCalculationCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> PairsByEletronicPointToCalculate { get; set; }

    }
}