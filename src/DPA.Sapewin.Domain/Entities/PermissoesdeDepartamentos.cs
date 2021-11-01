using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeDepartamentos
    {
        public int IDUsuario { get; set; }

        public long IDDepartamento { get; set; }

        public Guid CompanyId { get; set; }

        public Department Departamento { get; set; }

        public Companie Empresa { get; set; }
    }
}