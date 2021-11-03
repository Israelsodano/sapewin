using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class PermissoesdeSetores
    {       
        public int IDUsuario { get; set; }

        public long IDSetor { get; set; }

        public Guid CompanyId { get; set; }

        public Setores Setor { get; set; }

        public Companie Empresa { get; set; }
    }
}