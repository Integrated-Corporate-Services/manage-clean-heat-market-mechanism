using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetSchemeYearQueryHandler : BaseRequestHandler<GetSchemeYearQuery, ActionResult<SchemeYearDto>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IMapper _mapper;

        public GetSchemeYearQueryHandler(
            ILogger<BaseRequestHandler<GetSchemeYearQuery, ActionResult<SchemeYearDto>>> logger,
            ISchemeYearRepository schemeYearRepository, 
            IMapper mapper) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _mapper = mapper;
        }

        public override async Task<ActionResult<SchemeYearDto>> Handle(GetSchemeYearQuery request, CancellationToken cancellationToken)
        {
            var schemeYearData = await _schemeYearRepository.GetSchemeYear(i => i.Id == request.SchemeYearId);

            if (schemeYearData == null)
                return CannotLoadSchemeYear(request.SchemeYearId);

            var mappedData = _mapper.Map<SchemeYearDto>(schemeYearData);

            return mappedData;
        }
    }
}

