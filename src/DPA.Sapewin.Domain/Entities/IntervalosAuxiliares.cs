using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class IntervalosAuxiliares
    {
        public virtual int IDIntervalo { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual int IDHorario { get; set; }

        public virtual String Inicio { get; set; }

        public virtual String Fim { get; set; }

        public virtual String Carga { get; set; }

        public virtual tipo Tipo { get; set; }

        public virtual Companies Empresa { get; set; }

        public virtual Horarios Horarios { get; set; }

        public virtual int Order { get; set; }

        public virtual bool DescontarIntervalo { get; set; }

        public enum tipo
        {
            Fixo=1, Carga=2
        }

    }
}