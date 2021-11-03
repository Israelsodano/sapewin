using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class Empresas
    {
       
        public virtual int IDEmpresa { get; set; }
           
        public virtual String Nome { get; set; }       
        
        public virtual long? IDFolha { get; set; }
       
        public virtual String Endereco { get; set; }
        
        public virtual String Cep { get; set; }
        
        public virtual String Cidade { get; set; }
             
        public virtual String Bairro { get; set; }
               
        public virtual String UF { get; set; }
                
        public virtual String Documento { get; set; }
                
        public virtual String IE { get; set; }
                
        public virtual String CEI { get; set; }

        public virtual IList<Setores> Setores { get; set; }

        public virtual IList<PermissoesdeSetores> PermissoesdeSetores { get; set; }

        public virtual IList<Departamentos> Departamentos { get; set; }

        public virtual IList<PermissoesdeDepartamentos> PermissoesdeDepartamentos { get; set; }

        public virtual IList<Funcionarios> Funcionarios { get; set; }

        public virtual IList<PermissoesdeFuncionarios> PermissoesdeFuncionarios { get; set; }

        public virtual IList<PermissoesdeEmpresas> PermissoesdeEmpresas { get; set; }

        public virtual IList<PermissoesdeTelas> PermissoesdeTelas { get; set; }

        public virtual IList<Escalas> Escalas { get; set; }

        public virtual IList<EscalasHorarios> EscalasHorarios { get; set; }

        public virtual IList<Horarios> Horarios { get; set; }

        public virtual IList<IntervalosAuxiliares> IntervalosAuxiliares { get; set; }
    }
}