using System.Collections.Generic;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.Entities;

namespace GI.ControlePonto.Business
{
    public class CalculosHelperPonto
    {
        public Horarios Horario { get; set; }

        public Funcionarios Funcionario { get; set; }

        public Task<IList<HelperPares>> PontoPares { get; set; }

        public Ponto Ponto { get; set; }

        public int AtrasosPer1 { get; set; }

        public int AtrasosPer2 { get; set; }

        public PontoHelperPares PontoHelperPares { get; set; }
    }
}