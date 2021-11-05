using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeEmpresas
    {       
        public int IDUsuario { get; set; }

        public Guid CompanyId { get; set; }

        public Company Empresa { get; set; }
    }
}