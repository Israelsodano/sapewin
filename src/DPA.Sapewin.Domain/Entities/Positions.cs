using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Positions : Entity
    {
        public string Name { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
    }
}