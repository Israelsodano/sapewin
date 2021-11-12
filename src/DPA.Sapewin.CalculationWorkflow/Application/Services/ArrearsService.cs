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

            var rappointments = _appointmentsService.GetAppointmentsBasedInEletronicPoint(eletronicPoint);
                               
            var eintervals = GetAllEntryApointmentsBasedInEletronicPont(eletronicPoint, 
                                                                        rappointments.eappointment).ToArray();

            var eappointments = from ea in eintervals  
                      select new { bappointment = _appointmentsService.GetBestAppointment(ea, 
                                                                     ref appointments, 
                                                                     true, 
                                                                     eintervals), 
                                                                     rappointment = ea };

            var fpappointments = from a in eappointments
                            where a.rappointment.Date <= rappointments.iiapointment.Date
                            select a;

            
            var fparrear = (from df in (from f in fpappointments
                                                select (f.rappointment.Date - f.bappointment.Date).TotalMinutes)
                            select df > 0 ? df : 0).Sum();
            
            


        }

        private IEnumerable<DateTime> GetAllEntryApointmentsBasedInEletronicPont(EletronicPoint eletronicPoint, 
                                                                                 DateTime eappointment)
        {
            var ax = (from aux in eletronicPoint.Schedule.AuxiliaryIntervals ?? new AuxiliaryInterval[] { }
                                where aux.DiscountInterval && aux.Kind == AuxiliaryIntervalKind.Fixed
                                select 
                                eletronicPoint.Date.AddMinutes(aux.Entry)
                                        .AddDays(_appointmentsService
                                                    .GetOnlyMinutesFromDateTime(eappointment) > aux.Entry ? 
                                                        1 : 0 
                                        )).ToList();
            
            ax.Add(eappointment);

            return ax;
        }
    }
}