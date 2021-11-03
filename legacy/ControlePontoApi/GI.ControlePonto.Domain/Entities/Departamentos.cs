using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class Departamentos
    {
       
        public virtual long IDDepartamento { get; set; }
                
        public virtual int IDEmpresa { get; set; }
                
        public virtual String Nome { get; set; }

        public virtual IList<Funcionarios> Funcionarios { get; set; }
                
        public virtual Empresas Empresa { get; set; }

        public virtual IList<PermissoesdeDepartamentos> PermissoesdeDepartamentos { get; set; }
    }
}