using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GI.ControlePonto.Domain.Entities
{
    [Table("marcacoessujas")]
    public class ApontamentoSujo
    {
        public string chaveUniqueMarc { get; set; }
        
        public int nsr { get; set; }

        public string pis { get; set; }

        public DateTime data { get; set; }

        public string hora { get; set; }

        public string nfr { get; set; }
    }
}