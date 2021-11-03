using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GI.ControlePonto.Domain.Entities
{
    [Table("filaProcesso")]
    public class FilaProcesso
    {
        public int IDFilaProcesso { get; set; }
        public int IDUsuario { get; set; }
        public int qtdLinhas { get; set; }
        public int linhasProcessadas { get; set; }
        public int linhasErro { get; set; }
        public bool finalizado { get; set; }
        public DateTime dataInicio { get; set; }
        public DateTime? dataFim { get; set; }
    }
}
