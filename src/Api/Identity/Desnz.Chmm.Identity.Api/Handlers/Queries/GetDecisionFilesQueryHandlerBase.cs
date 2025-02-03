using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

public abstract class GetDecisionFilesQueryHandlerBase<T> : BaseRequestHandler<T, ActionResult<List<string>>>
    where T : GetDecisionFilesQueryBase
{
    private readonly IFileService _fileService;
    private readonly IOrganisationsRepository _organisationsRepository;

    public GetDecisionFilesQueryHandlerBase(
        ILogger<BaseRequestHandler<T, ActionResult<List<string>>>> logger,
        IFileService fileService,
        IOrganisationsRepository organisationsRepository) : base(logger)
    {
        _fileService = fileService;
        _organisationsRepository = organisationsRepository;
    }

    protected async Task<ActionResult<List<string>>> Process(T query, string bucket, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;

        var organisation = await _organisationsRepository.GetById(organisationId);
        if (organisation == null)
            return CannotFindOrganisation(organisationId);

        return await _fileService.GetFileNamesAsync(bucket, $"{organisationId}");
    }
}
