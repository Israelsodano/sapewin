using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities.CommonModels
{
   public class Processamento
   {
       private Task _processamento;
       private int _percent;

       public Processamento(Task processamento)
       {
           _processamento = processamento;
       }

       public Processamento()
       {
           
       }
       public DateTime Dataini { get; set; }

       public DateTime Datafim { get; set; }

       public IList<Funcionarios> Funcionarios { get; set; }

       public TiposProcessamento TipoProcessamento { get; set; }

       public bool ProcessamentoisRunning() =>
            _processamento.Status == TaskStatus.Running;

       public void SetProcessamentoTask(Task processamento) =>
            _processamento = processamento;

       public void SetPercent(int percent) =>
            _percent = percent;

       public int GetPercent() =>
            _percent;

        public async Task Await() =>
            await _processamento;
   }
}