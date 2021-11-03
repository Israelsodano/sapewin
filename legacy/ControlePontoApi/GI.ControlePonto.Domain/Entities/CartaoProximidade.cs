using System;

namespace GI.ControlePonto.Domain.Entities
{
    public class CartaoProximidade
    {
        public virtual int IDCartao { get; set; }

        public virtual long IDFuncionario { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual DateTime DataInicial { get; set; }

        public virtual DateTime? DataFinal { get; set; }

        public virtual String NumerodoCartao { get; set; }

        public virtual Funcionarios Funcionario { get; set; }
    }
}