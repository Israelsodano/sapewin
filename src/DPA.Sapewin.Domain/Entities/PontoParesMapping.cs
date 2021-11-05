using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPA.Sapewin.Domain.Entities
{
    [Table("pontopares")]
    public class PontoPares
    {
        public int IDPontoPares { get; set; }

        public int IDPonto { get; set; }

        public Guid CompanyId { get; set; }

        public Guid EmployeeId { get; set; }

        public DateTime? DataHoraEntrada { get; set; }

        public DateTime? DataHoraSaida { get; set; }

        public int? IDEntradaOri { get; set; }

        public int? IDSaidaOri { get; set; }

        public string AbrMotivoAbonoEnt { get; set; }

        public string AbrMotivoAbonoSai { get; set; }

        public int Ordem { get; set; }

        [ForeignKey("IDPonto")]
        public Ponto Ponto { get; set; }

        [ForeignKey("IDEntradaOri")]
        public Appointment EntradaOriginal { get; set; }

        [ForeignKey("IDSaidaOri")]
        public Appointment SaidaOriginal { get; set; }
    }
}