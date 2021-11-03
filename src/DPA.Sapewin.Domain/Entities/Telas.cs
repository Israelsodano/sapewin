using System;
using System.Collections.Generic;

namespace DPA.Sapewin.Domain.Entities
{
    public class Screen
    {      
        public int IDTela { get; set; }

        public string Nome { get; set; }

        public IList<ScreenFunction> FuncoesdeTelas { get; set; }
    }
}