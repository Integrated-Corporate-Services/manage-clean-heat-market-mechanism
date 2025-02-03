using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.BoilerSales.Api.Entities;

/// <summary>
/// Annual boiler sales file
/// </summary>
public class AnnualBoilerSalesFile : Entity
{
    #region Constructors

    protected AnnualBoilerSalesFile() : base() { }

    public AnnualBoilerSalesFile(string fileKey, string fileName, string type, string? createdBy = null) : base(createdBy)
    {
        FileKey = fileKey;
        FileName = fileName;
        Type = type;
    }

    internal AnnualBoilerSalesFile(AnnualBoilerSales annualBoilerSales, string fileKey, string fileName, string type, string? createdBy = null) : base(createdBy)
    {
        AnnualBoilerSalesId = annualBoilerSales.Id;
        AnnualBoilerSales = annualBoilerSales;
        FileKey = fileKey;
        FileName = fileName;
        Type = type;
    }

    #endregion

    #region Properties

    public Guid AnnualBoilerSalesId { get; private set; }

    /// <summary>
    /// Annual boiler sales file is for
    /// </summary>
    public AnnualBoilerSales AnnualBoilerSales { get; private set; }

    /// <summary>
    /// File key (AWS reference)
    /// </summary>
    public string FileKey { get; private set; }

    /// <summary>
    /// Name of file
    /// </summary>
    public string FileName { get; private set; }

    public string Type { get; private set; }
    #endregion
}
