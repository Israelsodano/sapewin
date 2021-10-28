using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class MotivosdeAbono
    {       
        public virtual int IDEmpresa { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Abreviacao { get; set; }

        public virtual String EventDia { get; set; }

        public virtual String EventHora { get; set; }

        public virtual String Tipo { get; set; }

        public virtual bool Favorito { get; set; }

        public virtual Companies Empresa { get; set; }
    }
}