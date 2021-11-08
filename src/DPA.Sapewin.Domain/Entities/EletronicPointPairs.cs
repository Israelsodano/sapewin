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
        public DateTime? InputDateTime { get; set; }
        public DateTime? OutputDateTime { get; set; }
        public Guid? OriginalInputId { get; set; }
        public Guid? OriginalOutputId { get; set; }
        public string AbrMotivoAbonoEnt { get; set; }
        public string AbrMotivoAbonoSai { get; set; }
        public int Order { get; set; }
        public EletronicPoint EletronicPoint { get; set; }

        public Appointment OriginalInput { get; set; }
        public Appointment OriginalOutput { get; set; }
    }
}