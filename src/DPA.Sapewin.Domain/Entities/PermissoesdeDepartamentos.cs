namespace GI.ControlePonto.Domain.Entities
{
    public class PermissoesdeDepartamentos
    {
      
        public virtual int IDUsuario { get; set; }

        public virtual long IDDepartamento { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual Departamentos Departamento { get; set; }

        public virtual Empresas Empresa { get; set; }
    }
}