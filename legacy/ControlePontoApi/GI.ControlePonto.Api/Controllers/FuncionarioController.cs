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
    public class FuncionarioController : Controller
    {
        FuncionarioBusiness _business;

        public FuncionarioController(FuncionarioBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Funcionarios>> Get(string[] setores, string[] departamentos)
        {
            Setores[] setoresList = setores.Select((x)=> 
            { 
               return new Setores
               { 
                    IDSetor = int.Parse(x.Split('-')[0]),
                    IDEmpresa = int.Parse(x.Split('-')[1]),
               }; 
            }).ToArray(); 

            Departamentos[] departamentosList = departamentos.Select((x) => {
                return new Departamentos
                {
                    IDDepartamento = int.Parse(x.Split('-')[0]),
                    IDEmpresa = int.Parse(x.Split('-')[1]),
                };
            }).ToArray();  
            
            return _business.GetAll(x=> 
                            setoresList
                                .Any(y=> y.IDSetor == x.IDSetor 
                                && y.IDEmpresa == x.IDEmpresa) 
                         || departamentosList
                                .Any(y=> y.IDDepartamento == x.IDDepartamento 
                                && y.IDEmpresa == x.IDEmpresa)).ToList(); 
        } 
        
    }
}
