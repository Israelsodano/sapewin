using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPA.Sapewin.CalculationWorkflow.Application.Records;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IAbsencesService : ICalculationBase
    {
        
    }

    public class AbsencesService : CalculationBase, IAbsencesService
    {
        public AbsencesService(IAppointmentsService appointmentsService, 
                               IUnitOfWork<EletronicPoint> unitOfWorkEletronicPoint) 
                               : base(appointmentsService, 
                                      unitOfWorkEletronicPoint)
        { }

        public override async Task<IEnumerable<EletronicPoint>> Calculate(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsByEmployees)
        {
            var eletronicPoints = CalculateAbsencesByEletronicPoints(eletronicPointsByEmployees);
            await SaveEletronicPointsAsync(eletronicPoints);
            
            return eletronicPoints;
        }

        private IEnumerable<EletronicPoint> CalculateAbsencesByEletronicPoints(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsByEmployees)
        {
            foreach (var eletronicPointsByEmployee in eletronicPointsByEmployees)
            {
                var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPointsByEmployee.Key);
                yield return CalculateAbsence(eletronicPointsByEmployee.Key, 
                             OrganizePairs(eletronicPointsByEmployee.Key, eletronicPointsByEmployee, rappointments.eappointment).ToList(), rappointments);
            }
        }

        private EletronicPoint CalculateAbsence(EletronicPoint eletronicPoint, 
                                                List<EletronicPointPairs> pairs,
                                                AppointmentsRecord rappointments)
        {
            if (eletronicPoint.Schedule is null) return eletronicPoint;

            var nullPairs = from epair in pairs
                            where IsNullPair(epair)
                            orderby (epair.OriginalEntry ?? epair.OriginalWayOut).DateHour
                            select epair;
            var iiappointment = GetIntervalInAppointment(rappointments, nullPairs.SelectMany(x => x.GetAppointments()).ToList());

            eletronicPoint.FirstPeriodDiscountedAbsencesMinutes = GetFirstPeriodAbsence(pairs, nullPairs, iiappointment.DateHour, rappointments.eappointment);
            eletronicPoint.SecondPeriodDiscountedArrearsInMinutes = GetSecondPeriodAbsence(pairs, nullPairs, iiappointment.DateHour, rappointments.eappointment);

            return eletronicPoint;
        }

        private double GetFirstPeriodAbsence(List<EletronicPointPairs> pairs, IEnumerable<EletronicPointPairs> nullPairs,
            DateTime iiappointment, DateTime eappointment)
        {
            if (!pairs.Any(x => (x.OriginalEntry ?? x.OriginalWayOut).DateHour <= iiappointment))
                return (iiappointment - eappointment).TotalMinutes;


            return (from np in nullPairs
                    where (np.OriginalWayOut ?? np.OriginalEntry).DateHour <= iiappointment
                    select np != null && IsNullPair(np)
                        ? GetDiffBetweenPairs(np, NextPair(pairs, np))
                        : GetDiffBetweenPairs(np, np)).Sum();
        }
        private double GetSecondPeriodAbsence(List<EletronicPointPairs> pairs, IEnumerable<EletronicPointPairs> nullPairs,
            DateTime intervalIn, DateTime eappointment)
        {
            if (!pairs.Any(x => (x.OriginalWayOut ?? x.OriginalEntry).DateHour > intervalIn))
                return (intervalIn - eappointment).TotalMinutes;


            return (from np in nullPairs
                    where (np.OriginalWayOut ?? np.OriginalEntry).DateHour > intervalIn
                    select np != null && IsNullPair(np)
                        ? GetDiffBetweenPairs(np, NextPair(pairs, np))
                        : GetDiffBetweenPairs(np, np)).Sum();
        }
        private IEnumerable<EletronicPointPairs> OrganizePairs(EletronicPoint eletronicPoint, IEnumerable<EletronicPointPairs> pairsToOrganize, DateTime eappointment)
        => (from p in pairsToOrganize
            where (p.OriginalEntry ?? p.OriginalWayOut).DateHour >= eappointment
            orderby (p.OriginalEntry ?? p.OriginalWayOut).DateHour
            select p);

        private EletronicPointPairs NextPair(List<EletronicPointPairs> pairsToSearch, EletronicPointPairs currentPair)
        => pairsToSearch.Skip(pairsToSearch.IndexOf(currentPair)).FirstOrDefault(x => !IsNullPair(x));
        private double GetDiffBetweenPairs(EletronicPointPairs pair, EletronicPointPairs realPair)
        => realPair is null ? TimeSpan.FromTicks((pair.OriginalWayOut ?? pair.OriginalEntry).DateHour.Ticks).TotalMinutes
        : ((pair.OriginalWayOut ?? pair.OriginalEntry).DateHour - (realPair.OriginalEntry ?? realPair.OriginalWayOut).DateHour).TotalMinutes;
    }
}