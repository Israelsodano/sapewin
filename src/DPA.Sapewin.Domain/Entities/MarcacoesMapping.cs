using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPA.Sapewin.Domain.Entities
{
    [Table("marcacoes")]
    public class Marcacoes
    {
        public int? IDMarcacao { get; set; }

        public string nfr { get; set; }

        public int nsr { get; set; }

        public string chaveUniqueMarc { get; set; }

        public long IDFuncionario { get; set; }

        public Guid CompanyId { get; set; }

        public DateTime datahora { get; set; }

        public static implicit operator Marcacoes(DirtyNote obj){
            DateTime datahora = Convert.ToDateTime(obj.Date.ToShortDateString() + " " + obj.Hour);

            return obj == null ? null : new Marcacoes
            {
                nfr = obj.Nfr,
                nsr = obj.Nsr,
                chaveUniqueMarc = obj.UniqueMarkingKey,
                datahora = datahora,
            };
        }
    }
}