using System.Threading.Tasks;
using DPA.Sapewin.CalculationWorkflow.Domain.Commands;
using MassTransit;

namespace DPA.Sapewin.CalculationWorkflow.Application.Consumers
{
    public class CalculationWorkflowConsumer : IConsumer<StartProcessCommand>
    {
        public Task Consume(ConsumeContext<StartProcessCommand> context)
        {
            throw new System.NotImplementedException();
        }
    }
}