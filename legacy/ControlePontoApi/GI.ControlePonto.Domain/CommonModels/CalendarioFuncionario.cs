using System;
using System.Collections.Generic;


namespace GI.ControlePonto.Domain.Entities.CommonModels
{
    public class CalendarioFuncionario
    {
        public Funcionarios Funcionario { get; set; }

        public IList<Calendario> Calendario { get; set; }
    }
}