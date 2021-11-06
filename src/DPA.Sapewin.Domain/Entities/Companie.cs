using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Company : Entity
    {
        public string Name { get; set; }       
        public Guid? SheetId { get; set; }
        public Address Address { get; set; }
        public string Document { get; set; }
        public string StateRegistration { get; set; }
        public string CIS { get; set; }
        public IEnumerable<Setores> Sectors { get; set; }
        public IEnumerable<PermissoesdeSetores> SectorPermissions { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<PermissoesdeDepartamentos> DepartmentPermissions { get; set; }
        public IEnumerable<Employee> Funcionarios { get; set; }
        public IEnumerable<PermissoesdeFuncionarios> EmployeePermissions { get; set; }
        public IEnumerable<PermissoesdeEmpresas> CompanyPermissions { get; set; }
        public IEnumerable<ScreenPermissions> ViewPermissions { get; set; }
        public IEnumerable<Scale> Scales { get; set; }
        public IEnumerable<ScheduleScales> ScheduleScales { get; set; }
        public IEnumerable<Schedule> Schedules { get; set; }
        public IEnumerable<AuxiliaryInterval> AuxiliaryIntervals { get; set; }
    }
}