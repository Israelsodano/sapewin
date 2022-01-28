using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.CalculationWorkflow.Application.Records
{
    public record CalculatedDay(double dsrAmount, 
                                bool dsr,
                                bool workDay,
                                double absencesAmount, 
                                double arrearsAmount,
                                EletronicPoint eletronicPoint)
    {
        public bool HaveAbsence() => absencesAmount > 0;
        public bool HaveArrear() => absencesAmount > 0;
    }
}