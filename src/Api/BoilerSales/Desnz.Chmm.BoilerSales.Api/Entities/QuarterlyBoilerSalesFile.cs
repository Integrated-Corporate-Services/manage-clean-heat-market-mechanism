using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.BoilerSales.Api.Entities;

/// <summary>
/// Quarterly boiler sales file
/// </summary>
public class QuarterlyBoilerSalesFile : Entity
{
    #region Constructors

    protected QuarterlyBoilerSalesFile() : base() { }

    public QuarterlyBoilerSalesFile(string fileKey, string fileName, string? createdBy = null) : base(createdBy)
    {
        FileKey = fileKey;
        FileName = fileName;
    }

    #endregion

    #region Properties

    public Guid QuarterlyBoilerSalesId { get; private set; }

    /// <summary>
    /// Quarterly boiler sales file is for
    /// </summary>
    public QuarterlyBoilerSales QuarterlyBoilerSales { get; private set; }

    /// <summary>
    /// File key (AWS reference)
    /// </summary>
    public string FileKey { get; private set; }

    /// <summary>
    /// Name of file
    /// </summary>
    public string FileName { get; private set; }
    #endregion
}
