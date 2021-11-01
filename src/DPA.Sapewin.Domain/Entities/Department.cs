using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Department : Entity
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public Companie Company { get; set; }
        public IEnumerable<PermissoesdeDepartamentos> DepartmentPermission { get; set; }
    }
}