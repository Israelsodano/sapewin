using GI.ControlePonto.Domain.Business.Base;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Repository.Interfaces;

namespace GI.ControlePonto.Business
{
    public class EmpresaBusiness : BusinessBase<Empresas>
    {
        public EmpresaBusiness(IRepository<Empresas> repository) : base(repository)
        {
            
        }
    }
}