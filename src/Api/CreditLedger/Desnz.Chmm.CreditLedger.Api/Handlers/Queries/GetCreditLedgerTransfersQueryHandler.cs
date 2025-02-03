using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Queries
{
    public class GetCreditLedgerTransfersQueryHandler : BaseRequestHandler<GetCreditLedgerTransfersQuery, ActionResult<CreditLedgerTransfersDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrganisationService _orgnisationService;
        private readonly IUserService _userService;
        private readonly IRequestValidator _requestValidator;

        public GetCreditLedgerTransfersQueryHandler(
            ILogger<BaseRequestHandler<GetCreditLedgerTransfersQuery, ActionResult<CreditLedgerTransfersDto>>> logger,
            ITransactionRepository transactionRepository,
            IOrganisationService orgnisationService,
            IUserService userService,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _orgnisationService = orgnisationService;
            _userService = userService;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult<CreditLedgerTransfersDto>> Handle(GetCreditLedgerTransfersQuery query, CancellationToken cancellationToken)
        {
            var organisationId = query.OrganisationId;
            var schemeYearId = query.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                organisationId: organisationId,
                requireActiveOrganisation: true,
                schemeYearId: schemeYearId
                );
            if (validationError != null)
                return validationError;

            var getManufacturerUsersResponse = await _userService.GetManufacturerUsers(query.OrganisationId);
            if (!getManufacturerUsersResponse.IsSuccessStatusCode || getManufacturerUsersResponse.Result == null)
                return CannotLoadManufacturerUsers(organisationId, getManufacturerUsersResponse.Problem);
            var users = getManufacturerUsersResponse.Result;

            var transactions = await _transactionRepository.GetTransferTransactions(organisationId, schemeYearId);

            var transfersEntriesIn = transactions.Where(e => e.Entries.Single(i => i.OrganisationId == organisationId).Value >= 0);
            var transfersEntriesOut = transactions.Where(e => e.Entries.Single(i => i.OrganisationId == organisationId).Value < 0);

            var organisationIds = transactions.SelectMany(t => t.Entries.Select(e => e.OrganisationId)).ToList();
            var organisationsResponse = await _orgnisationService.GetManufacturerNames(new OrganisationNameLookupQuery(organisationIds));
            if (organisationsResponse.Result == null || !organisationsResponse.IsSuccessStatusCode)
                return CannotLoadOrganisations(organisationsResponse.Problem);
            var organisations = organisationsResponse.Result;

            //TODO - SpecifyKind is used here whilst we look in to wider issue of timestamp without time zone fields in the database
            var transfersIn = new List<CreditLedgerTransferInDto>();
            foreach (var transfer in transfersEntriesIn.OrderByDescending(i => i.DateOfTransaction))
            {
                var sourceId = transfer.Entries.Single(i => i.Value < 0).OrganisationId;
                var sourceOrganisationName = organisations.SingleOrDefault(i => i.Id == sourceId)?.Name;
                var value = transfer.Entries.Single(i => i.OrganisationId == organisationId).Value;
                transfersIn.Add(new CreditLedgerTransferInDto(DateTime.SpecifyKind(transfer.DateOfTransaction, DateTimeKind.Utc), sourceOrganisationName ?? "Unknown", value));
            }

            var transfersOut = new List<CreditLedgerTransferOutDto>();
            foreach (var transfer in transfersEntriesOut.OrderByDescending(i => i.DateOfTransaction))
            {
                var initiatedBy = users.SingleOrDefault(i => i.Id == transfer.InitiatedBy)?.Name;
                var destinationId = transfer.Entries.Single(i => i.Value >= 0).OrganisationId;
                var destinationOrganisationName = organisations.SingleOrDefault(i => i.Id == destinationId)?.Name;

                var value = transfer.Entries.Single(i => i.OrganisationId == organisationId).Value;
                transfersOut.Add(new CreditLedgerTransferOutDto(
                    DateTime.SpecifyKind(transfer.DateOfTransaction, DateTimeKind.Utc), destinationOrganisationName ?? "Unknown", value, initiatedBy ?? "Administrator"));
            }

            return new CreditLedgerTransfersDto(transfersIn, transfersOut);
        }
    }
}
