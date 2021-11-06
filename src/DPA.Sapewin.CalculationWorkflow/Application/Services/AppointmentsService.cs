using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DPA.Sapewin.Domain.Entities;
using DPA.Sapewin.Repository;
using System.Linq;

namespace DPA.Sapewin.CalculationWorkflow.Application.Services
{
    public interface IAppointmentsService
    {
        Task<IEnumerable<Appointment>> ImportDirtyNotes(IEnumerable<Employee> employees, 
                                           DateTime initial, 
                                           DateTime final);
    }

    public class AppointmentsService : IAppointmentsService
    {
        private readonly IUnitOfWork<Appointment> _unitOfWorkAppointment;
        private readonly IUnitOfWork<DirtyNote> _unitOfWorkDirtyNote;

        public AppointmentsService(IUnitOfWork<Appointment> unitOfWorkAppointment, 
                                   IUnitOfWork<DirtyNote> unitOfWorkDirtyNote)
        {
            _unitOfWorkAppointment = unitOfWorkAppointment ?? throw new ArgumentNullException(nameof(unitOfWorkAppointment));
            _unitOfWorkDirtyNote = unitOfWorkDirtyNote ?? throw new ArgumentNullException(nameof(unitOfWorkDirtyNote));
        }  

        public async Task<IEnumerable<Appointment>> ImportDirtyNotes(IEnumerable<Employee> employees, 
                                           DateTime initial, 
                                           DateTime final)
        {
            var dirtyNotes = _unitOfWorkDirtyNote.Repository
                                .GetAll(x=> 
                                    employees.Any(z=> z.Pis == x.Pis) && 
                                    x.Date >= initial && x.Date <= final).ToArray();
            
            var appointments = _unitOfWorkAppointment.Repository
                                    .GetAll(x=> 
                                        employees.Any(y=> y.Id == x.EmployeeId) && 
                                        x.Date >= initial && x.Date <= final).ToList();

            dirtyNotes = (from d in dirtyNotes
                         where !appointments.Any(x=> x.UniqueAppointmentKey == d.UniqueAppointmentKey)
                         select d).ToArray();

            if (dirtyNotes.Any())
            {
                var ap = (from d in dirtyNotes select (Appointment)d)
                        .ToArray();

                await _unitOfWorkAppointment.Repository
                        .InsertAsync(ap);

                await _unitOfWorkAppointment.SaveChangesAsync();
                appointments.AddRange(ap);
            }

            return appointments;
        }
    }
}