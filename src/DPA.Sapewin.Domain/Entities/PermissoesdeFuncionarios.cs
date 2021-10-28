namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeFuncionarios
    {       
        public virtual int IDUsuario { get; set; }

        public virtual long IDFuncionario { get; set; }

        public virtual int IDEmpresa { get; set; }

        public Employees Funcionario { get; set; }

        public Companies Empresa { get; set; }
    }
}