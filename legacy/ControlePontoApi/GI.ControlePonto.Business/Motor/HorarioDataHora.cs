using System.Collections.Generic;
using System;
using GI.ControlePonto.Domain.Entities;

namespace GI.ControlePonto.Business.Motor
{
    public class HorarioDataHora
    {
        private bool VerificaOutroDia(string entrada, string outroHorario) =>
            Calculadora.HorasparaMinuto(entrada) > Calculadora.HorasparaMinuto(outroHorario);
            
        public HorarioDataHora(Horarios horario, DateTime data)
        {
            DatasHorario = new Dictionary<string, DateTime>();
            DatasHorario.Add(horario.Entrada, Calculadora.HoraEmDateTime(horario.Entrada, data));
            DatasHorario.Add(horario.EntradaIntervalo, Calculadora.HoraEmDateTime(horario.EntradaIntervalo, data.AddDays(VerificaOutroDia(horario.Entrada, horario.EntradaIntervalo) ? 1 : 0)));
            DatasHorario.Add(horario.SaidaIntervalo, Calculadora.HoraEmDateTime(horario.SaidaIntervalo, data.AddDays(VerificaOutroDia(horario.Entrada, horario.SaidaIntervalo) ? 1 : 0)));
            DatasHorario.Add(horario.Saida, Calculadora.HoraEmDateTime(horario.Saida, data.AddDays(VerificaOutroDia(horario.Entrada, horario.Saida) ? 1 : 0)));    

            foreach (var intervalo in horario.IntervalosAuxiliares)
            {
                DatasHorario.Add(intervalo.Inicio, Calculadora.HoraEmDateTime(intervalo.Inicio, data.AddDays(VerificaOutroDia(horario.Entrada, intervalo.Inicio) ? 1 : 0)));    
                DatasHorario.Add(intervalo.Fim, Calculadora.HoraEmDateTime(intervalo.Fim, data.AddDays(VerificaOutroDia(horario.Entrada, intervalo.Fim) ? 1 : 0)));    
            }
        }

        public HorarioDataHora()
        {
            
        }

        public IDictionary<string, DateTime> DatasHorario { get; set; }
    }
}