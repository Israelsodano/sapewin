using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Functions : Entity
    {               
        public string Name { get; set; }
        public IEnumerable<FuncoesdeTelas> ViewFunctions { get; set; }
    }
}