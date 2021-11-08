using System.Collections.Generic;
using System.Linq;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IFitAppointmentsService
    {

    }
    public class FitAppointmentsService : IFitAppointmentsService
    {
        private IList<EletronicPointPairs> FitAppointmentsLoad(IList<Appointment> appointments, Schedule schedule,
            EletronicPoint eletronicPoint, Employee employee)
        {
            appointments = appointments.OrderBy(x => x.Date).ToList();

            var vetores = new List<IList<Appointment>>();
            var vetor = new List<Appointment>();

            for (int i = 0; i < appointments.Count; i++)
            {
                vetor.Add(appointments[i]);

                if (vetor.Count == 2)
                {
                    vetores.Add(vetor);
                    vetor = new List<Appointment>();
                }

                if (appointments.Count - 1 == i && vetor.Count > 0)
                {
                    vetor.Add(null);
                    vetores.Add(vetor);
                }
            }

            var helpers = new List<HelperPares>();

            for (int i = 0; i < appointments.Count; i += 2)
            {
                var helper = i == appointments.Count - 1 
                    ? new HelperPares(appointments[i], null, schedule, eletronicPoint) 
                    : new HelperPares(appointments[i], appointments[i + 1], schedule, ponto);

                helpers.Add(helper);
            }
            return helpers;
        }
    }
}