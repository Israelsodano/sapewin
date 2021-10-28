using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Positions : Entity
    {
        public virtual string Name { get; set; }
        public virtual IEnumerable<Employees> Employees { get; set; }
    }
}