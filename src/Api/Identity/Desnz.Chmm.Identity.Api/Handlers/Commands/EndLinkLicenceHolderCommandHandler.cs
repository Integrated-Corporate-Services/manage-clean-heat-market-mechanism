using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands;

/// <summary>
/// Handle the command to unlink a licence holder from an organisation
/// </summary>
public class EndLinkLicenceHolderCommandHandler : BaseRequestHandler<EndLinkLicenceHolderCommand, ActionResult>
{
    private readonly ISchemeYearService _schemeYearService;
    private readonly ILicenceHolderRepository _licenceHoldersRepository;
    private readonly ILicenceHolderLinkRepository _licenceHoldersLinkRepository;
    private readonly IOrganisationsRepository _organisationsRepository;
    private readonly IDateTimeProvider _datetimeProvider;

    /// <summary>
    /// DI Constructor
    /// </summary>
    public EndLinkLicenceHolderCommandHandler(
        ILogger<BaseRequestHandler<EndLinkLicenceHolderCommand, ActionResult>> logger,
        ISchemeYearService schemeYearService,
        ILicenceHolderRepository licenceHoldersRepository,
        ILicenceHolderLinkRepository licenceHoldersLinkRepository,
        IOrganisationsRepository organisationsRepository,
        IDateTimeProvider datetimeProvider
        ) : base(logger)
    {
        _schemeYearService = schemeYearService;
        _licenceHoldersRepository = licenceHoldersRepository;
        _licenceHoldersLinkRepository = licenceHoldersLinkRepository;
        _organisationsRepository = organisationsRepository;
        _datetimeProvider = datetimeProvider;
    }

    /// <summary>
    /// Handles the command
    /// </summary>
    /// <param name="command">Details of the command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the licence holders</returns>
    public override async Task<ActionResult> Handle(EndLinkLicenceHolderCommand command, CancellationToken cancellationToken)
    {
        var schemeYearResponse = await _schemeYearService.GetCurrentSchemeYear(cancellationToken);
        if (!schemeYearResponse.IsSuccessStatusCode || schemeYearResponse.Result == null)
            return CannotLoadCurrentSchemeYear(schemeYearResponse.Problem);
        var currentSchemeYear = schemeYearResponse.Result;

        // BEGIN validation for ending a licence holder link after the surrender day date
        // You cannot end a licence holder link before the previous years surrender day 
        // if we've passed it.
        DateOnly surrenderDayDate;
        if (currentSchemeYear.PreviousSchemeYearId == null)
        {
            surrenderDayDate = currentSchemeYear.SurrenderDayDate;
        }
        else
        {
            var previousSchemeYearResponse = await _schemeYearService.GetSchemeYear(currentSchemeYear.PreviousSchemeYearId.Value, cancellationToken);
            if (!previousSchemeYearResponse.IsSuccessStatusCode || previousSchemeYearResponse.Result == null)
                return CannotLoadSchemeYear(currentSchemeYear.PreviousSchemeYearId.Value, previousSchemeYearResponse.Problem);
            var previousSchemeYear = previousSchemeYearResponse.Result;

            surrenderDayDate = previousSchemeYear.SurrenderDayDate;
        }

        var now = _datetimeProvider.UtcDateNow;
        if (now > surrenderDayDate && command.EndDate < surrenderDayDate)
            return InvalidLicenceHolderEndDate(surrenderDayDate);
        // END validation for end date

        var licenceHolder = await _licenceHoldersRepository.Get(x => x.Id == command.LicenceHolderId, includeLicenceHolderLinks: true, withTracking: true);
        if (licenceHolder == null)
            return CannotFindLicenceHolder(command.LicenceHolderId);

        // If all links have an end date for the organisation, then it's already ended.
        if (licenceHolder.LicenceHolderLinks == null || !licenceHolder.LicenceHolderLinks.Any(i => i.OrganisationId == command.OrganisationId && i.EndDate == null))
            return LicenceHolderAlreadyUnlinked(command.LicenceHolderId);

        var existingLink = licenceHolder.LicenceHolderLinks.Single(i => i.OrganisationId == command.OrganisationId && i.EndDate == null);

        // You cannot end the link before it has started
        if (command.EndDate <= existingLink.StartDate)
            return InvalidLicenceHolderEndDate(command.EndDate, existingLink.StartDate);

        existingLink.EndLink(command.EndDate);

        if (command.OrganisationIdToTransfer.HasValue)
        {
            var organisation = await _organisationsRepository.GetById(command.OrganisationIdToTransfer.Value);
            if (organisation == null)
                return CannotFindOrganisation(command.OrganisationIdToTransfer.Value);

            if (organisation.Status != OrganisationConstants.Status.Active)
                return InvalidOrganisationStatus(command.OrganisationIdToTransfer.Value, organisation.Status);

            var link = new LicenceHolderLink(command.OrganisationIdToTransfer.Value, licenceHolder.Id, currentSchemeYear.StartDate, command.EndDate.AddDays(1));
            var linkId = await _licenceHoldersLinkRepository.Create(link);
            return Responses.Created(linkId);
        }

        await _licenceHoldersLinkRepository.UnitOfWork.SaveChangesAsync();
        return Responses.NoContent();
    }
}
