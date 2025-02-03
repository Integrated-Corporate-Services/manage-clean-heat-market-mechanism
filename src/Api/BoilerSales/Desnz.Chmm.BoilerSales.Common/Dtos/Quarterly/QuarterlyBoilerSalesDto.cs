namespace Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;

/// <summary>
/// Quarterly boiler sales
/// </summary>
public class QuarterlyBoilerSalesDto : SalesNumbersDto
{
    #region Properties

    /// <summary>
    /// Scheme year quarter boiler sales are reported for
    /// </summary>
    public Guid SchemeYearQuarterId { get; set; }

    /// <summary>
    /// Organisation boiler sales are reported for
    /// </summary>
    public Guid OrganisationId { get; set; }

    /// <summary>
    /// Admin review <see cref="BoilerSalesConstants.Status">status</see>
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Files to support quarterly boiler sales
    /// </summary>
    public List<QuarterlyBoilerSalesFileDto> Files { get; set; }

    /// <summary>
    /// History of changes
    /// </summary>
    public List<QuarterlyBoilerSalesChangeDto> Changes { get; set; }

    #endregion
}
