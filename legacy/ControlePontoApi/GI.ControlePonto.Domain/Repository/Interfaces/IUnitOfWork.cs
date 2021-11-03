using System.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GI.ControlePonto.Domain.Repository.Interfaces
{
    public interface IUnitOfWork : ICloneable, IDisposable
    {
        void Commit();

        Task CommitAsync();

        Task BeginTransaction();

        Task RollbackTransactionAsync();

        void RollbackTransaction();

        void CommitTransaction();

        Task CommitTransactionAsync();

        DbContext GetDbContext();

        DbSet<T> GetDbset<T>() where T : class;
    }
    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : ContextBase<TContext>
    {
        
    }
}