using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries;

/// <summary>
/// Handle the get unlinked licence holders query
/// </summary>
public class GetLicenceHoldersUnlinkedQueryHandler : BaseRequestHandler<GetLicenceHoldersUnlinkedQuery, ActionResult<List<LicenceHolderDto>>>
{
    private readonly ILicenceHolderRepository _licenceHoldersRepository;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// DI Constructor
    /// </summary>
    public GetLicenceHoldersUnlinkedQueryHandler(
        ILogger<BaseRequestHandler<GetLicenceHoldersUnlinkedQuery, ActionResult<List<LicenceHolderDto>>>> logger,
        ILicenceHolderRepository licenceHoldersRepository,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider) : base(logger)
    {
        _licenceHoldersRepository = licenceHoldersRepository;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <summary>
    /// Handles the query
    /// </summary>
    /// <param name="request">Details of the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the licence holders</returns>
    public override async Task<ActionResult<List<LicenceHolderDto>>> Handle(GetLicenceHoldersUnlinkedQuery request, CancellationToken cancellationToken)
    {
        var licenceHolders = await _licenceHoldersRepository.GetAll(l => !l.LicenceHolderLinks.Any(l => l.EndDate == null || l.EndDate >= _dateTimeProvider.UtcDateNow));
        return _mapper.Map<List<LicenceHolderDto>>(licenceHolders);
    }
}
