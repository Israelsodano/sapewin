using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Domain.Models;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IEarlyWayOutService
    {

    }
    public class EarlyWayOutService : IEarlyWayOutService
    {
        public Task<IEnumerable<EletronicPoint>> CalculateEarlyWayOut(IEnumerable<IGrouping<EletronicPoint, EletronicPointPairs>> eletronicPointPairsGroups, IEnumerable<EmployeeCalendars> ecalendars)
        {
            foreach (var ep in eletronicPointPairsGroups)
                yield return CalculateEarlyWayOut(ep.Key, ep,
                    GetCalendar(
                        GetCalendars(ep.Key, ecalendars), ep.Key));
        }
        private EmployeeCalendars GetCalendars(EletronicPoint ep, IEnumerable<EmployeeCalendars> ecalendars)
            => ecalendars.FirstOrDefault(x => x.Employee.Id == ep.Id && x.Employee.CompanyId == ep.CompanyId);

        private Calendar GetCalendar(EmployeeCalendars calendars, EletronicPoint ep)
            => calendars.Calendars.FirstOrDefault(x => x.Date.Date == ep.Date.Date);

        private bool IsNullPair(EletronicPointPairs epp)
            => epp.OriginalEntry is null || epp.OriginalWayOut is null
            || string.IsNullOrEmpty(epp.OriginalEntry.UniqueAppointmentKey)
            || string.IsNullOrEmpty(epp.OriginalWayOut.UniqueAppointmentKey);

        private IEnumerable<int> GetPerfectAppointments(Schedule schedule)
        {
            var result = (from p in schedule.AuxiliaryIntervals
                          where p.DiscountInterval && p.Kind == AuxiliaryIntervalKind.Fixed
                          select p.WayOut).ToList();
            ;
            result.AddRange(new int[] { schedule.Period.WayOut, schedule.Period.Entry });
            return result;
        }


        private async Task<EletronicPoint> CalculateEarlyWayOut(EletronicPoint ep, IEnumerable<EletronicPointPairs> epps, Calendar calendar)
        {
            if (int.TryParse(calendar.NonScheduleReference, out int tryparse))
            {
                var wayOutAppointments = epps.Where(x => !IsNullPair(x)).Select(x => x.OriginalWayOut).ToList();
                var appointments = wayOutAppointments;
                appointments.AddRange(epps.Where(x => x.OriginalEntry is not null).Select(x => x.OriginalEntry));

                var perfectAppointments = GetPerfectAppointments(ep.Schedule);

                var atribuidor = new Dictionary<DateTime, DateTime>();

                foreach (var marcacaoPerfeita in perfectAppointments)
                {
                    var result = await PegaMarcacao(horarioDataHora, marcacoesSaida, marcacaoPerfeita, true, marcacoesPerfeitas);
                    if (result != null)
                        atribuidor.Add(horarioDataHora.DatasHorario[marcacaoPerfeita], result.datahora);
                }

                var periodo1 = atribuidor.Keys.Where(x => x <= marcacaoIntervaloEntrada.datahora).ToArray();

                if (funcionario.IntervaloFixo)
                    foreach (var marcacao in periodo1)
                    {
                        int diff = (int)(marcacao - atribuidor[marcacao]).TotalMinutes;

                        diff = diff > 0 ? diff : 0;

                        saidaAntecipadaPer1 += diff;
                    }


                var periodo2 = atribuidor.Keys.Where(x => x > marcacaoIntervaloEntrada.datahora).ToArray();

                foreach (var marcacao in periodo2)
                {
                    int diff = (int)(marcacao - atribuidor[marcacao]).TotalMinutes;

                    diff = diff > 0 ? diff : 0;

                    saidaAntecipadaPer2 += diff;
                }
            }

            var totalParametros = Calculadora.HorasparaMinuto(funcionario.Parametro.SaidaJornada);

            saidaAntecipadaPer1 = saidaAntecipadaPer1 + saidaAntecipadaPer2 > totalParametros ? saidaAntecipadaPer1 : 0;
            saidaAntecipadaPer2 = saidaAntecipadaPer1 + saidaAntecipadaPer2 > totalParametros ? saidaAntecipadaPer2 : 0;

            saidaAntecipadaPer1 = saidaAntecipadaPer1 > Calculadora.HorasparaMinuto(funcionario.Parametro.SaidaTotal1P) ? saidaAntecipadaPer1 : 0;
            saidaAntecipadaPer2 = saidaAntecipadaPer2 > Calculadora.HorasparaMinuto(funcionario.Parametro.SaidaTotal2P) ? saidaAntecipadaPer2 : 0;

            ponto.SaidaDesPer1 = Calculadora.MinutosparaHora(saidaAntecipadaPer1);
            ponto.SaidaDesPer2 = Calculadora.MinutosparaHora(saidaAntecipadaPer2);

            return ponto;
        }
    }
}