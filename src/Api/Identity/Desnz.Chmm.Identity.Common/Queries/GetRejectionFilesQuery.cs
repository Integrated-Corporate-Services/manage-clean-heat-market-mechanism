namespace Desnz.Chmm.Identity.Common.Queries;

public class GetRejectionFilesQuery : GetDecisionFilesQueryBase
{
    public GetRejectionFilesQuery(Guid organisationId) : base(organisationId)
    {
    }
}
