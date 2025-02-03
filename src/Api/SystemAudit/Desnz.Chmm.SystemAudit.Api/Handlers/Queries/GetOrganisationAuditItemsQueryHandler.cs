using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Infrastructure.Repositories;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.SystemAudit.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.SystemAudit.Api.Handlers.Queries
{
    public class GetOrganisationAuditItemsQueryHandler : BaseRequestHandler<GetOrganisationAuditItemsQuery, ActionResult<List<AuditItemDto>>>
    {
        private readonly IAuditItemRepository _auditItemRepository;
        private readonly IUserService _userService;
        private readonly IRequestValidator _requestValidator;

        public GetOrganisationAuditItemsQueryHandler(
            ILogger<BaseRequestHandler<GetOrganisationAuditItemsQuery, ActionResult<List<AuditItemDto>>>> logger, 
            IAuditItemRepository auditItemRepository, 
            IUserService userService,
            IRequestValidator requestValidator) : base(logger)
        {
            _auditItemRepository = auditItemRepository;
            _userService = userService;
            _requestValidator = requestValidator;
        }

        public override async Task<ActionResult<List<AuditItemDto>>> Handle(GetOrganisationAuditItemsQuery query, CancellationToken cancellationToken)
        {
            var organisationId = query.OrganisationId;

            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                organisationId: organisationId);
            if (validationError != null)
                return validationError;

            var getManufacturerUsersResponse = await _userService.GetManufacturerUsers(query.OrganisationId);
            if (!getManufacturerUsersResponse.IsSuccessStatusCode || getManufacturerUsersResponse.Result == null)
                return CannotLoadManufacturerUsers(organisationId, getManufacturerUsersResponse.Problem);

            var getAdminUsersResponse = await _userService.GetAdminUsers();
            if (!getAdminUsersResponse.IsSuccessStatusCode || getAdminUsersResponse.Result == null)
                return CannotLoadAdminUsers(getAdminUsersResponse.Problem);

            var users = new List<ChmmUserDto>();
            users.AddRange(getManufacturerUsersResponse.Result);
            users.AddRange(getAdminUsersResponse.Result);

            var auditItems = await _auditItemRepository.GetAuditItemsForOrganisation(organisationId);
            return auditItems.Select(a => a.ToDto(users)).ToList();
        }
    }
}
