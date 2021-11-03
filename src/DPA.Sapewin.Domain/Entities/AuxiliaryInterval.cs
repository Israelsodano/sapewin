using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class AuxiliaryInterval : Entity
    {

        public Guid CompanyId { get; set; }

        public Guid ScheduleId { get; set; }

        public int Entry { get; set; }

        public int WayOut { get; set; }

        public int Charge { get; set; }

        public int Order { get; set; }

        public AuxiliaryIntervalKind Kind { get; set; }

        public Companie Empresa { get; set; }

        public Schedule Horarios { get; set; }

        public bool DiscountInterval { get; set; }

        public enum AuxiliaryIntervalKind
        {
            Fixo=1, Carga=2
        }

    }
}