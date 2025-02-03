using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Identity.Common.Constants.OrganisationConstants;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public class GetApprovalFilesQueryHandler : GetDecisionFilesQueryHandlerBase<GetApprovalFilesQuery>
{
    public GetApprovalFilesQueryHandler(
        ILogger<BaseRequestHandler<GetApprovalFilesQuery, ActionResult<List<string>>>> logger,
        IFileService fileService,
        IOrganisationsRepository organisationsRepository) : base(logger, fileService, organisationsRepository)
    {
    }

    public override async Task<ActionResult<List<string>>> Handle(GetApprovalFilesQuery query, CancellationToken cancellationToken)
    {
        return await Process(query, Buckets.IdentityOrganisationApprovals, cancellationToken);
    }
}
