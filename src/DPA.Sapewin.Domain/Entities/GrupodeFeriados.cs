using System;
using System.Collections.Generic;

namespace DPA.Sapewin.Domain.Entities
{
    public class GrupodeFeriados
    {
        public virtual int IDFeriado { get; set; }

        public virtual String Descricao { get; set; }

        public virtual IList<SpecificHolidays> FeriadosEspecificos { get; set; }

        public String ListaFeriados;
    }
}