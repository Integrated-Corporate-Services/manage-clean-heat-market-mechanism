using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Entities;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Entities;

/// <summary>
/// Annual boiler sales
/// </summary>
public class AnnualBoilerSales : Entity
{
    #region Constructors

    protected AnnualBoilerSales() : base() { }

    public AnnualBoilerSales(Guid schemeYearId, Guid organisationId, int gas, int oil, List<AnnualBoilerSalesFile> files, string? createdBy = null) : base(createdBy)
    {
        SchemeYearId = schemeYearId;
        OrganisationId = organisationId;
        Gas = gas;
        Oil = oil;
        Status = BoilerSalesStatus.Submitted;

        _files = files ?? new List<AnnualBoilerSalesFile>();
        _changes = new List<AnnualBoilerSalesChange>();
    }

    #endregion

    #region Private fields

    private List<AnnualBoilerSalesFile> _files;
    private List<AnnualBoilerSalesChange> _changes;

    #endregion

    #region Properties

    /// <summary>
    /// Scheme year boiler sales are reported for
    /// </summary>
    public Guid SchemeYearId { get; private set; }

    /// <summary>
    /// Organisation boiler sales are reported for
    /// </summary>
    public Guid OrganisationId { get; private set; }

    /// <summary>
    /// Gas boiler sales for scheme year
    /// </summary>
    public int Gas { get; private set; }

    /// <summary>
    /// Oil boiler sales for scheme year
    /// </summary>
    public int Oil { get; private set; }

    /// <summary>
    /// Admin review <see cref="BoilerSalesStatusConstants.BoilerSalesStatus">status</see>
    /// </summary>
    public string Status { get; protected set; }

    /// <summary>
    /// Files to support annual boiler sales
    /// </summary>
    public IReadOnlyCollection<AnnualBoilerSalesFile> Files => _files;

    /// <summary>
    /// History of changes
    /// </summary>
    public IReadOnlyCollection<AnnualBoilerSalesChange> Changes => _changes;


    #endregion

    #region Behaviours

    /// <summary>
    /// Change the status of the annual boiler sales to approved.
    /// </summary>
    public void Approve()
    {
        Status = BoilerSalesStatus.Approved;
    }

    public void Edit(int gas, int oil, List<AnnualBoilerSalesFile> files)
    {
        Gas = gas;
        Oil = oil;
        _files.Clear();
        _files.AddRange(files);
    }

    #endregion
}
