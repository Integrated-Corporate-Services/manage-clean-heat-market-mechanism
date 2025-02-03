using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetNextSchemeYearQueryHandler : BaseRequestHandler<GetNextSchemeYearQuery, ActionResult<SchemeYearDto>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IMapper _mapper;

        public GetNextSchemeYearQueryHandler(
            ILogger<BaseRequestHandler<GetNextSchemeYearQuery, ActionResult<SchemeYearDto>>> logger,
            ISchemeYearRepository schemeYearRepository, 
            IMapper mapper) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _mapper = mapper;
        }

        public override async Task<ActionResult<SchemeYearDto>> Handle(GetNextSchemeYearQuery request, CancellationToken cancellationToken)
        {
            var schemeYearData = await _schemeYearRepository.GetSchemeYear(i => i.PreviousSchemeYearId == request.SchemeYearId);

            if (schemeYearData == null)
                return CannotLoadSchemeYearByPreviousId(request.SchemeYearId);

            var mappedData = _mapper.Map<SchemeYearDto>(schemeYearData);

            return mappedData;
        }
    }
}

