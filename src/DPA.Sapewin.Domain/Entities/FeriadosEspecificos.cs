using System;

namespace GI.ControlePonto.Domain.Entities
{
    public class FeriadosEspecificos
    {
        public virtual int IDFeriado { get; set; }

        public virtual int Dia { get; set; }

        public virtual int Mes { get; set; }

        public virtual int Ano { get; set; }

        public virtual String Descricao { get; set; }

        public virtual GrupodeFeriados GrupodeFeriados { get; set; }
    }
}