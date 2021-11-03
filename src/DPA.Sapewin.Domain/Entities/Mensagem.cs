using System;
using System.Collections.Generic;

namespace DPA.Sapewin.Domain.Entities
{
    public class Mensagem
    {
        public int IDMensagem { get; set; }

        public string Conteudo { get; set; }

        public string Nome { get; set; }

        public IList<MensagensFuncionarios> MensagensFuncionarios { get; set; }
    }
}