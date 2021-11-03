using GI.ControlePonto.Domain.Business.Base;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Repository.Interfaces;

namespace GI.ControlePonto.Business
{
    public class FuncionarioBusiness : BusinessBase<Funcionarios>
    {
        public FuncionarioBusiness(IRepository<Funcionarios> repository) : base(repository)
        {

        }
    }
}