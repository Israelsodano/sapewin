using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class MotivosdeAbono
    {       
        public Guid CompanyId { get; set; }

        public string Nome { get; set; }

        public string Abreviacao { get; set; }

        public string EventDia { get; set; }

        public string EventHora { get; set; }

        public string Tipo { get; set; }

        public bool Favorito { get; set; }

        public Company Empresa { get; set; }
    }
}