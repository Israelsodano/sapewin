using System;
using System.Collections.Generic;

namespace DPA.Sapewin.Domain.Entities
{
    public class Setores
    {      
        public virtual long IDSetor { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual String Nome { get; set; }

        public virtual IList<Employees> Funcionarios { get; set; }

        public virtual IList<PermissoesdeSetores> PermissoesdeSetores { get; set; }

        public virtual Companies Empresa { get; set; }

    }
}