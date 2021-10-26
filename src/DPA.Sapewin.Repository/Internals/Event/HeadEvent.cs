using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace DPA.Sapewin.Repository.Internals.Event
{
    public class HeadEvent
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AggregationId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Field> Fields { get; set; }

        public static implicit operator HeadEvent(Repository.Event e) =>
            e is null ? null : new() 
            {
                AggregationId = e.AggregationId,
                Name = e.GetType().Name,
                Fields = Field.BuildFields(e)
            };
    }
}