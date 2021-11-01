using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class LogSistema
    {
        public int IDLog { get; set; }

        public string IP { get; set; }

        public int IDUsuario { get; set; }

        public string Funcao { get; set; }

        public string Tela { get; set; }

        public string Descricao { get; set; }

        public DateTime DataHora { get; set; }
    }
}