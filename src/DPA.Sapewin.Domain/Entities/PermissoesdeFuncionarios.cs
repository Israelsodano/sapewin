using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeFuncionarios
    {       
        public int IDUsuario { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid CompanyId { get; set; }

        public Employee Funcionario { get; set; }

        public Company Empresa { get; set; }
    }
}