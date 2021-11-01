using System;
using System.Collections.Generic;

namespace DPA.Sapewin.Domain.Entities
{
    public class Setores
    {      
        public long IDSetor { get; set; }

        public Guid CompanyId { get; set; }

        public string Nome { get; set; }

        public IList<Employee> Funcionarios { get; set; }

        public IList<PermissoesdeSetores> PermissoesdeSetores { get; set; }

        public Companie Empresa { get; set; }

    }
}