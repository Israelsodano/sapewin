using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.Repository.Base;
using GI.ControlePonto.Domain.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace GI.ControlePonto.Repository
{
    public class Repository<T, R> : IRepository<T> where T : class where R : ContextBase<R>
    {
        private readonly IUnitOfWork<R> _unitOfWork;
        private readonly DbSet<T> _dbSet;

        public Repository(IUnitOfWork<R> unitOfWork){
            _unitOfWork = unitOfWork;
            _dbSet = _unitOfWork.GetDbset<T>();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _unitOfWork.Commit();
        }

        public void Delete(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
            _unitOfWork.Commit();
        }

        public void DeleteAll(Expression<Func<T, bool>> predicate)
        {
            _dbSet.RemoveRange(_dbSet.Where(predicate));
            _unitOfWork.Commit();
        }

        public async Task DeleteAllAsync(Expression<Func<T, bool>> predicate)
        {
            await Task.Run(() => {
                _dbSet.RemoveRange(_dbSet.Where(predicate));
                _unitOfWork.Commit();
            });
        }

        public async Task DeleteAsync(T entity)
        {
            await Task.Run(() => {
                _dbSet.Remove(entity);
                _unitOfWork.Commit();
            });   
        }

        public async Task DeleteAsync(IEnumerable<T> entity)
        {
            await Task.Run(() => {
                _dbSet.RemoveRange(entity);
                _unitOfWork.Commit();
            });
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsNoTracking();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking();
        }

        public IUnitOfWork GetUnit()
        {
            return _unitOfWork;
        }

        public T Insert(T entity)
        {
            _dbSet.Add(entity);
            _unitOfWork.Commit();
            return entity;
        }

        public IEnumerable<T> Insert(IEnumerable<T> entity)
        {
            _dbSet.AddRange(entity);
            _unitOfWork.Commit();
            return entity;
        }

        public async Task<T> InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entity)
        {
            await _dbSet.AddRangeAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        public T Update(T entity)
        {
            _dbSet.Update(entity);
            _unitOfWork.Commit();
            return entity;
        }

        public IEnumerable<T> Update(IEnumerable<T> entity)
        {
            _dbSet.UpdateRange(entity);
            _unitOfWork.Commit();

            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            await Task.Run(() => { 
                _dbSet.Update(entity);
                _unitOfWork.Commit();
            });

            return entity;
        }

        public async Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entity)
        {
            await Task.Run(() => {
                _dbSet.UpdateRange(entity);
                _unitOfWork.Commit();
            });

            return entity;
        }

        public IRepository<T> Clone()
        {
            return new Repository<T, R>((IUnitOfWork<R>)_unitOfWork.Clone());
        }
        
        public void Dispose()
        {
            GC.SuppressFinalize(_dbSet);
            _unitOfWork.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}