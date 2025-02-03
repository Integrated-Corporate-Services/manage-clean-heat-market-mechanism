namespace Desnz.Chmm.Configuration.Common.Dtos;

public class SchemeYearSummaryConfigurationDto
{
    public SchemeYearSummaryConfigurationDto(
        string currentSchemeYear,
        decimal obligationCalculationPercentage,
        bool isAfterEndOfSchemeYearDate,
        bool isAfterSurrenderDay,
        bool isAfterPreviousSchemeYearSurrenderDate,
        bool isWithinAmendObligationsWindow,
        bool isWithinAmendCreditsWindow,
        bool isWithinCreditTranferWindow,
        decimal alternativeRenewableSystemFuelTypeWeightingValue,
        decimal alternativeFossilFuelSystemFuelTypeWeightingValue)
    {
        CurrentSchemeYear = currentSchemeYear;
        ObligationCalculationPercentage = obligationCalculationPercentage;
        IsAfterEndOfSchemeYearDate = isAfterEndOfSchemeYearDate;
        IsAfterSurrenderDay = isAfterSurrenderDay;
        IsAfterPreviousSchemeYearSurrenderDate = isAfterPreviousSchemeYearSurrenderDate;
        IsWithinAmendObligationsWindow = isWithinAmendObligationsWindow;
        IsWithinAmendCreditsWindow = isWithinAmendCreditsWindow;
        IsWithinCreditTranferWindow = isWithinCreditTranferWindow;
        AlternativeRenewableSystemFuelTypeWeightingValue = alternativeRenewableSystemFuelTypeWeightingValue;
        AlternativeFossilFuelSystemFuelTypeWeightingValue = alternativeFossilFuelSystemFuelTypeWeightingValue;
    }

    public string CurrentSchemeYear { get; private set; }
    public decimal ObligationCalculationPercentage { get; private set; }
    public bool IsAfterEndOfSchemeYearDate { get; private set; }
    public bool IsAfterSurrenderDay { get; private set; }
    public bool IsAfterPreviousSchemeYearSurrenderDate { get; private set; }
    public bool IsWithinAmendObligationsWindow { get; private set; }
    public bool IsWithinAmendCreditsWindow { get; private set; }
    public bool IsWithinCreditTranferWindow { get; private set; }
    public decimal AlternativeRenewableSystemFuelTypeWeightingValue { get; private set; }
    public decimal AlternativeFossilFuelSystemFuelTypeWeightingValue { get; private set; }
}
