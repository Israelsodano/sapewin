using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GI.ControlePonto.Business;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Entities.CommonModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlePonto.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class ProcessoController : Controller
    {
        FuncionarioBusiness _funcBusiness;
        MotordeProcessamento _motordeProcessamento;
        ApontamentoSujoBusiness _apontamentoSujoBusiness;

        public ProcessoController(FuncionarioBusiness funcBusiness, 
                                  EmpresaBusiness empBusiness, MotordeProcessamento motordeProcessamento,
                                  ApontamentoSujoBusiness apontamentoSujoBusiness)
        {
            _funcBusiness = funcBusiness;
            _motordeProcessamento = motordeProcessamento;
            _apontamentoSujoBusiness = apontamentoSujoBusiness;
        }

        [HttpPost]
        public async Task<ActionResult<object>> IniciaProcessamento(Processamento processamento)
        {
            try
            {
                //Task task = _motordeProcessamento.IniciaProcessamento(new DateTime(2019, 08, 24), new DateTime(2019, 08, 24), _funcBusiness.ListFuncionarios().ToArray(), TiposProcessamento.Reanalizar, null);
                processamento.Funcionarios = _funcBusiness
                             .GetAll(x=> processamento.Funcionarios
                             .Any(y=> y.IDFuncionario == x.IDFuncionario 
                             && y.IDEmpresa == x.IDEmpresa)).Include("Escala.EscalasHorarios")
                                                            .Include("HorariosOcasionais.Horario")
                                                            .Include("GrupodeFeriados")
                                                            .Include("Parametro")
                                                            .Include("Folgas").ToArray();
                             
                processamento.SetProcessamentoTask(
                    _motordeProcessamento.IniciaProcessamento(processamento.Dataini, 
                                                              processamento.Datafim, 
                                                              processamento.Funcionarios, 
                                                              processamento.TipoProcessamento,
                                                              processamento)
                                                  );
                
                await processamento.Await();

                return new { success = true };
            }
            catch (Exception ex)
            {
                return new { success = false, msg = ex.Message };
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> ImportaAFD(IFormFile Afd)
        {
            await _apontamentoSujoBusiness.SalvaAfd(Afd.OpenReadStream());
            return null;
        }
    }
}
