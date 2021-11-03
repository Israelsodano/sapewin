using GI.ControlePonto.Domain.Repository.Base;
using GI.ControlePonto.Repository.Contexts;

namespace GI.ControlePonto.Repository.Units
{
    public class SapewinUnit : UnitOfWorkBase<SapeWinContext, SapewinUnit>
    {
        public SapewinUnit(SapeWinContext context) : base(context)
        {

        }
    }
}