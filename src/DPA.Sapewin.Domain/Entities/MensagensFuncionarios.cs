using System;

namespace GI.ControlePonto.Domain.Entities
{
    public class MensagensFuncionarios
    {
        public virtual int IDMensagem { get; set; }

        public virtual long IDFuncionario { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual DateTime DataInicial { get; set; }

        public virtual DateTime? DataFinal { get; set; }

        public virtual Mensagem Mensagem { get; set; }

        public virtual Funcionarios Funcionario { get; set; }

        public virtual Empresas Empresa { get; set; }
    }
}