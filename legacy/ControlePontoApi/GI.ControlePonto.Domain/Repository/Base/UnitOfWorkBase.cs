using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.Constants.Repository;
using GI.ControlePonto.Domain.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace GI.ControlePonto.Domain.Repository.Base
{
   public abstract class UnitOfWorkBase<TContext, TUnit> : IUnitOfWork<TContext> where TContext : 
                                                            ContextBase<TContext> where TUnit : 
                                                            UnitOfWorkBase<TContext, TUnit>
    {
        private TContext _context;
        private IDbContextTransaction _trasaction;

        public UnitOfWorkBase(TContext context){
            _context = context;
        }

        public void Commit(){     
            try 
            {
                _context.SaveChanges();
            }
            catch (System.Exception) 
            { 
                //TODO: LogError
                throw new System.Exception(RepositoryErrors.CommitError); 
            }
        }
       
        public async Task CommitAsync(){     
            try 
            {
                await _context.SaveChangesAsync();
            }
            catch (System.Exception) 
            { 
                //TODO: LogError
                throw new System.Exception(RepositoryErrors.CommitError); 
            }
        }
        public async Task BeginTransaction(){     
            try 
            {
               _trasaction = await _context.Database.BeginTransactionAsync();
            }
            catch (System.Exception) 
            { 
                //TODO: LogError
                throw new System.Exception(RepositoryErrors.BeginTransactionError); 
            }
        }

        public void RollbackTransaction(){
            try 
            {
                if(_trasaction == null){
                    //TODO: LogError
                    throw new System.Exception(RepositoryErrors.InitTransaction);
                }
                _trasaction.Rollback();
            }
            catch (System.Exception) 
            {
                //TODO: LogError
                throw new System.Exception(RepositoryErrors.CommitError); 
            }
        }

         public async Task RollbackTransactionAsync(){
            try 
            {
                if(_trasaction == null){
                    //TODO: LogError
                    throw new System.Exception(RepositoryErrors.InitTransaction);
                }

                await Task.Run(() => {
                    _trasaction.Rollback();
                });
            }
            catch (System.Exception) 
            {
                //TODO: LogError
                throw new System.Exception(RepositoryErrors.CommitError); 
            }
        }

        public void CommitTransaction(){
            try 
            {
                if(_trasaction == null){
                    //TODO: LogError
                    throw new System.Exception(RepositoryErrors.InitTransaction);
                }
                _trasaction.Commit();
            }
            catch (System.Exception) 
            {
                //TODO: LogError
                throw new System.Exception(RepositoryErrors.CommitError); 
            }
        }

        public async Task CommitTransactionAsync(){
            try
            {
                if(_trasaction == null){
                    //TODO: LogError
                    throw new System.Exception(RepositoryErrors.InitTransaction);
                }

                await Task.Run(() => {
                    _trasaction.Commit();
                });
            }
            catch (System.Exception)
            {
                //TODO: LogError
                throw;
            }
        }

        public DbSet<T> GetDbset<T>() where T : class{
            try
            {
                return _context.Set<T>();
            }
            catch (System.Exception)
            {
                //TODO: LogError
                throw;
            }
        }
        
        public void SetContext(TContext context){
            _context = context;
        }

        public object Clone()
        {
            try
            {
                return Activator.CreateInstance(typeof(TUnit), _context.Clone());
            }
            catch (System.Exception)
            {
                //TODO: LogError
                throw;
            }
        }

        public void Dispose()
        {
            _trasaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public DbContext GetDbContext()
        {
            return _context;
        }
    }
}