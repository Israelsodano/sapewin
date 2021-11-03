using GI.ControlePonto.Domain.Business.Base;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Repository.Interfaces;

namespace GI.ControlePonto.Business
{
    public class SetorBusiness : BusinessBase<Setores>
    {
        public SetorBusiness(IRepository<Setores> repository) : base(repository)
        {
            
        }
    }
}