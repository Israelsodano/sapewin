using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeTelas
    {      
        public virtual int IDUsuario { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual String IDFuncaoTela { get; set; }

        public virtual Companies Empresa { get; set; }

        public virtual FuncoesdeTelas FuncaodeTela { get; set; }
    }
}