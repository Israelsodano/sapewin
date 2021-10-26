using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DPA.Sapewin.Repository.Internals.Event;

namespace DPA.Sapewin.Repository
{
    public interface IEventStore
    {
        Task AppendEvents(IEnumerable<Event> events, Guid correlationId = default);
        IQueryable<HeadEvent> GetEvents(Expression<Func<HeadEvent, bool>> predicate);
    }
}