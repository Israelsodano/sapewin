using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.CommonModels;
using GI.ControlePonto.Domain.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GI.ControlePonto.Domain.Business.Base
{
    public abstract class BusinessBase<TEntity> : IDisposable where TEntity : class
    {
        protected readonly IRepository<TEntity> _repository;
        public BusinessBase(IRepository<TEntity> repository)
        {
            _repository = repository;
        }
        
        public virtual IQueryable<TEntity> GetAll() =>
            _repository.GetAll();

        public virtual IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate) =>
            _repository.GetAll(predicate);

        public virtual TEntity Insert(TEntity entity) => 
            _repository.Insert(entity);

        public virtual IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities) =>
            _repository.Insert(entities);

        public virtual async Task<TEntity> InsertAsync(TEntity entity) =>
            await _repository.InsertAsync(entity);

        public virtual async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities) =>
             await _repository.InsertAsync(entities);

        public virtual TEntity Update(TEntity entity) =>
            _repository.Update(entity);

        public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities) =>
            _repository.Update(entities);

        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities) => 
            await _repository.UpdateAsync(entities);

        public virtual void Delete(TEntity entity) => 
            _repository.Delete(entity);

        public virtual async Task DeleteAsync(TEntity entity) => 
            await _repository.DeleteAsync(entity);

        public virtual void Delete(IEnumerable<TEntity> entities) =>
            _repository.Delete(entities);

        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities) =>
            await _repository.DeleteAsync(entities);

        public virtual void DeleteAll(Expression<Func<TEntity, bool>> predicate) => 
            _repository.DeleteAll(predicate);
        
        public virtual async Task DeleteAllAsync(Expression<Func<TEntity, bool>> predicate) =>
            await _repository.DeleteAllAsync(predicate);

        public virtual GridResult<TEntity> GetGrid(Grid grid) =>
            throw new NotImplementedException();
        
        protected virtual GridResult<TEntity> GetGridPaged(int page, int range, Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            int count = _repository.GetAll(predicate).Count();
            int pages = (int)Math.Ceiling(((decimal)count) / (decimal)range);

            page = page >= pages ? pages - 1 : page;
            int skip = page * range;
            int take = range;

            take = skip + take > count ? count - skip : take;
            skip = skip + take > count ? count - take : skip;

            if(count == 0){
                skip = 0;
                take = 0;
            } 

            var queryResult = _repository.GetAll(predicate);
            for (int i = 0; i < includes.Length; i++)
            {
                queryResult = queryResult.Include(includes[i]);
            }

            return new GridResult<TEntity> {
                count = count,
                page = page,
                pages = pages,
                list = queryResult.Skip(skip).Take(take)
            };
        }
        public void Dispose()
        {
            _repository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}