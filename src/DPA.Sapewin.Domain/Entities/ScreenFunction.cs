using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class ScreenFunction : Entity
    {    
        public Guid FunctionId { get; set; }
       
        public Guid ScreenId { get; set; }

        public Function Function { get; set; }

        public Screen Screen { get; set; }
        
        public IList<ScreenPermissions> ScreenPermissions { get; set; }
    }
}