using DPA.Sapewin.Repository;
using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class Markup : Entity
    {
        public string Nfr { get; set; }

        public int Nsr { get; set; }

        public string UniqueKey { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid CompanyId { get; set; }

        public DateTime DateTime { get; set; }

        public static implicit operator Markup(DirtyNote obj){
            DateTime datetime = Convert.ToDateTime(obj.Date.ToShortDateString() + " " + obj.Hour);

            return obj == null ? null : new Markup
            {
                Nfr = obj.Nfr,
                Nsr = obj.Nsr,
                UniqueKey = obj.UniqueMarkingKey,
                DateTime = datetime,
            };
        }
    }
}