using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GI.ControlePonto.Domain.Repository.Interfaces
{
    public interface IRepository
    {
        IUnitOfWork GetUnit();
    }

    public interface IRepository<T> : IRepository, IDisposable where T : class
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        T Insert(T entity);

        IEnumerable<T> Insert(IEnumerable<T> entity);

        Task<T> InsertAsync(T entity);

        Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entity);

        T Update(T entity);

        Task<T> UpdateAsync(T entity);

        IEnumerable<T> Update(IEnumerable<T> entity);

        Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entity);

        void Delete(T entity);

        void Delete(IEnumerable<T> entity);

        Task DeleteAsync(T entity);

        Task DeleteAsync(IEnumerable<T> entity);

        void DeleteAll(Expression<Func<T, bool>> predicate);

        Task DeleteAllAsync(Expression<Func<T, bool>> predicate);

        IRepository<T> Clone();
    }
}