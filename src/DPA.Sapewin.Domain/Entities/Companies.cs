using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Companies : Entity
    {
        public string Name { get; set; }       
        public Guid? SheetId { get; set; }
        public Address Address { get; set; }
        public string Document { get; set; }
        public string StateRegistration { get; set; }
        public string CIS { get; set; }
        public IEnumerable<Setores> Sectors { get; set; }
        public IEnumerable<PermissoesdeSetores> SectorPermissions { get; set; }
        public IEnumerable<Departments> Departments { get; set; }
        public IEnumerable<PermissoesdeDepartamentos> DepartmentPermissions { get; set; }
        public IEnumerable<Employees> Funcionarios { get; set; }
        public IEnumerable<PermissoesdeFuncionarios> EmployeePermissions { get; set; }
        public IEnumerable<PermissoesdeEmpresas> CompanyPermissions { get; set; }
        public IEnumerable<PermissoesdeTelas> ViewPermissions { get; set; }
        public IEnumerable<Scales> Scales { get; set; }
        public IEnumerable<ScheduleScales> ScheduleScales { get; set; }
        public IEnumerable<Horarios> Schedules { get; set; }
        public IEnumerable<IntervalosAuxiliares> AuxiliaryIntervals { get; set; }
    }
}