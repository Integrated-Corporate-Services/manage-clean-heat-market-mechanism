using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

public abstract class DeleteDecisionFilesCommandHandlerBase<T> : BaseRequestHandler<T, ActionResult>
    where T : DeleteDecisionFilesCommandBase
{
    private readonly IFileService _fileService;
    private readonly IOrganisationsRepository _organisationsRepository;

    public DeleteDecisionFilesCommandHandlerBase(
        ILogger<BaseRequestHandler<T, ActionResult>> logger,
        IFileService fileService,
        IOrganisationsRepository organisationsRepository) : base(logger)
    {
        _fileService = fileService;
        _organisationsRepository = organisationsRepository;
    }

    protected async Task<ActionResult> Process(T command, string bucket, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;

        var organisation = await _organisationsRepository.GetById(organisationId);
        if (organisation == null)
            return CannotFindOrganisation(organisationId);

        var fileKey = $"{command.OrganisationId}/{command.FileName}";

        var deleteResponse = await _fileService.DeleteObjectNonVersionedBucketAsync(bucket, fileKey);
        if (deleteResponse.ValidationError != null)
            return ErrorDeletingFile(deleteResponse.ValidationError);

        return Responses.Ok();
    }
}
