using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Configuration.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Api.Handlers.Queries
{
    public class GetFirstSchemeYearQueryHandler : BaseRequestHandler<GetFirstSchemeYearQuery, ActionResult<SchemeYearDto>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IMapper _mapper;

        public GetFirstSchemeYearQueryHandler(
            ILogger<BaseRequestHandler<GetFirstSchemeYearQuery, ActionResult<SchemeYearDto>>> logger,
            ISchemeYearRepository schemeYearRepository,
            IMapper mapper) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _mapper = mapper;
        }

        public override async Task<ActionResult<SchemeYearDto>> Handle(GetFirstSchemeYearQuery request, CancellationToken cancellationToken)
        {
            var schemeYear = await _schemeYearRepository.GetFirstSchemeYearAsync();
            return _mapper.Map<SchemeYearDto>(schemeYear);
        }
    }
}
