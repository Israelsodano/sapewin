using System.Collections.Generic;
using System.Linq;
using GI.ControlePonto.Business;
using GI.ControlePonto.Domain.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ControlePonto.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class DepartamentoController : Controller
    {
        DepartamentoBusiness _business;

        public DepartamentoController(DepartamentoBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Departamentos>> Get(int[] empresas) =>
             _business.GetAll(x=> empresas.Contains(x.IDEmpresa)).ToArray();
        
    }
}
