using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

/// <summary>
/// Handle the get licence holders query
/// </summary>
public class GetLicenceHoldersAllQueryHandler : BaseRequestHandler<GetLicenceHoldersAllQuery, ActionResult<List<LicenceHolderDto>>>
{
    private readonly ILicenceHolderRepository _licenceHoldersRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// DI Constructor
    /// </summary>
    public GetLicenceHoldersAllQueryHandler(
        ILogger<BaseRequestHandler<GetLicenceHoldersAllQuery, ActionResult<List<LicenceHolderDto>>>> logger,
        ILicenceHolderRepository licenceHoldersRepository,
        IMapper mapper) : base(logger)
    {
        _licenceHoldersRepository = licenceHoldersRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handls the query
    /// </summary>
    /// <param name="request">Details of the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the licence holders</returns>
    public override async Task<ActionResult<List<LicenceHolderDto>>> Handle(GetLicenceHoldersAllQuery request, CancellationToken cancellationToken)
    {
        var licenceHolders = await _licenceHoldersRepository.GetAll();
        return _mapper.Map<List<LicenceHolderDto>>(licenceHolders);
    }
}
