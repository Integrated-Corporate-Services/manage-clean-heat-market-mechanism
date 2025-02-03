using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Entities;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Entities;

/// <summary>
/// Quarterly boiler sales
/// </summary>
public class QuarterlyBoilerSales : Entity
{
    #region Constructors

    protected QuarterlyBoilerSales() : base() { }

    public QuarterlyBoilerSales(Guid organisationId,
                                Guid schemeYearId,
                                Guid schemeYearQuarterId,
                                int gas,
                                int oil,
                                List<QuarterlyBoilerSalesFile>? files,
                                string? createdBy = null) : base(createdBy)
    {
        SchemeYearId = schemeYearId;
        SchemeYearQuarterId = schemeYearQuarterId;
        OrganisationId = organisationId;
        Gas = gas;
        Oil = oil;
        Status = BoilerSalesStatus.Submitted;
        _files = files ?? new();
        _changes = new List<QuarterlyBoilerSalesChange>();
    }

    #endregion

    #region Private fields

    private List<QuarterlyBoilerSalesFile> _files;
    private List<QuarterlyBoilerSalesChange> _changes;

    #endregion

    #region Properties

    /// <summary>
    /// Scheme year boiler sales are reported for
    /// </summary>
    public Guid SchemeYearId { get; private set; }

    /// <summary>
    /// Scheme year quarter boiler sales are reported for
    /// </summary>
    public Guid SchemeYearQuarterId { get; private set; }

    /// <summary>
    /// Organisation boiler sales are reported for
    /// </summary>
    public Guid OrganisationId { get; private set; }

    /// <summary>
    /// Gas boiler sales for scheme year quarter
    /// </summary>
    public int Gas { get; private set; }

    /// <summary>
    /// Oil boiler sales for scheme year quarter
    /// </summary>
    public int Oil { get; private set; }

    /// <summary>
    /// Admin review <see cref="BoilerSalesStatusConstants.BoilerSalesStatus">status</see>
    /// </summary>
    public string Status { get; protected set; }

    /// <summary>
    /// Files to support quarterly boiler sales
    /// </summary>
    public IReadOnlyCollection<QuarterlyBoilerSalesFile> Files => _files;

    /// <summary>
    /// History of changes
    /// </summary>
    public IReadOnlyCollection<QuarterlyBoilerSalesChange> Changes => _changes;

    #endregion

    #region Behaviours

    /// <summary>
    /// Change the status of the quarterly boiler sales to approved.
    /// </summary>
    public void Approve()
    {
        Status = BoilerSalesStatus.Approved;
    }

    public void Edit(int gas, int oil, List<QuarterlyBoilerSalesFile> files)
    {
        Gas = gas;
        Oil = oil;
        _files.Clear();
        _files.AddRange(files);
    }

    #endregion
}
