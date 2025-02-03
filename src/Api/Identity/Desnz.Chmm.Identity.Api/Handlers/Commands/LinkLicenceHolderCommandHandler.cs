using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

/// <summary>
/// Handle the command to link a licence holder to an organisation
/// </summary>
public class LinkLicenceHolderCommandHandler : BaseRequestHandler<LinkLicenceHolderCommand, ActionResult>
{
    private readonly IOrganisationsRepository _organisationsRepository;
    private readonly ILicenceHolderRepository _licenceHoldersRepository;
    private readonly ILicenceHolderLinkRepository _licenceHoldersLinkRepository;
    private readonly ISchemeYearService _schemeYearService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICreditLedgerService _creditLedgerService;

    /// <summary>
    /// DI Constructor
    /// </summary>
    public LinkLicenceHolderCommandHandler(
        ILogger<BaseRequestHandler<LinkLicenceHolderCommand, ActionResult>> logger,
        IOrganisationsRepository organisationsRepository,
        ILicenceHolderRepository licenceHoldersRepository,
        ILicenceHolderLinkRepository licenceHoldersLinkRepository,
        ISchemeYearService schemeYearService,
        IDateTimeProvider dateTimeProvider,
        ICreditLedgerService creditLedgerService) : base(logger)
    {
        _organisationsRepository = organisationsRepository;
        _licenceHoldersRepository = licenceHoldersRepository;
        _licenceHoldersLinkRepository = licenceHoldersLinkRepository;
        _schemeYearService = schemeYearService;
        _dateTimeProvider = dateTimeProvider;
        _creditLedgerService = creditLedgerService;
    }

    /// <summary>
    /// Handles the command
    /// </summary>
    /// <param name="command">Details of the command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the licence holders</returns>
    public override async Task<ActionResult> Handle(LinkLicenceHolderCommand command, CancellationToken cancellationToken)
    {
        var organisation = await _organisationsRepository.Get(x => x.Id == command.OrganisationId);
        if (organisation == null)
            return CannotFindOrganisation(command.OrganisationId);

        if (organisation.Status != OrganisationConstants.Status.Active)
            return InvalidOrganisationStatus(command.OrganisationId, organisation.Status);

        var licenceHolder = await _licenceHoldersRepository.Get(x => x.Id == command.LicenceHolderId, includeLicenceHolderLinks: true);
        if (licenceHolder == null)
            return CannotFindLicenceHolder(command.LicenceHolderId);

        // If there are any links for the organisation with a null end date, it's already linked
        if (licenceHolder.LicenceHolderLinks != null && licenceHolder.LicenceHolderLinks.Any(l => l.EndDate == null || l.EndDate >= _dateTimeProvider.UtcDateNow))
            return LicenceHolderAlreadyLinked(command.LicenceHolderId);

        var firstSchemeYearResponse = await _schemeYearService.GetFirstSchemeYear();
        if (!firstSchemeYearResponse.IsSuccessStatusCode || firstSchemeYearResponse.Result == null)
            return CannotLoadFirstSchemeYear(firstSchemeYearResponse.Problem);
        var firstSchemeYear = firstSchemeYearResponse.Result;

        var currentSchemeYearResponse = await _schemeYearService.GetCurrentSchemeYear(cancellationToken);
        if (!currentSchemeYearResponse.IsSuccessStatusCode || currentSchemeYearResponse.Result == null)
            return CannotLoadCurrentSchemeYear(currentSchemeYearResponse.Problem);
        var currentSchemeYear = currentSchemeYearResponse.Result;

        if (currentSchemeYear.PreviousSchemeYearId.HasValue)
        { 
            var previousSchemeYearResponse = await _schemeYearService.GetSchemeYear(currentSchemeYear.PreviousSchemeYearId.Value, cancellationToken);
            if (!previousSchemeYearResponse.IsSuccessStatusCode || previousSchemeYearResponse.Result == null)
                return CannotLoadPreviousSchemeYear(previousSchemeYearResponse.Problem);
            var previousSchemeYear = previousSchemeYearResponse.Result;

            if (_dateTimeProvider.UtcDateNow > previousSchemeYear.SurrenderDayDate && 
                (!command.StartDate.HasValue || previousSchemeYear.SurrenderDayDate > command.StartDate.Value))
            {
                var startDate = command.StartDate.HasValue? command.StartDate.Value : firstSchemeYear.StartDate;
                HttpObjectResponse<object> response = await _creditLedgerService.CarryOverNewLicenceHolders(new CarryOverNewLicenceHoldersCommand(organisation.Id, licenceHolder.Id, previousSchemeYear.Id, startDate));

                if (!response.IsSuccessStatusCode)
                    return CannotCarryOverNewLicenceHolders(response.Problem);
            }
        }

        var licenceHolderLink = new LicenceHolderLink(organisation.Id, licenceHolder.Id, firstSchemeYear.StartDate, command.StartDate);

        var id = await _licenceHoldersLinkRepository.Create(licenceHolderLink, true);
        return Responses.Created(id);
    }
}
