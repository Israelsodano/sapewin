using System;
using System.ComponentModel.DataAnnotations.Schema;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class ProcessQueueMapping : Entity
    {
        public Guid UserId { get; set; }
        public int LinesAmount { get; set; }
        public int ProcessedLines { get; set; }
        public int ErrorLines { get; set; }
        public bool Finished { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
