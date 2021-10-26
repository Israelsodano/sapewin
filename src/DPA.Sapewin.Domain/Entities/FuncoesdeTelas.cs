using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class FuncoesdeTelas
    {
        
        public virtual String IDFuncaoTela { get; set; }
        
        public virtual int IDFuncao { get; set; }
       
        public virtual int IDTela { get; set; }

        public virtual IList<PermissoesdeTelas> PermissoesdeTelas { get; set; }

        public virtual Funcoes Funcao { get; set; }

        public virtual Telas Tela { get; set; }
    }
}