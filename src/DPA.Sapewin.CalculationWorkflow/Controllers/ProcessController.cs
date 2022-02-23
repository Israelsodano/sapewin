using System.Threading.Tasks;
using System;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using DPA.Sapewin.CalculationWorkflow.Domain.Commands;
using System.ComponentModel.DataAnnotations;
using DPA.Sapewin.CalculationWorkflow.Domain.Events;

namespace DPA.Sapewin.CalculationWorkflow.Controllers
{
    [ApiController, Route("api/[controller]/[action]")]
    public class ProcessController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public ProcessController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentException(nameof(publishEndpoint));    
        }

        [HttpPost]
        public async Task<IActionResult> ProcessAsync([FromHeader(Name = "x-correlation")][Required] Guid correlationId)
        {
            await _publishEndpoint.Publish<IStartProcessEventWasSubmitted>(new 
            {
                correlationId
            });
            
            return Ok();
        }
    }
}