using System;
namespace DPA.Sapewin.CalculationWorkflow.Application.Records
{
    public record Occurrence(double arrearMinutes, 
                             double absenceMinutes,
                             DateTime Date)
    {

        public static Occurrence Empty => new Occurrence(0, 0, DateTime.MinValue);

        public static Occurrence operator +(Occurrence o1, 
                                            Occurrence o2) =>
            new(o1.arrearMinutes + o2.arrearMinutes,
                o1.absenceMinutes + o2.absenceMinutes,
                o2.Date > o1.Date ? o2.Date : o1.Date);

        public static implicit operator double(Occurrence o) =>
            o.arrearMinutes + o.absenceMinutes;
    };
}