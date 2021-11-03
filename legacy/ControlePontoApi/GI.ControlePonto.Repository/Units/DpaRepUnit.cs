using GI.ControlePonto.Domain.Repository.Base;
using GI.ControlePonto.Repository.Contexts;

namespace GI.ControlePonto.Repository.Units
{
    public class DpaRepUnit : UnitOfWorkBase<DpaRepContext, DpaRepUnit>
    {
        public DpaRepUnit(DpaRepContext context) : base(context)
        {
            
        }
    }
}