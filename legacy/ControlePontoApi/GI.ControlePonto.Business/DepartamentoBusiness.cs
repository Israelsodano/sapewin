using GI.ControlePonto.Domain.Business.Base;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Repository.Interfaces;

namespace GI.ControlePonto.Business
{
    public class DepartamentoBusiness : BusinessBase<Departamentos>
    {
        public DepartamentoBusiness(IRepository<Departamentos> repository) : base(repository)
        {
            
        }
    }
}