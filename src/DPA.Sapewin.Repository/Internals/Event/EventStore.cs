using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DPA.Sapewin.Repository.Internals.Event
{
    internal class EventStore : IEventStore
    {
        private readonly DbSet<HeadEvent> _dbset;
        private readonly ILogger _logger;

        public EventStore(DbContext context, 
                          ILogger<EventStore> logger)
        {
            if(context is null) throw new ArgumentNullException(nameof(context));

            _dbset = context.Set<HeadEvent>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));   
        }

        public Task AppendEvents(IEnumerable<Repository.Event> events, Guid correlationId = default)
        {   
            _logger.LogTrace(correlationId + " - append events: " + JsonConvert.SerializeObject(events));

            var headEvents = from e in events
                             select (HeadEvent)e;

            _logger.LogTrace(correlationId + " - events successfully parsed");

            return _dbset.AddRangeAsync(headEvents);
        }

        public IQueryable<HeadEvent> GetEvents(Expression<Func<HeadEvent, bool>> predicate) =>
            _dbset.Where(predicate)
                  .AsNoTracking();
    }
}