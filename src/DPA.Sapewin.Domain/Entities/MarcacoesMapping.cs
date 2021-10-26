using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GI.ControlePonto.Domain.Entities
{
    [Table("marcacoes")]
    public class Marcacoes
    {
        public int? IDMarcacao { get; set; }

        public string nfr { get; set; }

        public int nsr { get; set; }

        public string chaveUniqueMarc { get; set; }

        public long IDFuncionario { get; set; }

        public int IDEmpresa { get; set; }

        public DateTime datahora { get; set; }

        public static implicit operator Marcacoes(ApontamentoSujo obj){
            DateTime datahora = Convert.ToDateTime(obj.data.ToShortDateString() + " " + obj.hora);

            return obj == null ? null : new Marcacoes
            {
                nfr = obj.nfr,
                nsr = obj.nsr,
                chaveUniqueMarc = obj.chaveUniqueMarc,
                datahora = datahora,
            };
        }
    }
}