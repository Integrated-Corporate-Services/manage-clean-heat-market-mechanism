
namespace Desnz.Chmm.McsSynchronisation.Common.Dtos
{
    public class PeriodCreditTotals
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int HeatPumpsInstallations { get; set; }
        public int HybridHeatPumpsInstallations { get; set; }
        public decimal HeatPumpsGeneratedCredits { get; set; }
        public decimal HybridHeatPumpsGeneratedCredits { get; set; }


        public PeriodCreditTotals(DateOnly startDate,
                                  DateOnly endDate,
                                  int heatPumpsInstallations,
                                  int hybridHeatPumpsInstallations,
                                  decimal heatPumpsGeneratedCredits,
                                  decimal hybridHeatPumpsGeneratedCredits) 
        {
            StartDate = startDate;
            EndDate = endDate;
            HeatPumpsInstallations = heatPumpsInstallations;
            HybridHeatPumpsInstallations = hybridHeatPumpsInstallations;
            HeatPumpsGeneratedCredits = heatPumpsGeneratedCredits;
            HybridHeatPumpsGeneratedCredits = hybridHeatPumpsGeneratedCredits;
        }
    }
}
