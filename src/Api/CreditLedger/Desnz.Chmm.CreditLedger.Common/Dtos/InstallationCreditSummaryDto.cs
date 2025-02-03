namespace Desnz.Chmm.CreditLedger.Common.Dtos;

/// <summary>
/// Installation credit
/// </summary>
public class InstallationCreditSummaryDto
{
    public InstallationCreditSummaryDto(decimal creditsGeneratedFromHeatPumpInstallations, decimal creditsGeneratedFromHybridHeatPumpInstallations)
    {
        CreditsGeneratedFromHeatPumpInstallations = creditsGeneratedFromHeatPumpInstallations;
        CreditsGeneratedFromHybridHeatPumpInstallations = creditsGeneratedFromHybridHeatPumpInstallations;
    }

    public decimal CreditsGeneratedFromHeatPumpInstallations { get; set; }
    public decimal CreditsGeneratedFromHybridHeatPumpInstallations { get; set; }
}
