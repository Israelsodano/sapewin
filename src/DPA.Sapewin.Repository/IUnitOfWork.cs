using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace DPA.Sapewin.Repository
{
    public interface IUnitOfWork<TEntity> where TEntity : Entity
    {
        IRepository<TEntity> Repository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}
