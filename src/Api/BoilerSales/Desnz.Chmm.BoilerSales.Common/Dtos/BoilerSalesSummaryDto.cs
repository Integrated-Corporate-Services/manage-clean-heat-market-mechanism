namespace Desnz.Chmm.BoilerSales.Common.Dtos;

/// <summary>
/// Contains all the details of the boiler sales for a given organisation and year
/// </summary>
public class BoilerSalesSummaryDto
{
    public BoilerSalesSummaryDto(
        Guid organisationId,
        int gasBoilerSales,
        int oilBoilerSales,
        int gasBoilerSalesAboveThreshold,
        int oilBoilerSalesAboveThreshold,
        string boilerSalesSubmissionStatus)
    {
        OrganisationId = organisationId;

        GasBoilerSales = gasBoilerSales;
        OilBoilerSales = oilBoilerSales;
        GasBoilerSalesAboveThreshold = gasBoilerSalesAboveThreshold;
        OilBoilerSalesAboveThreshold = oilBoilerSalesAboveThreshold;
        SumOfBoilerSales = gasBoilerSales + oilBoilerSales;
        SumOfBoilerSalesAboveThreshold = gasBoilerSalesAboveThreshold + oilBoilerSalesAboveThreshold;

        BoilerSalesSubmissionStatus = boilerSalesSubmissionStatus;
    }

    public Guid OrganisationId { get; private set; }
    public int GasBoilerSales { get; private set; }
    public int OilBoilerSales { get; private set; }
    public int SumOfBoilerSales { get; private set; }
    public int GasBoilerSalesAboveThreshold { get; private set; }
    public int SumOfBoilerSalesAboveThreshold { get; private set; }
    public int OilBoilerSalesAboveThreshold { get; private set; }
    public string BoilerSalesSubmissionStatus { get; private set; }
}