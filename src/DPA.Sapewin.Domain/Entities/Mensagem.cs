using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class Mensagem
    {
        public virtual int IDMensagem { get; set; }

        public virtual String Conteudo { get; set; }

        public virtual String Nome { get; set; }

        public virtual IList<MensagensFuncionarios> MensagensFuncionarios { get; set; }
    }
}