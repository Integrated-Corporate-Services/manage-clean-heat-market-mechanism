using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

public abstract class UploadDecisionFilesCommandHandlerBase<T> : BaseRequestHandler<T, ActionResult>
    where T : UploadDecisionFilesCommandBase
{
    private readonly IFileService _fileService;
    private readonly IOrganisationsRepository _organisationsRepository;

    public UploadDecisionFilesCommandHandlerBase(
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

        foreach (var file in command.Files)
        {
            var fileKey = $"{organisationId}/{file.FileName}";

            var uploadResponse = await _fileService.UploadFileAsync(bucket, fileKey, file);
            if (uploadResponse.ValidationError != null)
                return Responses.BadRequest(uploadResponse.ValidationError);
        }

        return Responses.Ok();
    }
}
