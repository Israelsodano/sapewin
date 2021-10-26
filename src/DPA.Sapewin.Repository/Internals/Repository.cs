using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DPA.Sapewin.Repository.Internals
{
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly DbSet<TEntity> _dbset;
        private readonly ILogger _logger;
        private readonly IEventStore _eventStore;
    
        public Repository(DbContext context,
                          IEventStore eventStore,
                          ILogger<Repository<TEntity>> logger)
        {
            if(context is null) throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _dbset = context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate) => _dbset.Where(predicate).AsNoTracking();
        
        public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate) => _dbset.FirstOrDefaultAsync(predicate);

        public async Task<TEntity> InsertAsync(TEntity e, Guid correlationId = default)
        {
            await _eventStore.AppendEvents(e.PendingEvents);
            e.PendingEvents.Clear();

            _logger.LogTrace(correlationId + " - inserting entity: " + JsonConvert.SerializeObject(e));
            var et = await  _dbset.AddAsync(e);
            _logger.LogTrace(correlationId + " - entity successfully inserted");
            return et.Entity;
        }

        public async Task InsertAsync(IEnumerable<TEntity> e, Guid correlationId = default)
        {
            await _eventStore.AppendEvents(e.SelectMany(x=> x.PendingEvents));
            foreach (var et in e) et.PendingEvents.Clear();

            _logger.LogTrace(correlationId + " - inserting entities: " + JsonConvert.SerializeObject(e));
            await _dbset.AddRangeAsync(e);
            _logger.LogTrace(correlationId + " - entities successfully inserted");
        }

        public TEntity Update(TEntity e, Guid correlationId = default)
        {
            Task.WaitAll(_eventStore.AppendEvents(e.PendingEvents));
            e.PendingEvents.Clear();

            _logger.LogTrace(correlationId + " - update entity: " + JsonConvert.SerializeObject(e));
            var et = _dbset.Update(e);
            _logger.LogTrace(correlationId + " - entity successfully inserted");
            return et.Entity;
        }

        public void Update(IEnumerable<TEntity> e, Guid correlationId = default)
        {
            Task.WaitAll(_eventStore.AppendEvents(e.SelectMany(x=> x.PendingEvents)));
            foreach (var et in e) et.PendingEvents.Clear();

            _logger.LogTrace(correlationId + " - updating entities: " + JsonConvert.SerializeObject(e));
            _dbset.UpdateRange(e);
            _logger.LogTrace(correlationId + " - entities successfully updated");
        } 

        public void Delete(Expression<Func<TEntity, bool>> predicate, Guid correlationId = default)
        {
            var e = _dbset.Where(predicate).AsNoTracking().ToArray();
            Task.WaitAll(_eventStore.AppendEvents(e.SelectMany(x=> x.PendingEvents)));
            foreach (var et in e) et.PendingEvents.Clear();

            _logger.LogTrace(correlationId + " - removing entities: " + JsonConvert.SerializeObject(e));
            _dbset.RemoveRange(e);
            _logger.LogTrace(correlationId + " - entities successfully revoved");
        }
    }
}