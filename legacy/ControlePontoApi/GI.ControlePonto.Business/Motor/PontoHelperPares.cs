using System.Collections.Generic;
using GI.ControlePonto.Domain.Entities;

namespace GI.ControlePonto.Business
{
    public class PontoHelperPares
    {
        public readonly Ponto ponto;
        public readonly IList<HelperPares> helperPares;
        public PontoHelperPares(Ponto ponto, IList<HelperPares> helperPares)
        {
            this.ponto = ponto;
            this.helperPares = helperPares;
        }
    }
}