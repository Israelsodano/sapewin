using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DPA.Sapewin.Repository.Internals.Event
{
    internal class UnitOfWork<TEntity> : IUnitOfWork<TEntity> where TEntity : Entity
    {
        private readonly DbContext _context;
        private readonly ILogger _logger;

        public UnitOfWork(DbContext context, 
                           IRepository<TEntity> repository, 
                           ILogger<UnitOfWork<TEntity>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(context));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IRepository<TEntity> Repository { get; }

        public Task<IDbContextTransaction> BeginTransactionAsync() => _context.Database.BeginTransactionAsync();

        public Task<int> SaveChangesAsync()
        {
            _logger.LogTrace("saving changes");
            var r = _context.SaveChangesAsync();
            _logger.LogTrace("changes successfully saved");
            return r;
        }
    }
}