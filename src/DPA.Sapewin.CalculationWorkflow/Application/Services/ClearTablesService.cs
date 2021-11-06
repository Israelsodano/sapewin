using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models.Enums;
using DPA.Sapewin.Repository;
using Microsoft.EntityFrameworkCore;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IClearTablesService 
    {
        Task Clear(DateTime initalDate, DateTime finalDate, IList<Employee> employeess, ProcessingTypes processingType);
    }

    public class ClearTablesService : IClearTablesService
    {
        IUnitOfWork<EletronicPoint> _unitOfWorkEletronicPoint;
        IUnitOfWork<Markup> _unitOfWorkMarkups;
        IUnitOfWork<EletronicPointPairs> _unitOfWorkEletronicPointPairs;

        public ClearTablesService(
  
            IUnitOfWork<EletronicPoint> unitOfWorkEletronicPoint,
            IUnitOfWork<Markup> unitOfWorkMarkups,
            IUnitOfWork<EletronicPointPairs> unitOfWorkEletronicPointPairs)
        {
            _unitOfWorkEletronicPoint = unitOfWorkEletronicPoint ?? throw new ArgumentNullException(nameof(unitOfWorkEletronicPoint));
            _unitOfWorkMarkups = unitOfWorkMarkups ?? throw new ArgumentNullException(nameof(unitOfWorkMarkups));
            _unitOfWorkEletronicPointPairs = unitOfWorkEletronicPointPairs ?? throw new ArgumentNullException(nameof(unitOfWorkEletronicPointPairs));
        }

        public async Task Clear(DateTime initialDate,
            DateTime finalDate,
            IList<Employee> employees,
            ProcessingTypes processingType)
        {
            Expression<Func<EletronicPoint, bool>> expression = null;
            var deletemarkups = false;

            var clearEletronicPoints = new Task(() =>
            {
                var normalEletronicPointsList = _unitOfWorkEletronicPoint.Repository.GetAll(expression)
                    .Include(x => x.Pairs)
                    .Include("Pares.EntradaOriginal")
                    .Include("Pares.SaidaOriginal")
                    .ToArray();
               

                var markups = new List<Markup>();
                var eletronicPointPairs = new List<EletronicPointPairs>();

                for (int i = 0; i < normalEletronicPointsList.Length; i++)
                {
                    if (normalEletronicPointsList[i].Pairs.Any())
                    {
                        eletronicPointPairs.AddRange(normalEletronicPointsList[i].Pairs);
                        markups.AddRange(normalEletronicPointsList[i].Pairs.Select(x => x.OriginalInput));
                        markups.AddRange(normalEletronicPointsList[i].Pairs.Select(x => x.OriginalOutput));
                    }

                    normalEletronicPointsList[i].Pairs = null;
               
                    _unitOfWorkEletronicPointPairs.Repository.Delete(x => eletronicPointPairs.Any(y => y.Id == x.Id));
                    _unitOfWorkEletronicPointPairs.SaveChangesAsync();
                

                    _unitOfWorkEletronicPoint.Repository.Delete(x => normalEletronicPointsList.Any(y => y.Id == x.Id));
                    _unitOfWorkEletronicPoint.SaveChangesAsync();
                
                    if (deletemarkups)
                    {
                        markups.RemoveAll(x => x == null);
                        _unitOfWorkMarkups.Repository.Delete(x => markups.Exists(y => y.Id == x.Id));
                        _unitOfWorkMarkups.SaveChangesAsync();
                    }
                }
            });

            switch (processingType)
            {
                case ProcessingTypes.Normal:

                    expression = (x => (initialDate <= x.Data && x.Data <= finalDate)
                    && (employees.Any(y => y.Id == x.EmployeeId && y.CompanyId == x.CompanyId))
                    && !x.Tratado);
                    break;
                case ProcessingTypes.Recalculate:

                    expression = (x => (initialDate <= x.Data && x.Data <= finalDate)
                    && (employees.Any(y => y.Id == x.EmployeeId && y.CompanyId == x.CompanyId)));
                    break;
                case ProcessingTypes.Reanalyze:
                    expression = (x => (initialDate <= x.Data && x.Data <= finalDate)
                    && (employees.Any(y => y.Id == x.EmployeeId && y.CompanyId == x.CompanyId)));

                    deletemarkups = true;
                    break;
            }

            clearEletronicPoints.Start();
            using(clearEletronicPoints)
                await clearEletronicPoints;
        }
    }
}