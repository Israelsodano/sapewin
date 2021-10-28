namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeDepartamentos
    {
      
        public virtual int IDUsuario { get; set; }

        public virtual long IDDepartamento { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual Departments Departamento { get; set; }

        public virtual Companies Empresa { get; set; }
    }
}