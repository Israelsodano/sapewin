using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class Funcionarios
    {        
        public virtual long IDFuncionario { get; set; }
        
        public virtual int IDCargo { get; set; }
        
        public virtual String Cpf { get; set; }
        
        public virtual String Endereco { get; set; }

        public virtual String Cidade { get; set; }

        public virtual long? IDDepartamento { get; set; }
        
        public virtual long IDSetor { get; set; }

        public virtual int IDParametro { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual int? IDFeriado { get; set; }

        public virtual String Nome { get; set; }
        
        public virtual String Pis { get; set; }

        public virtual DateTime Admissao { get; set; }

        public virtual DateTime? Rescisao { get; set; }

        public virtual Cargos Cargo { get; set; }

        public virtual IList<PermissoesdeFuncionarios> PermissoesdeFuncionarios { get; set; }
        
        public virtual Setores Setor { get; set; }

        public virtual Parametros Parametro { get; set; }

        public virtual Departamentos Departamento { get; set; }
        
        public virtual Empresas Empresa { get; set; }

        public virtual GrupodeFeriados GrupodeFeriados { get; set; }

        public virtual bool IntervaloFixo { get; set; }

        public virtual feriado Feriado { get; set; }

        public virtual intervalo Intervalo { get; set; }

        public virtual String IDFolha { get; set; }

        public virtual int? CTPSNum { get; set; }

        public virtual int? Serie { get; set; }

        public virtual String Telefone { get; set; }

        public virtual String RG { get; set; }

        public virtual String Salario { get; set; }

        public virtual String Observacoes { get; set; }

        public virtual bool FotoPadrao { get; set; }

        public virtual int IDEscala { get; set; }

        public enum feriado
        {
            Folga = 1, Trabalha = 2
        }

        public enum intervalo
        {
            Manual = 1, Pre_Assinalado = 2
        }

        public virtual IList<MensagensFuncionarios> MensagensCartao { get; set; }

        public virtual IList<Afastamentos> Afastamentos { get; set; }

        public virtual IList<Folgas> Folgas { get; set; }

        public virtual IList<HorariosOcasionais> HorariosOcasionais { get; set; }

        public virtual IList<CartaoProximidade> CartoesProximidade { get; set; }

        public virtual Escalas Escala { get; set; }
    }
}