using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class InformacoesPonto
    {
        public Funcionarios Funcionario { get; set; }

        public List<Marcacoes> Marcacoes { get; set; }

        public Horarios Horario { get; set; }

        public Ponto Ponto { get; set; }
    }
}