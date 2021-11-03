using System.Collections.Generic;
using System.Linq;
using GI.ControlePonto.Business;
using GI.ControlePonto.Domain.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ControlePonto.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class EmpresaController : Controller
    {
        EmpresaBusiness _business;

        public EmpresaController(EmpresaBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Empresas>> Get() =>
             _business.GetAll().ToList();
        
    }
}
