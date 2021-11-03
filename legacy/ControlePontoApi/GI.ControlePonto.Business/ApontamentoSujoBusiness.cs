using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.Business.Base;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Repository.Interfaces;

namespace GI.ControlePonto.Business
{
    public class ApontamentoSujoBusiness : BusinessBase<ApontamentoSujo>
    {
        public ApontamentoSujoBusiness(IRepository<ApontamentoSujo> repository) : base(repository)
        {

        }

        public async Task SalvaAfd(Stream stream)
        {
            var sr = new StreamReader(stream);
            string linha = null;
            string nfr = await sr.ReadLineAsync();
            nfr = nfr.Substring(187, 17);
            int linhas = 0;

            var listApontamentos = new List<ApontamentoSujo>();

            while ((linha = await sr.ReadLineAsync()) != null)
            {
                var marcacao = linha.Substring(9, 1);

                if(marcacao == "3")
                {
                    var data = linha.Substring(10, 8);
                    var hora = linha.Substring(18, 4);
                    var pis = linha.Substring(22, 12);
                    var nsr = linha.Substring(0, 9);
                    
                    int dia = Convert.ToInt32(data.Substring(0, 2));
                    int mes = Convert.ToInt32(data.Substring(2, 2));
                    int ano = Convert.ToInt32(data.Substring(4, 4));

                    listApontamentos.Add(new ApontamentoSujo 
                    {
                        data = new DateTime(ano, mes, dia),
                        hora = hora,
                        nfr = nfr,
                        nsr = Convert.ToInt32(nsr),
                        pis = pis,
                        chaveUniqueMarc = nfr + nsr.PadLeft(9, '0')
                    });
                }
                    
                linhas++;
            }

            _repository.Insert(listApontamentos);
        }
    }
}