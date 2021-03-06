using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GI.ControlePonto.Domain.Entities
{
    [Table("pontopares")]
    public class PontoPares
    {
        public int IDPontoPares { get; set; }

        public int IDPonto { get; set; }

        public int IDEmpresa { get; set; }

        public long IDFuncionario { get; set; }

        public DateTime? DataHoraEntrada { get; set; }

        public DateTime? DataHoraSaida { get; set; }

        public int? IDEntradaOri { get; set; }

        public int? IDSaidaOri { get; set; }

        public string AbrMotivoAbonoEnt { get; set; }

        public string AbrMotivoAbonoSai { get; set; }

        public int Ordem { get; set; }

        [ForeignKey("IDPonto")]
        public virtual Ponto Ponto { get; set; }

        [ForeignKey("IDEntradaOri")]
        public virtual Marcacoes EntradaOriginal { get; set; }

        [ForeignKey("IDSaidaOri")]
        public virtual Marcacoes SaidaOriginal { get; set; }
    }
}