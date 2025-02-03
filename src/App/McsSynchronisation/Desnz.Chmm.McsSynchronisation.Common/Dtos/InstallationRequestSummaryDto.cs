namespace Desnz.Chmm.McsSynchronisation.Common.Dtos;

/// <summary>
/// Summary information for a MCS Installation Request
/// </summary>
public class InstallationRequestSummaryDto
{
    public InstallationRequestSummaryDto(Guid id, DateTime requestDate, DateTime startDate, DateTime endDate, IEnumerable<string>? technologyTypes, IEnumerable<string>? newBuilds)
    {
        Id = id;
        RequestDate = requestDate;
        StartDate = startDate;
        EndDate = endDate;
        TechnologyTypes = technologyTypes?.ToArray();
        IsNewBuilds = newBuilds?.ToArray();
    }

    /// <summary>
    /// The Id of the installation request
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// The date the request to MCS was made
    /// </summary>
    public DateTime RequestDate { get; private set; }

    /// <summary>
    /// The start of the date range of data requested
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// The end of the date range of data requested
    /// </summary>
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// The list of technology types to include
    /// </summary>
    public string[]? TechnologyTypes { get; private set; }

    /// <summary>
    /// The list of property types included in the request
    /// </summary>
    public string[]? IsNewBuilds { get; private set; }
}
