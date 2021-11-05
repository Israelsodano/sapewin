using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class ScreenPermissions
    {      
        public int IDUsuario { get; set; }

        public Guid CompanyId { get; set; }

        public string IDFuncaoTela { get; set; }

        public Company Empresa { get; set; }

        public ScreenFunction FuncaodeTela { get; set; }
    }
}