using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DPA.Sapewin.Repository
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> InsertAsync(TEntity e, Guid correlationId = default);
        Task InsertAsync(IEnumerable<TEntity> e, Guid correlationId = default);
        TEntity Update(TEntity e, Guid correlationId = default);
        void Update(IEnumerable<TEntity> e, Guid correlationId = default);
        void Delete(Expression<Func<TEntity, bool>> predicate, Guid correlationId = default);
    }
}