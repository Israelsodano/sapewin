using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class Cargos
    {
        public virtual int IDCargo { get; set; }
        
        public virtual String Nome { get; set; }
                
        public virtual IList<Funcionarios> Funcionarios { get; set; }
    }
}