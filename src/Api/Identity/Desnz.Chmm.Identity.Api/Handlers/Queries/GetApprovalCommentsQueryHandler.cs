using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries
{
    public class GetApprovalCommentsQueryHandler : BaseRequestHandler<GetApprovalCommentsQuery, ActionResult<OrganisationApprovalCommentsDto>>
    {
        private readonly IOrganisationsRepository _organisationsRepository;
        private readonly IOrganisationDecisionCommentsRepository _organisationCommentsRepository;
        private readonly IOrganisationDecisionFilesRepository _organisationApprovalFilesRepository;
        private readonly ICurrentUserService _userService;

        public GetApprovalCommentsQueryHandler(
            ILogger<BaseRequestHandler<GetApprovalCommentsQuery, ActionResult<OrganisationApprovalCommentsDto>>> logger,
            IOrganisationDecisionCommentsRepository organisationCommentsRepository,
            IOrganisationDecisionFilesRepository organisationApprovalFilesRepository,
            IOrganisationsRepository organisationsRepository,
            ICurrentUserService userService) : base(logger)
        {
            _organisationCommentsRepository = organisationCommentsRepository;
            _organisationApprovalFilesRepository = organisationApprovalFilesRepository;
            _organisationsRepository = organisationsRepository;
            _userService = userService;
        }

        public override async Task<ActionResult<OrganisationApprovalCommentsDto>> Handle(GetApprovalCommentsQuery query, CancellationToken cancellationToken)
        {
            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
                return UserNotAuthenticated();

            var organisation = await _organisationsRepository.GetById(query.OrganisationId, false, true, withTracking: true);
            if (organisation == null)
                return CannotFindOrganisation(query.OrganisationId);

            var comments = await _organisationCommentsRepository.GetByOrganisationIdAndDecision(query.OrganisationId, OrganisationFileConstants.Decisions.Approve, false);
            var files = await _organisationApprovalFilesRepository.GetByOrganisationIdAndDecision(query.OrganisationId, OrganisationFileConstants.Decisions.Approve, false);

            return new OrganisationApprovalCommentsDto()
            {
                Comments = comments?.Comment,
                FileNames = files?.Select(f => f.FileName).ToArray() ?? Array.Empty<string>()
            };
        }
    }
}
