namespace Desnz.Chmm.Identity.Common.Queries;

public class GetApprovalFilesQuery : GetDecisionFilesQueryBase
{
    public GetApprovalFilesQuery(Guid organisationId) : base(organisationId)
    {
    }
}
