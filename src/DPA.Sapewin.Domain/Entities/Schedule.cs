using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Schedule : Entity
    {
        public Guid CompanyId { get; set; }

        public string Description { get; set; }

        public Guid PeriodId { get; set; }

        public Period Period { get; set; }

        public Guid TotalsId { get; set; }
        
        public Totals Totals { get; set; }

        public int? DayTurn { get; set; }

        public bool DiscountInterval { get; set; }

        public bool IsTwentyFourHours { get; set; }

        public bool IsWeeklyCharge { get; set; }

        public int Charge { get; set; }

        public ScheduleKind Kind { get; set; }

        public Company Company { get; set; }        

        public IList<AuxiliaryInterval> AuxiliaryIntervals { get; set; }

        public IList<OccasionalSchedule> OccasionalSchedules { get; set; }

        public enum ScheduleKind
        {
            Fixed=1, Charge=2
        };
    }
}