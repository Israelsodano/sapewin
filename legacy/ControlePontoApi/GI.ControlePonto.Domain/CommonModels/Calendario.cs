using System.Linq;
using System.Collections.Generic;
using System;


namespace GI.ControlePonto.Domain.Entities.CommonModels
{
    public class Calendario
    {
        public Calendario(DateTime data, Horarios horarios)
        {
            Data = data;
            Horario = horarios;
        }

        public Calendario(DateTime data, 
                          Horarios horarios, 
                          Afastamentos afastamento, 
                          Funcionarios funcionario,
                          IList<FeriadosGerais> feriadosGerais,
                          EscalasHorarios escalasHorarios)
        {
            Data = data;
            Horario = horarios;

            if(afastamento == null)
            {
                if(((!funcionario.IDFeriado.HasValue ? 
                        false : 
                        funcionario.GrupodeFeriados
                            .FeriadosEspecificos
                                .Any(x=> x.Dia == data.Day && 
                                         x.Mes == data.Month && 
                                        (x.Ano == 0 || 
                                         x.Ano == data.Year))) || 
                    (feriadosGerais.Any(x=> x.Dia == data.Day && 
                                         x.Mes == data.Month && 
                                        (!x.Ano.HasValue || 
                                         x.Ano == data.Year)))) && 
                    funcionario.Feriado == Funcionarios.feriado.Folga)
                {
                    ReferenciaSemHorario = Ponto.Feriado;
                }
                else{
                    ReferenciaSemHorario = escalasHorarios.IDHorario;
                }
            }else{
                ReferenciaSemHorario = afastamento.Abreviacao;
            }
    
            if(funcionario.Folgas.Any(x=>x.Data.Date == data.Date)){
                ReferenciaSemHorario = Ponto.Folga;
                Horario = null;
            }
        }

        public DateTime Data { get; set; }
        public Horarios Horario { get; set; }
        public string ReferenciaSemHorario { get; set; }
    }
}