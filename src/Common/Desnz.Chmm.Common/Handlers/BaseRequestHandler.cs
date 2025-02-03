using Desnz.Chmm.Common.Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Desnz.Chmm.Common.Handlers
{
    /// <summary>
    /// A Base for IRequestHandler providing additional functionality common to all handlers
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public abstract class BaseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<BaseRequestHandler<TRequest, TResponse>> _logger;

        /// <summary>
        /// Constructor for Dependency Injection
        /// </summary>
        /// <param name="logger">An ILogger</param>
        /// <exception cref="ArgumentNullException">Thrown if ILogger is null</exception>
        public BaseRequestHandler(ILogger<BaseRequestHandler<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from the request</returns>
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

        private static string FailedToGet(string @object, string @for, string forIdProperty, bool hasProblem = false)
            => $"Failed to get {@object} for {@for} with Id: {{{forIdProperty}}}" + AppendProblem(hasProblem);
        private static string FailedToGet(string @object, string forIdProperty, bool hasProblem = false)
            => $"Failed to get {@object} with Id: {{{forIdProperty}}}" + AppendProblem(hasProblem);
        private static string FailedToGet(string @object, bool hasProblem = true)
            => $"Failed to get {@object}" + AppendProblem(hasProblem);
        private static string AppendProblem(bool hasProblem)
        {
            var message = "";
            if (hasProblem)
            {
                message = ", problem: {problem}";
            }
            return message;
        }

        #region User Errors
        public UnauthorizedObjectResult UserNotAuthenticated()
        {
            return LogErrorAndReturnUnauthorizedRequest("You are not authenticated");
        }
        public NotFoundObjectResult CannotFindUser(Guid userId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("User", "userId", false), userId);
        }
        public BadRequestObjectResult CannotLoadManufacturerUsers(Guid organisationId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Organiastion Users", "Organisation", "organisationId", true), organisationId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadAdminUsers(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Admin Users", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadUser(Guid userId)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("User", "userId", false), userId);
        }
        public BadRequestObjectResult UserAlreadyExists(string email)
        {
            return LogErrorAndReturnBadRequest("User already exists with email \"{email}\"", email);
        }
        public BadRequestObjectResult UserAlreadyExistsForDifferentOrganisation()
        {
            return LogErrorAndReturnBadRequest("Unable to save details, please contact the Administrator");
        }
        public BadRequestObjectResult UserAlreadyExistsWhilstOnboarding()
        {
            return LogErrorAndReturnBadRequest("Registration cannot be completed, contact the Administrator.");
        }
        public BadRequestObjectResult InvalidUserStatus(Guid userId, string status)
        {
            return LogErrorAndReturnBadRequest("User with Id: {userId} has an invalid status: {status}", userId, status);
        }
        public BadRequestObjectResult CannotDeactivateYourself()
        {
            return LogErrorAndReturnBadRequest("You cannot deactivate yourself");
        }
        public BadRequestObjectResult CannotDeactivateUserAcrossOrganisation(Guid organiationId, Guid userId)
        {
            return LogErrorAndReturnBadRequest("User with Id: {userId} is not part of Organisation {organisationId} and cannot be deactivated as you do not have permission", userId, organiationId);
        }
        public BadRequestObjectResult CannotEditUserAcrossOrganisation(Guid organiationId, Guid userId)
        {
            return LogErrorAndReturnBadRequest("User with Id: {userId} is not part of Organisation {organisationId} and cannot be edited", userId, organiationId);
        }
        public BadRequestObjectResult NonManufacturerUserEditingAtempted(Guid userId)
        {
            return LogErrorAndReturnBadRequest("The user with Id: {userId} must be a Manufacturer in order to be editable", userId);
        }

        public BadRequestObjectResult CannotEditUserOrganisation(Guid userId)
        {
            return LogErrorAndReturnBadRequest("User with Id: {userId} cannot make user changes", userId);
        }

        public NotFoundObjectResult CannotLoadUser(string errorMessage)
        {
            return LogErrorAndReturnNotFound(errorMessage);
        }
        public NotFoundObjectResult CannotFindRole(string role)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Role", "name", false), role);
        }
        public NotFoundObjectResult CannotLoadRole(IEnumerable<Guid> roleIds)
        {
            return LogErrorAndReturnNotFound(FailedToGet("User Roles", "roleId", false), string.Join(", ", roleIds));
        }
        #endregion

        #region Organisation Errors
        public BadRequestObjectResult MustSpeficyAddressDetails(Guid organisationId, string legalAddressType)
        {
            return LogErrorAndReturnBadRequest("Cannot update Organisation with Id: {organisationId}, speficying no address and {addressType}", organisationId, legalAddressType);
        }
        public BadRequestObjectResult MustSpeficyAddressDetails(string legalAddressType)
        {
            return LogErrorAndReturnBadRequest("Cannot onboard Organisation speficying no address and {addressType}", legalAddressType);
        }
        public BadRequestObjectResult MustNotSpeficyAddressDetails(Guid organisationId, string legalAddressType)
        {
            return LogErrorAndReturnBadRequest("Cannot update Organisation with Id: {organisationId}, speficying address and {addressType}", organisationId, legalAddressType);
        }
        public BadRequestObjectResult MustNotSpeficyAddressDetails(string legalAddressType)
        {
            return LogErrorAndReturnBadRequest("Cannot onboard Organisation speficying address and {addressType}", legalAddressType);
        }
        public BadRequestObjectResult MustSpeficySroDetails(Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Cannot update Organisation with Id: {organisationId}, speficying no details and Applicant is not Senior Responsible Officer", organisationId);
        }
        public BadRequestObjectResult MustNotSpeficySroDetails(Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Cannot update Organisation with Id: {organisationId}, speficying details and Applicant is Senior Responsible Officer", organisationId);
        }
        public NotFoundObjectResult CannotFindOrganisation(Guid organisationId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Organisation", "organisationId", false), organisationId);
        }
        public NotFoundObjectResult CannotFindOrganisations(List<Guid> organisationIds)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Organisations", "organisationId", false), string.Join(", ", organisationIds));
        }
        public BadRequestObjectResult CannotLoadOrganisation(Guid organisationId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Organisation", "organisationId", true), organisationId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadOrganisation(Guid organisationId)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Organisation", "organisationId", false), organisationId);
        }
        public BadRequestObjectResult CannotLoadOrganisations(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("all Organisations", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotTransferToOrganisation(Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Cannot transfer credits to Organisation with Id: {organisationId}", organisationId);
        }
        public BadRequestObjectResult InvalidOrganisationStatus(Guid organisationId, string status)
        {
            return LogErrorAndReturnBadRequest("Organisation with Id: {organisationId} has an invalid status: {status}", organisationId, status);
        }

        public BadRequestObjectResult CannotTransferCreditsToSameOrganisation(Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Cannot transfer credits to the same organisation, Id: {organisationId}", organisationId);
        }

        public BadRequestObjectResult CannotAssignSroAsUserIsNotPartOfOrganisation(Guid organisationId, Guid userId)
        {
            return LogErrorAndReturnBadRequest("Cannot assign user {userId} as SRO for organisation {organisationId} because user is not part of organisation", userId, organisationId);
        }

        public BadRequestObjectResult CannotAssignSroAsUserAsTheyAreNotActive(Guid organisationId, Guid userId)
        {
            return LogErrorAndReturnBadRequest("Cannot assign user {userId} as SRO for organisation {organisationId} because they are not Active", userId, organisationId);
        }
        #endregion

        #region Mcs Manufacturer Errors
        public BadRequestObjectResult CannotChangeMcsManufacturerName(Guid organisationId, string fromName, string toName)
        {
            return LogErrorAndReturnBadRequest("Cannot change Licence Holder with Id: {organisationId} from: {organisationName} to: {organisationName}", organisationId, fromName, toName);
        }

        public BadRequestObjectResult CannotGetManufacturerInstallationRequests(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot installation requests for Scheme Year with Id: {organisationId}", schemeYearId);
        }
        #endregion

        #region Obligation Errors
        public BadRequestObjectResult CannotLoadObligationTotals(Guid schemeYearId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Obligation Totals", "Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadAnnualObligation(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("annual Obligations", "Scheme Year", "schemeYearId", false), schemeYearId);
        }
        public BadRequestObjectResult CannotAdjustObligationsAfterEndOfSchemeYear(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot adjust Obligations after end of Scheme Year with Id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult ObligationAlreadySumbitted(Guid schemeYearId, Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Annual Obligation for Scheme Year with Id: {schemeYearId} and Organisation with Id {organisationId} have already been submitted.", schemeYearId, organisationId);
        }
        public BadRequestObjectResult ObligationAlreadySumbitted(Guid schemeYearId, Guid quarterId, Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Quarter Obligation for Scheme Year with Id: {schemeYearId}, Quarter Id: {quarterId} and Organisation with Id {organisationId} have already been submitted.", schemeYearId, quarterId, organisationId);
        }
        #endregion

        #region Licence Holder Errors
        public NotFoundObjectResult CannotFindLicenceHolder(Guid licenceHolderId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Licence Holder", "licenceHolderId", false), licenceHolderId);
        }
        public BadRequestObjectResult LicenceHolderAlreadyLinked(Guid licenceHolderId)
        {
            return LogErrorAndReturnBadRequest("Licence holder {licenceHolderId} already has an ongoing link", licenceHolderId);
        }
        public BadRequestObjectResult LicenceHolderAlreadyUnlinked(Guid licenceHolderId)
        {
            return LogErrorAndReturnBadRequest("Licence holder {licenceHolderId} is already unlinked", licenceHolderId);
        }
        public BadRequestObjectResult CannotLoadLicenceHolders(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("all Licence Holders", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadLicenceHolder(Guid licenceHolderId)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Licence Holder", "licenceHolderId", false), licenceHolderId);
        }
        public BadRequestObjectResult CannotLoadLicenceHolderLinks(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("all Licence Holder Links", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadLicenceHolders(Guid organisationId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Licence Holders", "Organisation", "organisationId", true), organisationId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult LicenceHolderNotReturned(int manufacturerId, IEnumerable<Guid> foundIds)
        {
            return LogErrorAndReturnBadRequest("Failed to get Manufacturer with Id: {manufacturerId}, found {manufacturerCount} Ids: {manufacturerIds}", manufacturerId, foundIds.Count(), string.Join(", ", foundIds));
        }
        public BadRequestObjectResult InvalidLicenceHolderStartDate(DateOnly schemeYearStartDate)
        {
            return LogErrorAndReturnBadRequest("Licence holder start date must not be before the scheme year start date: {schemeYearStartDate}", schemeYearStartDate);
        }

        public BadRequestObjectResult InvalidLicenceHolderEndDate(DateOnly surrenderDayDate)
        {
            return LogErrorAndReturnBadRequest("End date of the link must be after the surrender day date of the scheme: {surrenderDayDate}", surrenderDayDate);
        }
        public BadRequestObjectResult InvalidLicenceHolderEndDate(DateOnly startDate, DateOnly endDate)
        {
            return LogErrorAndReturnBadRequest("End date of the link {endDate} must be after the start date of the link: {startDate}", endDate, startDate);
        }
        #endregion

        #region Scheme Year Errors
        public BadRequestObjectResult CannotLoadCreditWeightings(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Credit Weightings", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadAllSchemeYears(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("all Scheme Years", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadNextSchemeYear(Guid schemeYearId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("next Scheme Year", "Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadNextSchemeYear(Guid previousSchemeYearId)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("next Scheme Year", "Scheme Year", "schemeYearId"), previousSchemeYearId);
        }
        public BadRequestObjectResult CannotLoadSchemeYear(Guid schemeYearId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadSchemeYear(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Scheme Year", "schemeYearId"), schemeYearId);
        }
        public BadRequestObjectResult CannotLoadCurrentSchemeYear(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("current Scheme Year", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadPreviousSchemeYear(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("previous Scheme Year", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadFirstSchemeYear(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("first Scheme Year", true), JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult SchemeYearExists(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Scheme Year with Id: {schemeYearId} already exists", schemeYearId);
        }
        public BadRequestObjectResult CannotLoadSchemeYearByPreviousId(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot retrieve the Scheme Year with previous Scheme Year Id: {previousSchemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult CannotEditSchemeYearConfiguration(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot edit the configuration of Scheme Year with Id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult InvalidLoadSchemeYearQuarter(Guid schemeYearId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Scheme Year Quarter", "Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }
        #endregion

        #region Boiler Sales Errors
        public BadRequestObjectResult CannotSubmitQuarterlyBoilerFiguresOutsideWindow(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot submit quarterly boiler sales figures outside adjustment window for Scheme Year with Id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult CannotSubmitAnnualBoilerFiguresOutsideWindow(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot submit annual boiler sales figures outside adjustment window for Scheme Year with Id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult CannotEditAnnualBoilerFiguresOutsideWindow(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot adjust annual boiler sales figures outside adjustment window for Scheme Year with Id: {schemeYearId}", schemeYearId);
        }
        public NotFoundObjectResult CannotLoadBoilerSalesData(Guid schemeYearId, Guid organisationId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Annual boiler sales data", "organisation", "organisationId"), organisationId);
        }
        public NotFoundObjectResult CannotLoadBoilerSalesData(Guid quarterSalesId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Quarterly boiler sales data", "Id"), quarterSalesId);
        }
        public NotFoundObjectResult CannotLoadBoilerSalesData(Guid schemeYearId, Guid quarterId, Guid organisationId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Quarterly boiler sales data", "quarter", "quarterId", false), quarterId);
        }
        public NotFoundObjectResult CannotLoadBoilerSalesData(Guid schemeYearId, Guid quarterId, Guid organisationId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Quarter boiler sales data", "organisation", "organisationId", true), organisationId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult InvalidAnnualBoilerSalesDataStatus(Guid organisationId, string status)
        {
            return LogErrorAndReturnBadRequest("Annual boiler sales data for: {organisationId} has an invalid status: {status}", organisationId, status);
        }
        public BadRequestObjectResult InvalidQuarterlyBoilerSalesDataStatus(Guid organisationId, string status)
        {
            return LogErrorAndReturnBadRequest("Quarterly boiler sales data for: {organisationId} has an invalid status: {status}", organisationId, status);
        }
        public BadRequestObjectResult AnnualSalesDataAlreadyExists(Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Annual boiler sales for organisation with Id: {organisationId} already exist.", organisationId);
        }
        public BadRequestObjectResult QuarterSalesDataAlreadyExists(Guid organisationId)
        {
            return LogErrorAndReturnBadRequest("Quarter boiler sales for organisation with Id: {organisationId} already exist.", organisationId);
        }
        public BadRequestObjectResult CannotLoadBoilerSalesSummaries(Guid schemeYearId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Boiler Sales summaries", "Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult NoBoilerSalesSummaries(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("No Boiler Sales found for scheme year id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult UnapprovedBoilerSales(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Unapproved Annual Boiler Sales found for scheme year id: {schemeYearId}", schemeYearId);
        }
        #endregion

        #region Obligation Errors
        public BadRequestObjectResult CannotCreateYearlyObligation(Guid organisationId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest("Failed to create yearly obligation for organisation with Id: {organisationId}, problem: {problem}", organisationId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotCreateQuarterlyObligation(Guid organisationId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest("Failed to create quarterly obligation for organisation with Id: {organisationId}, problem: {problem}", organisationId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadObligationCalculations(Guid schemeYearId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Obligation Calculations", "Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadObligationCalculations(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Obligation Calculations", "Scheme Year", "schemeYearId"), schemeYearId);
        }
        public BadRequestObjectResult CarryForwardObligationProcessAlreadyRun(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Carry Forward obligation is already generated for some organisations for the Scheme Year Id: {schemeYearId}", schemeYearId);
        }
        #endregion

        #region File Errors
        public BadRequestObjectResult ErrorDeletingFile(string error)
        {
            return LogErrorAndReturnBadRequest(error);
        }
        public BadRequestObjectResult ErrorUploadingFile(string error)
        {
            return LogErrorAndReturnBadRequest(error);
        }
        public BadRequestObjectResult ErrorDownloadingFile(string error)
        {
            return LogErrorAndReturnBadRequest(error);
        }
        #endregion

        #region Credit Errors
        public BadRequestObjectResult CannotAdjustCreditsOutsideAdjustmentWindow(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot adjust credits outside adjustment window for Scheme Year with Id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult CannotTradeOutsideWindow(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Cannot trade credits outside of the trading window of Scheme Year with Id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult NotEnoughCredits(decimal currentCreditBalance)
        {
            return LogErrorAndReturnBadRequest("The current credit balance: {currentCreditBalance} is insufficient to complete this action", currentCreditBalance);
        }
        public BadRequestObjectResult CannotLoadCreditBalances(Guid schemeYearId, ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest(FailedToGet("Credit Balances", "Scheme Year", "schemeYearId", true), schemeYearId, JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult NoCreditBalances(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("No Credit Balances found for scheme year id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult CarryForwardCreditProcessAlreadyRun(Guid schemeYearId)
        {
            return LogErrorAndReturnBadRequest("Carry Forward Credit is already generated for some organisations for the Scheme Year Id: {schemeYearId}", schemeYearId);
        }
        public BadRequestObjectResult FailedToGetInstallationCredits(DateOnly startDate, DateOnly endDate)
        {
            return LogErrorAndReturnBadRequest("Failed to get Installation Credits for the period: [{startDate}..{endDate}]", startDate, endDate);
        }
        #endregion

        #region Note Errors
        public NotFoundObjectResult CannotFindNote(Guid noteId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Note", "noteId", false), noteId);
        }
        #endregion

        public BadRequestObjectResult CannotLoadJwtToken(ValueObjects.ProblemDetails? problemDetails)
        {
            return LogErrorAndReturnBadRequest("Failed to retrieve CHMM token from Identity Service: {problem}", JsonConvert.SerializeObject(problemDetails));
        }
        public BadRequestObjectResult CannotLoadHeatPumpInstallations(DateTime startDate, DateTime endDate, int[]? technologyTypeIds, int[]? isNewBuildIds, ValueObjects.ProblemDetails? problem)
        {
            return LogErrorAndReturnBadRequest("Failed to retrieve MCS Installations, problem: {problem}", JsonConvert.SerializeObject(problem));
        }
        public BadRequestObjectResult CannotGenerateCredits(ValueObjects.ProblemDetails? problem)
        {
            return LogErrorAndReturnBadRequest("Failed to generate credits, problem: {problem}", JsonConvert.SerializeObject(problem));
        }
        public BadRequestObjectResult CannotCarryOverNewLicenceHolders(ValueObjects.ProblemDetails? problem)
        {
            return LogErrorAndReturnBadRequest("Failed to carry over credits for new licence holders, problem: {problem}", JsonConvert.SerializeObject(problem));
        }
        public ObjectResult ExceptionPersistingMcsData(Exception exception)
        {
            return LogErrorAndReturnException("Error persisting the MSC data, exception: {exception}", exception.Message);
        }
        public BadRequestObjectResult McsDataAlreadyExists(DateTime startDate, DateTime endDate)
        {
            return LogErrorAndReturnBadRequest("MCS data already exists for the week of {startDate} -> {endDate}", startDate, endDate);
        }
        public NotFoundObjectResult CannotFindRequestData(Guid requestId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Request data", "Request", "requestId", false), requestId);
        }
        public NotFoundObjectResult CannotFindRequest(Guid requestId)
        {
            return LogErrorAndReturnNotFound(FailedToGet("Request", "Request", "requestId", false), requestId);
        }
        public ObjectResult ErrorGettingInstallations(Guid requestId, Exception exception)
        {
            return LogErrorAndReturnException("Error getting Installations for Request Id: {requestId}, exception: {exception}", requestId, exception.Message);
        }
        public ObjectResult ErrorGettingInstallations(Exception exception)
        {
            return LogErrorAndReturnException("Error getting Installations, exception: {exception}", exception.Message);
        }
        public BadRequestObjectResult EndOfYearProcessFailed(ValueObjects.ProblemDetails? problem)
        {
            return LogErrorAndReturnBadRequest("End of year processing failed, please review logs and resolve any issues, problem: {problem}", JsonConvert.SerializeObject(problem));
        }
        public BadRequestObjectResult RollingBackEndOfYearProcessFailed(ValueObjects.ProblemDetails? problem)
        {
            return LogErrorAndReturnBadRequest("Rolling back end of year processing failed, please review logs and resolve any issues, problem: {problem}", JsonConvert.SerializeObject(problem));
        }
        public NotFoundObjectResult CannotFindCreditOrObligationTotal(Guid organisationId)
        {
            return LogErrorAndReturnNotFound("Could not find a credit or obligation total for Organisation with Organisation Id: {organisationId}", organisationId);
        }
        public BadRequestObjectResult CannotLogObligationRedemption(Guid schemeYearId, ValueObjects.ProblemDetails? problem)
        {
            return LogErrorAndReturnBadRequest("Could not log Obligation redemption for Scheme Year with Scheme Year Id: {schemeYearId}, problem: {problem}", schemeYearId, JsonConvert.SerializeObject(problem));
        }
        public BadRequestObjectResult CannotLogCreditRedemption(Guid schemeYearId, ValueObjects.ProblemDetails? creditProblem)
        {
            return LogErrorAndReturnBadRequest("Could not log Credit redemption for Scheme Year with Scheme Year Id: {schemeYearId}, problem: {problem}, Obligation redemption rolled back", schemeYearId, JsonConvert.SerializeObject(creditProblem));
        }
        public BadRequestObjectResult CannotLogCreditRedemption(Guid schemeYearId, ValueObjects.ProblemDetails? creditProblem, ValueObjects.ProblemDetails? rollbackProblem)
        {
            return LogErrorAndReturnBadRequest("Could not log Credit redemption for Scheme Year with Scheme Year Id: {schemeYearId}, problem: {problem}, Obligations failed to roll back, problem: {rollbackProblem}", schemeYearId, JsonConvert.SerializeObject(creditProblem), JsonConvert.SerializeObject(rollbackProblem));
        }
        public BadRequestObjectResult CannotRollbackRedemptionProcess(ValueObjects.ProblemDetails? problem)
        {
            return LogErrorAndReturnBadRequest("Could not rollback redemption: {problem}", JsonConvert.SerializeObject(problem));
        }
        public BadRequestObjectResult CannotCreateLicenceHolders(ValueObjects.ProblemDetails? problem)
        {
            return LogErrorAndReturnBadRequest("Failed to create Licence Holders, problem: {problem}", JsonConvert.SerializeObject(problem));
        }

        private ObjectResult LogErrorAndReturnException(string? message, params object?[] args)
        {
            var processedMessage = LogMessage(message, args);

            return new ObjectResult(string.Format(processedMessage, args))
            {
                StatusCode = 500
            };
        }
        public NotFoundObjectResult LogErrorAndReturnNotFound(string? message, params object?[] args)
        {
            var processedMessage = LogMessage(message, args);
            return Responses.NotFound(string.Format(processedMessage, args));
        }

        public BadRequestObjectResult LogErrorAndReturnBadRequest(string? message, params object?[] args)
        {
            var processedMessage = LogMessage(message, args);
            return Responses.BadRequest(string.Format(processedMessage, args));
        }
        public ObjectResult ExceptionCopyingS3Files(string error)
        {
            return LogErrorAndReturnException("Error copying S3 files: exception: {error}", error);
        }
        public ObjectResult ExceptionDeletingS3Files(string error)
        {
            return LogErrorAndReturnException("Error deleting S3 files: exception: {error}", error);
        }
        public ObjectResult ExceptionPreparingS3FilesForEditing(IEnumerable<string?> errors)
        {
            return LogErrorAndReturnException("Error preparing S3 files for editing: exception: {error}", string.Join("; ", errors));
        }
        public ObjectResult ExceptionConcludingS3FilesForEditing(IEnumerable<string?> errors)
        {
            return LogErrorAndReturnException("Error concluding S3 files for editing: exception: {error}", string.Join("; ", errors));
        }
        private UnauthorizedObjectResult LogErrorAndReturnUnauthorizedRequest(string? message, params object?[] args)
        {
            var processedMessage = LogMessage(message, args);
            return Responses.Unauthorized(string.Format(processedMessage, args));
        }

        private string LogMessage(string? message, params object?[] args)
        {
            _logger.LogError(message, args);
            message ??= string.Empty;
            string? processedMessage;
            if (message.IndexOf("{") > 0)
            {
                processedMessage = ReplaceWithIncrementingNumbers(message);
            }
            else
            { processedMessage = message; }
            return processedMessage;
        }

        private static string ReplaceWithIncrementingNumbers(string input)
        {
            StringBuilder result = new StringBuilder();
            int count = 0;

            int start = 0;
            int end;

            while ((end = input.IndexOf('{', start)) != -1)
            {
                result.Append(input, start, end - start);

                start = input.IndexOf('}', end) + 1;
                result.Append("{" + count++ + "}");
            }

            result.Append(input, start, input.Length - start);

            return result.ToString();
        }
    }
}
