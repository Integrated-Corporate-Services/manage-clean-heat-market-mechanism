using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Obligation.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Api.Handlers.Queries
{
    public class GetSchemeYearObligationTotalsQueryHandler : BaseRequestHandler<GetSchemeYearObligationTotalsQuery, ActionResult<List<ObligationTotalDto>>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrganisationService _organisationService;
        private readonly IObligationCalculator _obligationCalculator;
        private readonly IRequestValidator _requestValidator;

        public GetSchemeYearObligationTotalsQueryHandler(
            ILogger<BaseRequestHandler<GetSchemeYearObligationTotalsQuery, ActionResult<List<ObligationTotalDto>>>> logger,
            ITransactionRepository transactionRepository,
            IOrganisationService organisationService,
            IObligationCalculator obligationCalculator,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _organisationService = organisationService;
            _obligationCalculator = obligationCalculator;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult<List<ObligationTotalDto>>> Handle(GetSchemeYearObligationTotalsQuery query, CancellationToken cancellationToken)
        {
            var schemeYearId = query.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var organisationsResponse = await _organisationService.GetManufacturers();
            if (organisationsResponse.Result == null || !organisationsResponse.IsSuccessStatusCode)
                return CannotLoadOrganisations(organisationsResponse.Problem);

            var activeOrganisations = organisationsResponse.Result
                .Where(x => x.Status == OrganisationConstants.Status.Active);

            var totals = new List<ObligationTotalDto>();

            foreach (var organisation in activeOrganisations)
            {
                var transactions = await _transactionRepository.GetAll(t =>
                    t.SchemeYearId == schemeYearId &&
                    t.OrganisationId == organisation.Id &&
                    t.IsExcluded == false,
                    RepositoryConstants.SortOrder.Ascending,
                    false);

                var summary = _obligationCalculator.GenerateSummary(transactions);

                var totalObligations = summary.FinalObligations;
                totals.Add(new ObligationTotalDto(organisation.Id, totalObligations));
            }

            return totals;
        }
    }
}
