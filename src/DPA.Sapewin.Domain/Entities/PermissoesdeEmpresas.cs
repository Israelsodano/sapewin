namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeEmpresas
    {       
        public virtual int IDUsuario { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual Companies Empresa { get; set; }
    }
}