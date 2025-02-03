using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries
{
    public class GetRejectionCommentsQueryHandler : BaseRequestHandler<GetRejectionCommentsQuery, ActionResult<OrganisationRejectionCommentsDto>>
    {
        private readonly IOrganisationsRepository _organisationsRepository;
        private readonly IOrganisationDecisionCommentsRepository _organisationCommentsRepository;
        private readonly IOrganisationDecisionFilesRepository _organisationRejectionFilesRepository;
        private readonly ICurrentUserService _userService;

        public GetRejectionCommentsQueryHandler(
            ILogger<BaseRequestHandler<GetRejectionCommentsQuery, ActionResult<OrganisationRejectionCommentsDto>>> logger,
            IOrganisationDecisionCommentsRepository organisationCommentsRepository,
            IOrganisationDecisionFilesRepository organisationRejectionFilesRepository,
            IOrganisationsRepository organisationsRepository,
            ICurrentUserService userService) : base(logger)
        {
            _organisationCommentsRepository = organisationCommentsRepository;
            _organisationRejectionFilesRepository = organisationRejectionFilesRepository;
            _organisationsRepository = organisationsRepository;
            _userService = userService;
        }

        public override async Task<ActionResult<OrganisationRejectionCommentsDto>> Handle(GetRejectionCommentsQuery query, CancellationToken cancellationToken)
        {
            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
                return UserNotAuthenticated();

            var organisation = await _organisationsRepository.GetById(query.OrganisationId, false, true, withTracking: true);
            if (organisation == null)
                return CannotFindOrganisation(query.OrganisationId);

            var comments = await _organisationCommentsRepository.GetByOrganisationIdAndDecision(query.OrganisationId, OrganisationFileConstants.Decisions.Reject, false);
            var files = await _organisationRejectionFilesRepository.GetByOrganisationIdAndDecision(query.OrganisationId, OrganisationFileConstants.Decisions.Reject, false);

            return new OrganisationRejectionCommentsDto()
            {
                Comments = comments?.Comment ?? string.Empty,
                FileNames = files?.Select(f => f.FileName).ToArray() ?? Array.Empty<string>(),
                RejectedBy = comments?.ChmmUser?.Name ?? "Unknown",
                RejectedOn = comments?.CreationDate ?? DateTime.MinValue
            };
        }
    }
}
