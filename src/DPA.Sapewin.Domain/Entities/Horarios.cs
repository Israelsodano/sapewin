using System;
using System.Collections.Generic;

namespace DPA.Sapewin.Domain.Entities
{
    public class Horarios
    {
        public int Dias;

        public int IDRef;

        public int Order;

        public ScheduleScalesStartDay DiaInicio;

        public bool Direto;

        public string HoradeEntrada;

        public virtual int IDHorario { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Entrada { get; set; }

        public virtual String EntradaIntervalo { get; set; }

        public virtual String SaidaIntervalo { get; set; }

        public virtual String Saida { get; set; }

        public virtual String TotaldoPeriodo1 { get; set; }

        public virtual String TotaldoPeriodo2 { get; set; }

        public virtual String TotaldoIntervalo { get; set; }

        public virtual String TotalJornada { get; set; }

        public virtual String ViradadoDia { get; set; }

        public virtual bool NDescontarIntervalo { get; set; }

        public virtual bool VintqHoras { get; set; }

        public virtual bool CargaSemanal { get; set; }

        public virtual String Carga { get; set; }

        public virtual tipo Tipo { get; set; }

        public virtual Companies Empresa { get; set; }        

        public virtual IList<IntervalosAuxiliares> IntervalosAuxiliares { get; set; }

        public virtual IList<HorariosOcasionais> HorariosOcasionais { get; set; }

        public enum tipo
        {
            Fixo=1, Carga=2
        };
    }
}