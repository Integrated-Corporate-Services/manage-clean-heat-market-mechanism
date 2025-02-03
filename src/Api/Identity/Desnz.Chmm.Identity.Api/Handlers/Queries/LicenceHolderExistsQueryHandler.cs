using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

/// <summary>
/// Handle the get licence holders query
/// </summary>
public class LicenceHolderExistsQueryHandler : BaseRequestHandler<LicenceHolderExistsQuery, ActionResult<LicenceHolderExistsDto>>
{
    private readonly ILicenceHolderRepository _licenceHoldersRepository;

    /// <summary>
    /// DI Constructor
    /// </summary>
    public LicenceHolderExistsQueryHandler(
        ILogger<BaseRequestHandler<LicenceHolderExistsQuery, ActionResult<LicenceHolderExistsDto>>> logger,
        ILicenceHolderRepository licenceHoldersRepository) : base(logger)
    {
        _licenceHoldersRepository = licenceHoldersRepository;
    }

    /// <summary>
    /// Handls the query
    /// </summary>
    /// <param name="request">Details of the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the licence holders</returns>
    public override async Task<ActionResult<LicenceHolderExistsDto>> Handle(LicenceHolderExistsQuery request, CancellationToken cancellationToken)
    {
        Entities.LicenceHolder? licenceHolder = await _licenceHoldersRepository.Get(x => x.Id == request.LicenceHolderId);
        return new LicenceHolderExistsDto { Exists = licenceHolder != null };
    }
}
