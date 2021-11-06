using DPA.Sapewin.Repository;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPA.Sapewin.Domain.Entities
{
    public class EletronicPointPairs : Entity
    {
        public Guid EletronicPointId { get; set; }

        public Guid CompanyId { get; set; }

        public Guid EmployeeId { get; set; }

        public DateTime? DataHoraEntrada { get; set; }

        public DateTime? DataHoraSaida { get; set; }

        public int? IDEntradaOri { get; set; }

        public int? IDSaidaOri { get; set; }

        public string AbrMotivoAbonoEnt { get; set; }

        public string AbrMotivoAbonoSai { get; set; }

        public int Ordem { get; set; }

        
        public EletronicPoint EletronicPoint { get; set; }

        
        public Markup OriginalInput { get; set; }

  
        public Markup OriginalOutput { get; set; }
    }
}