using System;
using System.Linq;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Entities.CommonModels;

namespace GI.ControlePonto.Business.Motor
{
    public class DiaPonto
    {
        public Parametros parametro;
        public  Calendario calendario;
        public string ValorDsr { get; set; }
        public bool IsDiaTrabalho { get; set; } = true;

        public DiaPonto(Ponto ponto, Parametros parametro, Calendario calendario)
        {
            PontoReferencia = ponto;
            this.parametro = parametro;
            this.calendario = calendario;
        }

        public async Task Calcula()
        {
            await Task.Run(() => {
                if(!string.IsNullOrEmpty(parametro.DsrDomingo) && calendario.ReferenciaSemHorario == Ponto.Domingo)
                {
                    IsDsr = true;
                    ValorDsr = parametro.DsrDomingo;
                }
                
                if(!string.IsNullOrEmpty(parametro.DsrFeriado) && calendario.ReferenciaSemHorario == Ponto.Feriado)
                {
                    IsDsr = true;
                    ValorDsr = parametro.DsrFeriado;
                }
                
                if(!string.IsNullOrEmpty(parametro.DsrFolga) && calendario.ReferenciaSemHorario == Ponto.Folga)
                {
                    IsDsr = true;
                    ValorDsr = parametro.DsrFolga;
                }

                if(!string.IsNullOrEmpty(parametro.DsrSabado) && calendario.ReferenciaSemHorario == Ponto.Sabado)
                {
                    IsDsr = true;
                    ValorDsr = parametro.DsrSabado;
                }

                int tryOut;

                if(!int.TryParse(calendario.ReferenciaSemHorario, out tryOut))
                    IsDiaTrabalho = false;

                var faltas = new string[] 
                {
                    PontoReferencia.FaltaDesPer1,
                    PontoReferencia.FaltaDesPer2,
                };

                var atrasos = new string[] 
                {
                    PontoReferencia.AtrasoDesPer1,
                    PontoReferencia.AtrasoDesPer2,
                    PontoReferencia.SaidaDesPer1,
                    PontoReferencia.SaidaDesPer2
                };

                if(atrasos.Any(x=> !string.IsNullOrEmpty(x) && x != "00:00") 
                || faltas.Any(x=> !string.IsNullOrEmpty(x) && x != "00:00"))
                    IsFaltaouAtraso = true;

                if(IsFaltaouAtraso)
                {
                    QtdFaltasEmMinutos = faltas.Sum(x=> Calculadora.HorasparaMinuto(x));
                    QtdAtrasoseSaidasEmMinutos = atrasos.Sum(x=> Calculadora.HorasparaMinuto(x));
                }
            });
        }

        public bool IsDsr { get; set; } = false;

        public bool IsFaltaouAtraso { get; set; } = false;

        public int QtdFaltasEmMinutos { get; set; }

        public int QtdAtrasoseSaidasEmMinutos { get; set; }

        public Ponto PontoReferencia { get; }
    }
}