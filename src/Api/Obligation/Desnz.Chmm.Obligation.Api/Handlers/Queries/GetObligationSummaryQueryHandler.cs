using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Obligation.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Obligation.Api.Handlers.Queries
{
    /// <summary>
    /// Get the obligation summary for the given organisation
    /// </summary>
    public class GetObligationSummaryQueryHandler : BaseRequestHandler<GetObligationSummaryQuery, ActionResult<ObligationSummaryDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IObligationCalculator _obligationCalculator;
        private readonly IRequestValidator _requestValidator;

        public GetObligationSummaryQueryHandler(
            ILogger<BaseRequestHandler<GetObligationSummaryQuery, ActionResult<ObligationSummaryDto>>> logger,
            ITransactionRepository transactionRepository,
            IObligationCalculator obligationCalculator,
            IRequestValidator requestValidator) : base(logger)
        {
            _transactionRepository = transactionRepository;
            _obligationCalculator = obligationCalculator;
            _requestValidator = requestValidator;
        }

        /// <summary>
        /// Handle the query for an obligation summary
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The generated obligation summary</returns>
        public override async Task<ActionResult<ObligationSummaryDto>> Handle(GetObligationSummaryQuery query, CancellationToken cancellationToken)
        {
            var organisationId = query.OrganisationId;
            var schemeYearId = query.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                organisationId: organisationId,
                requireActiveOrganisation: true,
                schemeYearId: schemeYearId);
            if (validationError != null)
                return validationError;

            var transactions = await _transactionRepository.GetAll(t => 
                t.SchemeYearId == schemeYearId && 
                t.OrganisationId == organisationId && 
                t.IsExcluded == false,
                RepositoryConstants.SortOrder.Ascending, 
                false);

            return _obligationCalculator.GenerateSummary(transactions);
        }
    }
}
