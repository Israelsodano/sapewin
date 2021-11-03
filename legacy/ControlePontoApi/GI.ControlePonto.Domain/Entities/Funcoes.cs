using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class Funcoes
    {               
        public virtual int IDFuncao { get; set; }
        
        public virtual String Nome { get; set; }

        public virtual IList<FuncoesdeTelas> FuncoesdeTelas { get; set; }
    }
}