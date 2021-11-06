using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IClearTablesService 
    {
        /*TiposProcessamento não sei da onde vem*/
        Task Clear(DateTime initalDate, DateTime finalDate, IList<Employee> employeess);
    }

    public class ClearTablesService : IClearTablesService
    {
        IUnityOfWork<Ponto> _unitOfWorkPonto;
        IUnityOfWork<Markup> _unitOfWorkMarkups;
        IUnityOfWork<PontoPares> _unitOfWorkPontoPair;

        public ClearTablesService(
  
            IUnityOfWork<Ponto> unitOfWorkPonto,
            IUnityOfWork<Markup> unitOfWorkMarkups,
            IUnityOfWork<PontoPares> unitOfWorkPontoPair)
        {
            _unitOfWorkPonto = unitOfWorkPonto ?? throw new ArgumentNullException(nameof(unitOfWorkPonto));
            _unitOfWorkMarkups = unitOfWorkMarkups ?? throw new ArgumentNullException(nameof(unitOfWorkMarkups));
            _unitOfWorkPontoPair = unitOfWorkPontoPair ?? throw new ArgumentNullException(nameof(unitOfWorkPontoPair));
        }

        public async Task Clear(DateTime initialDate,
            DateTime finalDate,
            IList<Employee> employees,
            TiposProcessamento processingType)
        {
            IRepository<Ponto> repPonto = _repositoryPonto.Clone();
            IRepository<Markup> repMarkups = _repositoryMarkups.Clone();
            IRepository<PontoPares> repPontoPares = _repositoryPontoPairs.Clone();

            Expression<Func<Ponto, bool>> expression = null;
            bool deletemarkups = false;

            Task ClearPontos = new Task(() =>
            {
                IList<Ponto> normalPontosList = repPonto.GetAll().Include(x => x.Pares)
                        .Include("Pares.EntradaOriginal")
                        .Include("Pares.SaidaOriginal")
                        .Where(expression).ToArray();

                List<Markup> markups = new List<Markup>();
                List<PontoPares> pontopares = new List<PontoPares>();
                for (int i = 0; i < normalPontosList.Count; i++)
                {
                    if (normalPontosList[i].Pares.Count() > 0)
                    {
                        pontopares.AddRange(normalPontosList[i].Pares);
                        markups.AddRange(normalPontosList[i].Pares.Select(x => x.EntradaOriginal));
                        markups.AddRange(normalPontosList[i].Pares.Select(x => x.SaidaOriginal));
                    }

                    normalPontosList[i].Pares = null;
                }

                try
                {
                    {
                        _unitOfWorkPontoPair.BeginTransactionAsync().Wait();
                        repPontoPares.Delete(pontopares);
                        _unitOfWorkPontoPair.SaveChangesAsync().Wait();
                    }

                    {
                        _unitOfWorkPonto.BeginTransactionAsync().Wait();
                        repPonto.Delete(normalPontosList);
                        _unitOfWorkPonto.SaveChangesAsync().Wait();
                    }

                    {

                        if (deletemarkups)
                        {
                            _unitOfWorkMarkups.BeginTransactionAsync().Wait();
                            markups.RemoveAll(x => x == null);
                            repMarkups.Delete(markups);
                            _unitOfWorkMarkups.SaveChangesAsync().Wait();
                        }
                    }
                }
                catch (System.Exception ex)
                {

                    _unitOfWorkPonto.


                    _unitOfWorkPontoPair.RollbackTransactionAsync().Wait();


                    _unitOfWorkMarkups.RollbackTransactionAsync().Wait();


                    throw ex;
                }
            });

            switch (tipoProcessamento)
            {
                case TiposProcessamento.Normal:

                    expression = (x => (initialDate <= x.Data && x.Data <= finalDate)
                    && (employees.Any(y => y.Id == x.EmployeeId && y.CompanyId == x.CompanyId))
                    && !x.Tratado);
                    break;
                case TiposProcessamento.Recalcular:

                    expression = (x => (initialDate <= x.Data && x.Data <= finalDate)
                    && (employees.Any(y => y.Id == x.EmployeeId && y.CompanyId == x.CompanyId)));
                    break;
                case TiposProcessamento.Reanalizar:
                    expression = (x => (initialDate <= x.Data && x.Data <= finalDate)
                    && (employees.Any(y => y.Id == x.EmployeeId && y.CompanyId == x.CompanyId)));

                    deletemarkups = true;
                    break;
            }

            ClearPontos.Start();
            using(ClearPontos)
                await ClearPontos;
        }
    }
}