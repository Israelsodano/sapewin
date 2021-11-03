using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Function : Entity
    {               
        public string Name { get; set; }
        public IEnumerable<ScreenFunction> ViewFunctions { get; set; }
    }
}