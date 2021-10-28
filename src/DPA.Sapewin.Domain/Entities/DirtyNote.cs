using System;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class DirtyNote : Entity
    {
        public string UniqueMarkingKey { get; set; }
        public int Nsr { get; set; }
        public string Pis { get; set; }
        public DateTime Date { get; set; }
        public string Hour { get; set; }
        public string Nfr { get; set; }
    }
}