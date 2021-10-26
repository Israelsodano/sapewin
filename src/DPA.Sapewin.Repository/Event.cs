using System;
namespace DPA.Sapewin.Repository
{
    public abstract class Event
    {
        public Guid AggregationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public abstract class Event<TEntity> : Event where TEntity : Entity
    {
        
    }
}