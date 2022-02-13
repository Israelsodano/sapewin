using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models.Enums;
using DPA.Sapewin.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IClearTablesService
    {
        Task Clear(DateTime startDate, DateTime endDate,
            IEnumerable<Employee> employees, ProcessingTypes processingType);
    }

    public class ClearTablesService : IClearTablesService
    {
        private readonly IUnitOfWork<EletronicPoint> _unitOfWorkEletronicPoint;
        private readonly IUnitOfWork<Appointment> _unitOfWorkAppointments;
        private readonly IUnitOfWork<EletronicPointPairs> _unitOfWorkEletronicPointPairs;
        private readonly ILogger _logger;
        public ClearTablesService(
            IUnitOfWork<EletronicPoint> unitOfWorkEletronicPoint,
            IUnitOfWork<Appointment> unitOfWorkAppointments,
            IUnitOfWork<EletronicPointPairs> unitOfWorkEletronicPointPairs,
            ILogger<ClearTablesService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWorkEletronicPoint = unitOfWorkEletronicPoint ?? throw new ArgumentNullException(nameof(unitOfWorkEletronicPoint));
            _unitOfWorkAppointments = unitOfWorkAppointments ?? throw new ArgumentNullException(nameof(unitOfWorkAppointments));
            _unitOfWorkEletronicPointPairs = unitOfWorkEletronicPointPairs ?? throw new ArgumentNullException(nameof(unitOfWorkEletronicPointPairs));
        }

        public async Task Clear(DateTime startDate, DateTime endDate,
            IEnumerable<Employee> employees, ProcessingTypes processingType)
        {
            _logger.LogInformation("Getting normal eletronic points");
            var normalEletronicPointsList = GetNormalEletronicPoints(startDate, endDate, processingType, employees);

            var eletronicPointPairs = normalEletronicPointsList.SelectMany(x => x.Pairs).ToArray();
            var appointments = eletronicPointPairs.Select(x => new { x.OriginalEntry, x.OriginalWayOut }).ToArray();

            _unitOfWorkEletronicPointPairs.Repository.Delete(x => eletronicPointPairs.Any(y => y.Id == x.Id));
            await _unitOfWorkEletronicPointPairs.SaveChangesAsync();
            _logger.LogInformation("Deleted eletronic point pairs");

            _unitOfWorkEletronicPoint.Repository.Delete(x => normalEletronicPointsList.Any(y => y.Id == x.Id));
            await _unitOfWorkEletronicPoint.SaveChangesAsync();
            _logger.LogInformation("Deleted eletronic points");

            if (processingType == ProcessingTypes.Recalculate)
            {
                _unitOfWorkAppointments.Repository.Delete(x =>
                    appointments.Any(y => y.OriginalEntry.Id == x.Id || y.OriginalWayOut.Id == x.Id));
                await _unitOfWorkAppointments.SaveChangesAsync();
                _logger.LogInformation("Deleted appointments");
            }
        }
        private Expression<Func<EletronicPoint, bool>> ChooseExpression(DateTime startDate, DateTime endDate,
            ProcessingTypes processingType, IEnumerable<Employee> employees)
        => processingType switch
        {
            ProcessingTypes.Normal => (x => (startDate <= x.Date && x.Date <= endDate)
                    && (employees.Any(y => y.Belongs(x)))
                    && !x.Trated),

            ProcessingTypes.Recalculate => (x => (startDate <= x.Date && x.Date <= endDate)
                    && (employees.Any(y => y.Belongs(x)))),

            ProcessingTypes.Reanalyze => (x => (startDate <= x.Date && x.Date <= endDate)
                    && (employees.Any(y => y.Belongs(x)))),

            _ => throw new NotImplementedException()
        };

        private IEnumerable<EletronicPoint> GetNormalEletronicPoints(DateTime startDate, DateTime endDate,
            ProcessingTypes processingType, IEnumerable<Employee> employees)
        => _unitOfWorkEletronicPoint.Repository.GetAll(
                ChooseExpression(startDate, endDate, processingType, employees))
                .Include(x => x.Pairs)
                .Include("Pairs.OriginalInput")
                .Include("Pairs.OriginalOutput").ToArray();
    }
}