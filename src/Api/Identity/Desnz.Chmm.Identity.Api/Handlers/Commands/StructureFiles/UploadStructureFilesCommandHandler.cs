using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands.StructureFiles;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Identity.Common.Constants.OrganisationConstants;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands.StructureFiles;

public class UploadStructureFilesCommandHandler : UploadDecisionFilesCommandHandlerBase<UploadStructureFilesCommand>
{
    public UploadStructureFilesCommandHandler(
        ILogger<BaseRequestHandler<UploadStructureFilesCommand, ActionResult>> logger,
        IFileService fileService,
        IOrganisationsRepository organisationsRepository) : base(logger, fileService, organisationsRepository)
    {
    }

    public override async Task<ActionResult> Handle(UploadStructureFilesCommand command, CancellationToken cancellationToken)
    {
        return await Process(command, Buckets.IdentityOrganisationStructures, cancellationToken);
    }
}
