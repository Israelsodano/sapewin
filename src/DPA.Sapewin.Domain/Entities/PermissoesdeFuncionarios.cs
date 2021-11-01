using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeFuncionarios
    {       
        public int IDUsuario { get; set; }

        public long IDFuncionario { get; set; }

        public Guid CompanyId { get; set; }

        public Employee Funcionario { get; set; }

        public Companie Empresa { get; set; }
    }
}