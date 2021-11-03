using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class Escalas
    {        
        public int IDEscala { get; set; }

        public int IDEmpresa { get; set; }

        public String Descricao { get; set; }

        public tipo Tipo { get; set; }

        public DateTime DataInicio { get; set; }

        public bool Liv_AdcSab { get; set; }

        public bool Liv_AdcDom { get; set; }

        public bool Liv_Fer { get; set; }

        public bool Liv_Fol { get; set; }

        public bool Liv_Virarda { get; set; }

        public string Virada_Sab { get; set; }

        public string Virada_Dom { get; set; }

        public string Virada_Fer { get; set; }

        public string Virada_Fol { get; set; }
        
        public string Virada_Afa { get; set; }

        public viradaSemana ViradaSemana { get; set; }

        public IList<EscalasHorarios> EscalasHorarios { get; set; }

        public virtual IList<Funcionarios> Funcionarios { get; set; }

        public enum tipo
        {
            Semanal = 1, Revezamento = 2, CargaSemanal = 3, Livre = 4
        };

        public enum viradaSemana
        {
            Segunda = 1, Terca = 2, Quarta = 3, Quinta = 4, Sexta = 5, Sabado = 6, Domingo = 7
        };
    }
}