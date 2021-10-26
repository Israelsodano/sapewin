using System.Collections.Generic;
using System;
namespace DPA.Sapewin.Repository
{
    public abstract class Entity
    {
        public Entity() => PendingEvents = new List<Event>();
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public IList<Event> PendingEvents { get; }
        public void Append(Event e) => PendingEvents.Add(e);
    }
}