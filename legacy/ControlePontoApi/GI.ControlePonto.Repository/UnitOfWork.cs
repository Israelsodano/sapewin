using System.Linq;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.Repository.Interfaces;

namespace GI.ControlePonto.Repository
{
     public class UnitOfWork
    {
         public async static Task BeginTransactionAsync(params IRepository[] repositories){
            IUnitOfWork[] units = repositories.Select(x=>x.GetUnit()).ToArray();

            for (int i = 0; i < units.Length; i++)
            {
                await units[i].BeginTransaction();
            }
        }

        public async static Task RollbackTransactionAsync(params IRepository[] repositories){
            IUnitOfWork[] units = repositories.Select(x=>x.GetUnit()).ToArray();

            for (int i = 0; i < units.Length; i++)
            {
                await units[i].RollbackTransactionAsync();
            }
        }

        public async static Task CommitTransactionAsync(params IRepository[] repositories){
            IUnitOfWork[] units = repositories.Select(x=>x.GetUnit()).ToArray();

            for (int i = 0; i < units.Length; i++)
            {
                await units[i].CommitTransactionAsync();
            }
        }
    }
}