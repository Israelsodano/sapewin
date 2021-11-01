using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class HorariosOcasionais : Entity
    {
        public Guid ScheduleIdOcasional { get; set; }

        public long IDFuncionario { get; set; }

        public Guid ScheduleId { get; set; }

        public Guid CompanyId { get; set; }

        public DateTime Data { get; set; }

        public Employee Funcionario { get; set; }

        public Schedule Horario { get; set; }
    }
}