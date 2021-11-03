using System;

namespace GI.ControlePonto.Domain.Entities
{
    public class EscalasHorarios
    {
        public virtual int IDEmpresa { get; set; }

        public virtual int IDEscala { get; set; }

        public virtual String IDHorario { get; set; }

        public virtual int Ordem { get; set; }

        public virtual int QuantidadedeDias { get; set; }

        public virtual string Dias { get; set; }

        public virtual bool Direto { get; set; }

        public virtual diainicio DiaInicio { get; set; }

        public virtual string HoradeEntrada { get; set; }

        public virtual Empresas Empresa { get; set; }

        public virtual Escalas Escala { get; set; }

        public enum diainicio
        {
            Segunda = 1, Terca = 2, Quarta = 3, Quinta = 4, Sexta = 5, Sabado = 6, Domingo = 7
        }        
    }
}