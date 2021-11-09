using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;
using static DPA.Sapewin.Domain.Entities.AuxiliaryInterval;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IArrearService
    {

    }

    public class ArrearsService : IArrearService
    {
        private readonly IAppointmentsService _appointmentsService;

        public ArrearsService(IAppointmentsService appointmentsService)
        {
            _appointmentsService = appointmentsService ?? throw new ArgumentNullException(nameof(appointmentsService));
        }

        public async Task CalculateArrearsByEletronicPoint(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointsPairs)
        {

        }

        private void CalculateArrears(EletronicPoint eletronicPoint, IEnumerable<EletronicPointPairs> pairs)
        {
            var appointments = (from p in pairs.SelectMany(x=> x.GetAppointments()) 
                               where p is not null
                               select p).ToList();  
                               
            var eappointments = GetAllEntryApointmentsBasedInEletronicPont(eletronicPoint).ToArray();

            var app = from ea in eappointments  
                      select _appointmentsService.GetBestAppointment(ea, ref appointments, 
                                                                     true, eappointments);
        }

        private IEnumerable<DateTime> GetAllEntryApointmentsBasedInEletronicPont(EletronicPoint eletronicPoint)
        {
            var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPoint);

            var ax = (from aux in eletronicPoint.Schedule.AuxiliaryIntervals ?? new AuxiliaryInterval[] { }
                                where aux.DiscountInterval && aux.Kind == AuxiliaryIntervalKind.Fixed
                                select 
                                eletronicPoint.Date.AddMinutes(aux.Entry)
                                        .AddDays(_appointmentsService
                                                    .GetOnlyMinutesFromDateTime(rappointments.eappointment) > aux.Entry ? 
                                                        1 : 0 
                                        )).ToList();
            
            ax.Add(rappointments.eappointment);

            return ax;
        }
    }
}