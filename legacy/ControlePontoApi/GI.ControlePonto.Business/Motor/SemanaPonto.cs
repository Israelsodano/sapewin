using System;
using System.Collections.Generic;
using System.Linq;

namespace GI.ControlePonto.Business.Motor
{
    public class SemanaPonto
    {
        public IList<DiaPonto> DiasdaSemana { get; set; } = new List<DiaPonto>();

        public DiaPonto PegaProximoDsr(DateTime data)
        {
            return DiasdaSemana.OrderByDescending(x=> (x.PontoReferencia.Data.Date - data))
                               .FirstOrDefault(x=> x.IsDsr);
        }

        public DiaPonto PegaUltimoDsr()
        {
            return DiasdaSemana.OrderBy(x=> x.calendario.Data).LastOrDefault(x=> x.IsDsr);
        }

        public void MudarTodososDsrPara(string valordsrDesc, string valordsrPag)
        {
            for (int i = 0; i < DiasdaSemana.Count; i++)
            {
                if(DiasdaSemana[i].IsDsr)
                {
                    DiasdaSemana[i].PontoReferencia.DsrDescontado = valordsrDesc;
                    DiasdaSemana[i].PontoReferencia.DsrPago = valordsrPag;
                }
            }
        }

        public void MudarTodososDsrFuturoPara(string valordsr, DateTime data)
        {
            for (int i = 0; i < DiasdaSemana.Count; i++)
            {
                if(DiasdaSemana[i].IsDsr && DiasdaSemana[i].calendario.Data >= data)
                    DiasdaSemana[i].PontoReferencia.DsrDescontado = valordsr;
            }
        }

        public DiaPonto PegaDsrAnterior(DateTime data)
        {
            return DiasdaSemana.OrderBy(x=> (x.PontoReferencia.Data.Date - data))
                               .FirstOrDefault(x=> x.IsDsr);
        }
    }
}