using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Departments : Entity
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Employees> Employees { get; set; }
        public Companies Company { get; set; }
        public IEnumerable<PermissoesdeDepartamentos> DepartmentPermission { get; set; }
    }
}