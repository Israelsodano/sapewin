using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Employee : Entity
    {
        public Guid PositionId { get; set; }
        public string Document { get; set; }
        public Address Address { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid SectorId { get; set; }
        public Guid ParmeterId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? HolidayId { get; set; }
        public string Name { get; set; }
        public string Pis { get; set; }
        public DateTime Admission { get; set; }
        public DateTime? Termination { get; set; }
        public Positions Position { get; set; }
        public IEnumerable<PermissoesdeFuncionarios> EmployeePermissions { get; set; }
        public Setores Sector { get; set; }
        public Parametros Parameter { get; set; }
        public Department Department { get; set; }
        public Companie Company { get; set; }
        public HolidayGroup HolidaysGroup { get; set; }
        public bool FixedInterval { get; set; }
        public EmployeeHoliday Holiday { get; set; }
        public EmployeeInterval Interval { get; set; }
        public Guid SheetId { get; set; }
        public int? CTPSNum { get; set; }
        public int? Serial { get; set; }
        public string Phone { get; set; }
        public long Salary { get; set; }
        public string Observations { get; set; }
        public bool StandartPicture { get; set; }
        public Guid ScaleId { get; set; }
        public IEnumerable<MensagensFuncionarios> CardMessages { get; set; }
        public IEnumerable<Leaves> Leaves { get; set; }
        public IEnumerable<DayOff> DaysOff { get; set; }
        public IEnumerable<HorariosOcasionais> OccasionalSchedules { get; set; }
        public IEnumerable<ProximityCard> ProximityCards { get; set; }
        public Scales Scale { get; set; }
    }
    
    public enum EmployeeHoliday
    {
        DayOff = 1, Work = 2
    }

    public enum EmployeeInterval
    {
        Manual = 1, PreAssigned = 2
    }
}