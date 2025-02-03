using static Desnz.Chmm.BoilerSales.Api.Services.BoilerSalesFileCopyService;

namespace Desnz.Chmm.BoilerSales.Api.Services;

public interface IBoilerSalesFileCopyService
{
    /// <summary>
    /// Prepare a new annual editing session by deleting all existing "edit" files, then copying over the original files to include the "edit" prefix
    /// </summary>
    /// <returns></returns>
    Task<List<string?>> PrepareForEditing(Guid organisationId, Guid schemeYearId);

    /// <summary>
    /// Prepare a new quarterly editing session by deleting all existing "edit" files, then copying over the original files to include the "edit" prefix
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="schemeYearId"></param>
    /// <param name="schemeYearQuarterId"></param>
    /// <returns></returns>
    Task<List<string?>> PrepareForEditing(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId);

    /// <summary>
    /// Conclude annual editing session by deleting all existing original files, then copying over the edit files to remove the "edit" prefix
    /// </summary>
    /// <returns></returns>
    Task<ConcludedEditingSession> ConcludeEditing(Guid organisationId, Guid schemeYearId);

    /// <summary>
    /// Conclude quarterly editing session by deleting all existing original files, then copying over the edit files to remove the "edit" prefix
    /// </summary>
    /// <param name="organisationId"></param>
    /// <param name="schemeYearId"></param>
    /// <param name="schemeYearQuarterId"></param>
    /// <returns></returns>
    Task<ConcludedEditingQuarterlySession> ConcludeEditing(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId);
}
