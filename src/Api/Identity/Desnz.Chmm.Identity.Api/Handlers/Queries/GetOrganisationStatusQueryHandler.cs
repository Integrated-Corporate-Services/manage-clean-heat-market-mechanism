using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries
{
    public class GetOrganisationStatusQueryHandler : BaseRequestHandler<GetOrganisationStatusQuery, ActionResult<OrganisationStatusDto>>
    {
        private readonly IOrganisationsRepository _organisationsRepository;


        public GetOrganisationStatusQueryHandler(
            ILogger<BaseRequestHandler<GetOrganisationStatusQuery, ActionResult<OrganisationStatusDto>>> logger,
             IOrganisationsRepository organisationsRepository) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
        }

        public override async Task<ActionResult<OrganisationStatusDto>> Handle(GetOrganisationStatusQuery request, CancellationToken cancellationToken)
        {
            var organisation = await _organisationsRepository.Get(o => o.Id == request.OrganisationId);
            if (organisation == null)
                return CannotFindOrganisation(request.OrganisationId);

            return new OrganisationStatusDto()
            {
                Status = organisation.Status
            };
        }
    }
}
