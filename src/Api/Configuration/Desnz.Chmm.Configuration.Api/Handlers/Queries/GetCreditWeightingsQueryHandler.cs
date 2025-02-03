using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetCreditWeightingsQueryHandler : BaseRequestHandler<GetCreditWeightingsQuery, ActionResult<CreditWeightingsDto>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IMapper _mapper;

        public GetCreditWeightingsQueryHandler(
            ILogger<BaseRequestHandler<GetCreditWeightingsQuery, ActionResult<CreditWeightingsDto>>> logger,
            ISchemeYearRepository schemeYearRepository,
            IMapper mapper) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _mapper = mapper;
        }

        public override async Task<ActionResult<CreditWeightingsDto>> Handle(GetCreditWeightingsQuery request, CancellationToken cancellationToken)
        {
            var schemeYearData = _schemeYearRepository.GetCreditWeighting(i => i.SchemeYearId == request.SchemeYearId);
            if (schemeYearData == null)
                return CannotLoadSchemeYear(request.SchemeYearId);

            var mappedData = _mapper.Map<CreditWeightingsDto>(schemeYearData);

            return mappedData;
        }
    }
}

