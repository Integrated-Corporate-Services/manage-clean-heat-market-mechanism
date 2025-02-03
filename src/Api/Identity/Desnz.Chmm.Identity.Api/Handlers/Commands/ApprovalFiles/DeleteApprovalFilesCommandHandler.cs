using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands.ApprovalFiles;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Identity.Common.Constants.OrganisationConstants;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands.ApprovalFiles;

public class DeleteApprovalFilesCommandHandler : DeleteDecisionFilesCommandHandlerBase<DeleteApprovalFilesCommand>
{
    public DeleteApprovalFilesCommandHandler(
        ILogger<BaseRequestHandler<DeleteApprovalFilesCommand, ActionResult>> logger,
        IFileService fileService,
        IOrganisationsRepository organisationsRepository) : base(logger, fileService, organisationsRepository)
    {
    }

    public override async Task<ActionResult> Handle(DeleteApprovalFilesCommand command, CancellationToken cancellationToken)
    {
        return await Process(command, Buckets.IdentityOrganisationApprovals, cancellationToken);
    }
}
