using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual
{
    public class ApproveAnnualBoilerSalesCommandHandler : BaseRequestHandler<ApproveAnnualBoilerSalesCommand, ActionResult>
    {
        private readonly IAnnualBoilerSalesRepository _annualBoilerSalesRepository;
        private readonly IRequestValidator _requestValidator;

        public ApproveAnnualBoilerSalesCommandHandler(
            ILogger<BaseRequestHandler<ApproveAnnualBoilerSalesCommand, ActionResult>> logger,
            IAnnualBoilerSalesRepository annualBoilerSalesRepository,
            IRequestValidator requestValidator) : base(logger)
        {
            _annualBoilerSalesRepository = annualBoilerSalesRepository;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult> Handle(ApproveAnnualBoilerSalesCommand command, CancellationToken cancellationToken)
        {
            var organisationId = command.OrganisationId;
            var schemeYearId = command.SchemeYearId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(organisationId: organisationId, schemeYearId: schemeYearId, requireActiveOrganisation: true);
            if (validationError != null)
                return validationError;

            var annualBoilerSales = await _annualBoilerSalesRepository.Get(
            o =>
                o.OrganisationId == command.OrganisationId &&
                o.SchemeYearId == command.SchemeYearId,
                includeFiles: false,
                includeChanges: false,
                withTracking: true);

            if (annualBoilerSales == null)
                return CannotLoadBoilerSalesData(schemeYearId, organisationId);

            if (annualBoilerSales.Status != BoilerSalesStatus.Submitted)
                return InvalidAnnualBoilerSalesDataStatus(organisationId, annualBoilerSales.Status);

            annualBoilerSales.Approve();
            await _annualBoilerSalesRepository.UnitOfWork.SaveChangesAsync();

            return Responses.Ok();
        }
    }
}
